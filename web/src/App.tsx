import { type FormEvent, useEffect, useMemo, useState } from 'react'
import './App.css'
import type { FileRecord, Inspection } from './api'
import { createInspection, fetchInspections, uploadFile } from './api'

const statusOptions = ['pending', 'in-progress', 'completed']

function App() {
  const [inspections, setInspections] = useState<Inspection[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [uploading, setUploading] = useState<string | null>(null)
  const [form, setForm] = useState({ title: '', description: '', status: 'pending' })

  const apiBase = useMemo(
    () => import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
    [],
  )

  useEffect(() => {
    const load = async () => {
      try {
        const data = await fetchInspections()
        setInspections(data.map((item) => ({ ...item, files: item.files || [] })))
      } catch (err) {
        const message = err instanceof Error ? err.message : 'No se pudo cargar'
        setError(message)
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [])

  const handleCreate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError(null)

    if (!form.title.trim()) {
      setError('El título es obligatorio')
      return
    }

    try {
      const created = await createInspection({
        title: form.title.trim(),
        description: form.description.trim() || undefined,
        status: form.status,
      })
      const normalized = { ...created, files: created.files || [] }
      setInspections((prev) => [normalized, ...prev])
      setForm({ title: '', description: '', status: 'pending' })
    } catch (err) {
      const message = err instanceof Error ? err.message : 'No se pudo crear'
      setError(message)
    }
  }

  const handleUpload = async (inspectionId: string, file?: File | null) => {
    if (!file) return
    setUploading(inspectionId)
    setError(null)

    try {
      const record: FileRecord = await uploadFile(inspectionId, file)
      setInspections((prev) =>
        prev.map((inspection) =>
          inspection.id === inspectionId
            ? { ...inspection, files: [record, ...(inspection.files || [])] }
            : inspection,
        ),
      )
    } catch (err) {
      const message = err instanceof Error ? err.message : 'No se pudo subir el archivo'
      setError(message)
    } finally {
      setUploading(null)
    }
  }

  return (
    <div className="page">
      <header className="hero">
        <div>
          <p className="eyebrow">Belt Inspector</p>
          <h1>Mock de inspecciones listo para conectar a la API</h1>
          <p className="lede">
            Consulta inspecciones, crea nuevas y sube archivos de prueba contra la API
            configurada en <code>VITE_API_BASE_URL</code>.
          </p>
          <div className="pill">
            API base: <span>{apiBase}</span>
          </div>
        </div>
        <div className="badge">React + Vite</div>
      </header>

      <main className="content">
        <section className="panel">
          <div className="panel-header">
            <div>
              <p className="eyebrow">Nueva inspección</p>
              <h2>Cargar datos básicos</h2>
            </div>
            <span className="hint">POST /api/inspections</span>
          </div>

          <form className="form" onSubmit={handleCreate}>
            <label>
              <span>Título</span>
              <input
                type="text"
                value={form.title}
                onChange={(e) => setForm((prev) => ({ ...prev, title: e.target.value }))}
                placeholder="Elevador A - chequeo semanal"
                required
              />
            </label>
            <label>
              <span>Descripción</span>
              <textarea
                value={form.description}
                onChange={(e) => setForm((prev) => ({ ...prev, description: e.target.value }))}
                placeholder="Notas, hallazgos, etc."
                rows={3}
              />
            </label>
            <label>
              <span>Estado</span>
              <select
                value={form.status}
                onChange={(e) => setForm((prev) => ({ ...prev, status: e.target.value }))}
              >
                {statusOptions.map((option) => (
                  <option key={option} value={option}>
                    {option}
                  </option>
                ))}
              </select>
            </label>
            <button type="submit">Crear inspección</button>
          </form>
          {error && <p className="error">{error}</p>}
        </section>

        <section className="panel">
          <div className="panel-header">
            <div>
              <p className="eyebrow">Listado</p>
              <h2>Inspecciones</h2>
            </div>
            <span className="hint">GET /api/inspections</span>
          </div>

          {loading ? (
            <p className="muted">Cargando...</p>
          ) : inspections.length === 0 ? (
            <p className="muted">No hay inspecciones aún.</p>
          ) : (
            <div className="grid">
              {inspections.map((inspection) => (
                <article key={inspection.id} className="card">
                  <div className="card-header">
                    <h3>{inspection.title}</h3>
                    <span className={`status status-${inspection.status}`}>
                      {inspection.status}
                    </span>
                  </div>
                  <p className="description">{inspection.description || 'Sin descripción'}</p>
                  <p className="meta">
                    Creada: {new Date(inspection.createdAt).toLocaleString()}
                  </p>

                  <div className="upload">
                    <label className="upload-label">
                      <input
                        type="file"
                        onChange={(e) =>
                          handleUpload(inspection.id, e.target.files?.[0] ?? null)
                        }
                        disabled={uploading === inspection.id}
                      />
                      <span>
                        {uploading === inspection.id
                          ? 'Subiendo...'
                          : 'Subir archivo a R2'}
                      </span>
                    </label>
                  </div>

                  <div className="files">
                    <p className="eyebrow">Archivos</p>
                    {inspection.files?.length ? (
                      <ul>
                        {inspection.files.map((file) => (
                          <li key={file.id}>
                            <span className="file-name">{file.fileName}</span>
                          </li>
                        ))}
                      </ul>
                    ) : (
                      <p className="muted">Aún sin adjuntos.</p>
                    )}
                  </div>
                </article>
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  )
}

export default App
