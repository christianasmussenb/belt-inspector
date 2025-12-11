export type FileRecord = {
  id: string
  fileName: string
  contentType?: string
  storageKey: string
  inspectionId: string
}

export type Inspection = {
  id: string
  title: string
  description?: string
  status: string
  createdAt: string
  updatedAt: string
  files: FileRecord[]
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

async function handleJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || `Request failed with ${response.status}`)
  }
  return response.json() as Promise<T>
}

export async function fetchInspections(): Promise<Inspection[]> {
  const res = await fetch(`${API_BASE_URL}/api/inspections`)
  return handleJson<Inspection[]>(res)
}

export async function createInspection(payload: {
  title: string
  description?: string
  status?: string
}): Promise<Inspection> {
  const res = await fetch(`${API_BASE_URL}/api/inspections`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  })

  return handleJson<Inspection>(res)
}

export async function uploadFile(
  inspectionId: string,
  file: File,
): Promise<FileRecord> {
  const formData = new FormData()
  formData.append('file', file)

  const res = await fetch(`${API_BASE_URL}/api/files/${inspectionId}`, {
    method: 'POST',
    body: formData,
  })

  return handleJson<FileRecord>(res)
}
