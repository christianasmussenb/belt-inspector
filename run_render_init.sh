#!/usr/bin/env bash
set -euo pipefail

# Ejecuta render_init.sql contra el Postgres de Render usando la URL de conexión provista.
# Toma la URL de DB_CONNECTION_STRING o DATABASE_URL, con fallback al valor entregado de Render.
DEFAULT_CONN="postgresql://beltinspector_user:whtr0zMQUv62F5jvHrvnqoTiru62SZqi@dpg-d4tknavgi27c73bm7dug-a.oregon-postgres.render.com/belt_inspector"
CONN_STRING="${DB_CONNECTION_STRING:-${DATABASE_URL:-$DEFAULT_CONN}}"

psql "$CONN_STRING" -f render_init.sql
