# My Blog

A deliberately small, read-only blog:

- **Frontend** — React + TypeScript (Vite), Markdown rendering.
- **Backend** — ASP.NET Core Web API, EF Core, **Azure SQL Database** storage.
- Anyone can read published posts. There is no sign-in or authoring UI —
  posts are managed directly in the database (e.g. via EF Core seed data
  in `backend/Migrations`).

```
blog/
  backend/    ASP.NET Core Web API (Blog.Api)
  frontend/   Vite React TypeScript app
```

## 1. Azure setup (one time)

### Azure SQL Database
1. Create an Azure SQL Server + Database (e.g. `BlogDb`).
2. Put the connection string in `backend/appsettings.json` →
   `ConnectionStrings:DefaultConnection`. Two common options:
   - **Entra auth** (recommended): keep `Authentication=Active Directory Default`
     and sign in locally with `az login`.
   - **SQL auth**: `Server=...;Database=BlogDb;User ID=...;******;Encrypt=True;`

## 2. Configure

**frontend/.env** (copy from `.env.example`)
```
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
cp .env.example .env   # then fill in the value
npm install
npm run dev            # http://localhost:5173
```

## Notes
- Database schema is created/updated automatically via EF Core migrations
  (`backend/Migrations`) on API startup.
- Only posts with `Published = true` are returned by the API.
- Markdown is stored as-is and rendered with `@uiw/react-md-editor`.
- CORS allowed origins are configured in `backend/appsettings.json` → `Cors:AllowedOrigins`.
