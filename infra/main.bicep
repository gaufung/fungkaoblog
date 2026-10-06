targetScope = 'resourceGroup'

param location string = 'japanwest'
param webAppName string = 'fungkaoblog-pg'
param sharedResourceGroup string = 'transfer-hub-rg'
param postgresServerName string = 'transfer-hub-pg-yuemic'
param databaseName string = 'fungkaoblog'
param sharedVnetName string = 'transfer-hub-japanwest-vnet'
param privateDnsZoneName string = 'transfer-hub.postgres.database.azure.com'

@secure()
param databaseConnection string

var tags = {
  application: 'fungkaoblog'
  managedBy: 'bicep'
}

resource vnet 'Microsoft.Network/virtualNetworks@2024-05-01' = {
  name: '${webAppName}-vnet'
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.31.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'web'
        properties: {
          addressPrefix: '10.31.0.0/26'
          delegations: [
            {
              name: 'app-service'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
      {
        name: 'database-setup'
        properties: {
          addressPrefix: '10.31.1.0/28'
          delegations: [
            {
              name: 'container-instances'
              properties: {
                serviceName: 'Microsoft.ContainerInstance/containerGroups'
              }
            }
          ]
        }
      }
    ]
  }
}

resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2024-05-01' = {
  parent: vnet
  name: 'to-shared-postgres'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: resourceId(sharedResourceGroup, 'Microsoft.Network/virtualNetworks', sharedVnetName)
    }
  }
}

module shared './shared.bicep' = {
  name: 'fungkaoblog-shared-database-network'
  scope: resourceGroup(sharedResourceGroup)
  params: {
    postgresServerName: postgresServerName
    databaseName: databaseName
    sharedVnetName: sharedVnetName
    privateDnsZoneName: privateDnsZoneName
    blogVnetId: vnet.id
  }
}

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: '${webAppName}-plan'
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource app 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux'
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    virtualNetworkSubnetId: '${vnet.id}/subnets/web'
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      appCommandLine: 'dotnet Blog.Api.dll'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      vnetRouteAllEnabled: false
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
          value: 'true'
        }
        {
          name: 'ASPNETCORE_URLS'
          value: 'http://0.0.0.0:8080'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: databaseConnection
        }
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
  }
}

resource scmAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  parent: app
  name: 'scm'
  properties: {
    allow: false
  }
}

resource ftpAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  parent: app
  name: 'ftp'
  properties: {
    allow: false
  }
}

resource logs 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: app
  name: 'logs'
  properties: {
    applicationLogs: {
      fileSystem: {
        level: 'Information'
      }
    }
    httpLogs: {
      fileSystem: {
        enabled: true
        retentionInDays: 7
        retentionInMb: 35
      }
    }
  }
}

resource deployIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${webAppName}-github-deploy'
  location: location
  tags: tags
}

resource federation 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2023-01-31' = {
  parent: deployIdentity
  name: 'github-master'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:gaufung/fungkaoblog:ref:refs/heads/master'
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}

resource deployRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(app.id, deployIdentity.id, 'website-contributor')
  scope: app
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'de139f84-1756-47ae-9be6-808fbbe84772')
    principalId: deployIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

output siteUrl string = 'https://${app.properties.defaultHostName}'
output deployClientId string = deployIdentity.properties.clientId
output tenantId string = tenant().tenantId
output subscriptionId string = subscription().subscriptionId
output databaseSetupSubnetId string = '${vnet.id}/subnets/database-setup'
