# Belt Inspector – Monorepo

Proyecto **Belt Inspector** (mock productivo) con todos los componentes necesarios para desplegar en la nube:

- **API** (.NET 8 / ASP.NET Core)  
- **Web App** (React + Vite)  
- **Base de datos** PostgreSQL en **Render**  
- **Almacenamiento de archivos** en **Cloudflare R2** (bucket `belt-inspector`)

Este repo está pensado para correr en local (para desarrollo) y desplegarse en **Render** usando el mismo código y configuración basada en variables de entorno.

---

## 1. Arquitectura

### Componentes

- **API** (`/api`)
  - ASP.NET Core Web API.
  - Expone endpoints REST para inspecciones y archivos.
  - Se conecta a PostgreSQL (Render) usando `DB_CONNECTION_STRING`.
  - Sube y lee archivos desde Cloudflare R2 usando la API S3.

- **Web App** (`/web`)
  - React + Vite (TypeScript).
  - UI mínima (mock) para listar inspecciones y probar la API.
  - Usa `VITE_API_BASE_URL` para llamar a la API.

- **Base de datos**
  - PostgreSQL gestionado por **Render**.
  - La API ejecuta migraciones EF Core al arrancar.

- **File Storage**
  - Cloudflare R2.
  - Bucket: `belt-inspector`.
  - Endpoint S3:  
    `https://8d2ac30c8aa0d8419833f740e39e61c6.r2.cloudflarestorage.com`.

- **DNS (propuesto)**
  - `beltinspector.com` → Web pública (marketing / o la misma Web App).
  - `app.beltinspector.com` → Web App.
  - `api.beltinspector.com` → API.

---

## 2. Estructura del repositorio

```txt
belt-inspector/
  README.md            # este archivo
  api/
    README.md          # detalles de la API
    BeltInspector.Api.sln
    BeltInspector.Api/
      Program.cs
      appsettings.json
      Models/
      Data/
      Services/
      Controllers/
  web/
    README.md          # detalles de la Web
    package.json
    vite.config.ts
    index.html
    src/
      main.tsx
      App.tsx
      api.ts
  infra/
    README.md          # infraestructura, Render, DNS, env vars
