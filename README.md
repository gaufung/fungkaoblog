# My Blog

A deliberately small blog:

- **Frontend** — React + TypeScript (Vite), Markdown editor, Azure AD (Entra ID) sign-in.
- **Backend** — ASP.NET Core Web API, EF Core, **Azure SQL Database** storage.
- **Auth** — Anyone can read published posts. Only users with the **SuperAdmin**
  app role can create, edit, delete, or see drafts.

```
blog/
  backend/    ASP.NET Core Web API (Blog.Api)
  frontend/   Vite React TypeScript app
```

## 1. Azure setup (one time)

### a) Azure SQL Database
1. Create an Azure SQL Server + Database (e.g. `BlogDb`).
2. Put the connection string in `backend/appsettings.json` →
   `ConnectionStrings:DefaultConnection`. Two common options:
   - **Entra auth** (recommended): keep `Authentication=Active Directory Default`
     and sign in locally with `az login`.
   - **SQL auth**: `Server=...;Database=BlogDb;User ID=...;Password=...;Encrypt=True;`

### b) App registrations (Entra ID)
Create **two** app registrations (or one; two is cleaner):

**API app** (`Blog.Api`)
- Expose an API → set Application ID URI `api://<api-client-id>`.
- Add a scope `access_as_user`.
- **App roles** → create a role:
  - Display name: `Super Admin`
  - Value: `SuperAdmin`
  - Allowed member types: Users/Groups
- Enterprise applications → assign the `Super Admin` role to yourself.

**SPA app** (`Blog.Web`)
- Platform: **Single-page application**, redirect URI `http://localhost:5173`.
- API permissions → add the `access_as_user` scope from the API app.

> Tip: you can also use a single app registration for both; just point the SPA
> and API config at the same client id and assign the app role there.

## 2. Configure

**backend/appsettings.json**
```jsonc
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "<your-tenant-id>",
  "ClientId": "<api-client-id>",
  "Audience": "api://<api-client-id>"
}
```

**frontend/.env** (copy from `.env.example`)
```
VITE_AAD_CLIENT_ID=<spa-client-id>
VITE_AAD_TENANT_ID=<your-tenant-id>
VITE_API_SCOPE=api://<api-client-id>/access_as_user
VITE_API_BASE_URL=https://localhost:7xxx   # backend URL from launchSettings.json
```

## 3. Run

Backend (applies EF migrations to Azure SQL on startup):
```bash
cd backend
dotnet run
```

Frontend:
```bash
cd frontend
cp .env.example .env   # then fill in the values
npm install
npm run dev            # http://localhost:5173
```

Sign in with an account that has the **SuperAdmin** role to see the
**+ New post** / edit / delete controls. Everyone else sees published posts only.

## Notes
- Database schema is created/updated automatically via EF Core migrations
  (`backend/Migrations`) on API startup.
- Markdown is stored as-is and rendered with `@uiw/react-md-editor`.
- CORS allowed origins are configured in `backend/appsettings.json` → `Cors:AllowedOrigins`.
