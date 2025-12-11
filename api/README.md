# Belt Inspector API

API en ASP.NET Core 8 (Web API) para gestionar inspecciones y archivos en Cloudflare R2.

## Requisitos

- .NET 8 SDK
- PostgreSQL accesible
- Claves S3 para R2

## Variables de entorno

- `DB_CONNECTION_STRING` (ej: `Host=localhost;Port=5432;Database=beltinspector;Username=postgres;Password=postgres`)
- `R2_ENDPOINT` (ej: `https://8d2ac30c8aa0d8419833f740e39e61c6.r2.cloudflarestorage.com`)
- `R2_BUCKET` (ej: `belt-inspector`)
- `R2_ACCESS_KEY`
- `R2_SECRET_KEY`

Alternativamente, se pueden definir en `appsettings.json` bajo las secciones `ConnectionStrings` y `R2`.

## Ejecutar en local

```bash
cd api/BeltInspector.Api
export DB_CONNECTION_STRING="Host=localhost;Port=5432;Database=beltinspector;Username=postgres;Password=postgres"
dotnet restore
dotnet ef database update   # si se agregan migraciones
dotnet run
```

La API quedará en `https://localhost:5001` o `http://localhost:5000`. Endpoint de salud: `/api/health`.

## Endpoints principales

- `GET /api/health` – ping de salud.
- `GET /api/inspections` – lista de inspecciones.
- `GET /api/inspections/{id}` – detalle con archivos.
- `POST /api/inspections` – crear inspección (`{ title, description?, status? }`).
- `PUT /api/inspections/{id}` – actualizar título/descripcion/estado.
- `POST /api/files/{inspectionId}` – subir archivo (form-data `file`).
- `GET /api/files/{id}/download` – descargar archivo.

Swagger se activa en Development.
