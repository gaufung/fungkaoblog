targetScope = 'resourceGroup'

param postgresServerName string
param databaseName string
param sharedVnetName string
param privateDnsZoneName string
param blogVnetId string

resource server 'Microsoft.DBforPostgreSQL/flexibleServers@2024-08-01' existing = {
  name: postgresServerName
}

resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2024-08-01' = {
  parent: server
  name: databaseName
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

resource sharedVnet 'Microsoft.Network/virtualNetworks@2024-05-01' existing = {
  name: sharedVnetName
}

resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2024-05-01' = {
  parent: sharedVnet
  name: 'to-fungkaoblog'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: blogVnetId
    }
  }
}

resource dns 'Microsoft.Network/privateDnsZones@2024-06-01' existing = {
  name: privateDnsZoneName
}

resource link 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
  parent: dns
  name: 'fungkaoblog-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: blogVnetId
    }
  }
}
