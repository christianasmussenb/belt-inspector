# Belt Inspector Web

App React + Vite (TypeScript) para probar la API de Belt Inspector.

## Scripts

```bash
npm install
npm run dev      # entorno local
npm run build    # build de producción
npm run preview  # sirve la carpeta dist
```

## Variables de entorno

- `VITE_API_BASE_URL` (ej: `http://localhost:5000` en local, `https://api.beltinspector.com` en prod).

Crea un archivo `.env` basado en `.env.example`.

## Qué hace

- Lista inspecciones desde `GET /api/inspections`.
- Permite crear inspecciones vía `POST /api/inspections`.
- Permite subir archivos por inspección vía `POST /api/files/{inspectionId}` (se envía como `form-data`).
- Muestra la base URL configurada para validar rápidamente la conectividad con la API.

El UI está pensado como mock ligero para validaciones manuales en Render o local.
