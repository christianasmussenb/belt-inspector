
---

### `infra/README.md`

```md
# Infraestructura – Belt Inspector

Este documento resume la configuración de infraestructura para desplegar Belt Inspector en producción usando:

- **Render** (PostgreSQL + Web Services)
- **Cloudflare R2** (almacenamiento de archivos)
- **DNS** en `beltinspector.com`

---

## 1. Componentes en producción

### 1.1. Render

1. **PostgreSQL**
   - Servicio: `beltinspector-db`.
   - Usar la **External Connection String** como `DB_CONNECTION_STRING`.

2. **API Web Service**
   - Nombre sugerido: `beltinspector-api`.
   - Root: `api/BeltInspector.Api`.
   - Build:

     ```bash
     dotnet restore
     dotnet publish -c Release -o out
     ```

   - Start:

     ```bash
     dotnet out/BeltInspector.Api.dll
     ```

   - Variables de entorno:
     - `DB_CONNECTION_STRING = postgres://...` (desde Postgres de Render)
     - `R2_ENDPOINT = https://8d2ac30c8aa0d8419833f740e39e61c6.r2.cloudflarestorage.com`
     - `R2_BUCKET = belt-inspector`
     - `R2_ACCESS_KEY = <Access Key ID>`
     - `R2_SECRET_KEY = <Secret Access Key>`

   - Dominio:
     - `api.beltinspector.com`

3. **Web Static Site**
   - Nombre sugerido: `beltinspector-web`.
   - Root: `web`.
   - Build:

     ```bash
     npm install
     npm run build
     ```

   - Publish directory: `dist`.
   - Env vars:
     - `VITE_API_BASE_URL = https://api.beltinspector.com`
   - Dominio:
     - `app.beltinspector.com` o `beltinspector.com`.

---

## 2. Cloudflare R2

- **Account ID**: `8d2ac30c8aa0d8419833f740e39e61c6`
- **Bucket**: `belt-inspector`
- **Endpoint S3**:  
  `https://8d2ac30c8aa0d8419833f740e39e61c6.r2.cloudflarestorage.com`

### 2.1. Access Keys

- Crear un par `Access Key ID` / `Secret Access Key` con permisos para R2.
- Usar estos valores en:

  - `R2_ACCESS_KEY`
  - `R2_SECRET_KEY`

> Si alguna vez las claves se exponen, rotarlas inmediatamente en Cloudflare.

---

## 3. DNS (beltinspector.com)

En el proveedor DNS del dominio `beltinspector.com`:

- `api.beltinspector.com` → CNAME al host del servicio `beltinspector-api` en Render.
- `app.beltinspector.com` → CNAME al host del servicio `beltinspector-web` en Render.
- Opcional:
  - `beltinspector.com` → CNAME al mismo host que `app.beltinspector.com`.

Render se encarga de emitir certificados SSL para esos dominios (Let’s Encrypt).

---

## 4. Flujo de despliegue (resumen)

1. Subir monorepo `belt-inspector` a GitHub.
2. Crear **PostgreSQL** en Render.
3. Crear **Web Service API** en Render:
   - Conectar a repo.
   - Configurar build/start.
   - Definir variables de entorno (DB + R2).
   - Añadir dominio `api.beltinspector.com`.
4. Crear **Static Site Web** en Render:
   - Conectar a repo.
   - Configurar build.
   - Definir `VITE_API_BASE_URL`.
   - Añadir dominio `app.beltinspector.com`.
5. Configurar DNS en el proveedor del dominio.
6. Probar:
   - `https://api.beltinspector.com/api/health`
   - `https://app.beltinspector.com` (o `https://beltinspector.com`).

---

## 5. Checklist

- [ ] Repo `belt-inspector` en GitHub.
- [ ] Render PostgreSQL creado y accesible.
- [ ] API desplegada en Render, respondiendo `/api/health`.
- [ ] Cloudflare R2 con bucket `belt-inspector` y claves configuradas.
- [ ] Web desplegada en Render, conectada a la API por `VITE_API_BASE_URL`.
- [ ] DNS propagado para `api.beltinspector.com` y `app.beltinspector.com`.
- [ ] Pruebas end-to-end desde Web y (más adelante) desde App Android.

