import { useState } from "react";

const emptyForm = {
  categoryKey: "complaint",
  title: "",
  description: "",
};

export default function CreatePublication({
  onCancel,
  onSubmit,
  busy,
  error,
  initialValues = emptyForm,
  isEditing = false,
}) {
  const [form, setForm] = useState(() => ({ ...initialValues }));

  function updateField(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  async function submitForm(event) {
    event.preventDefault();
    await onSubmit({
      ...form,
      title: form.title.trim(),
      description: form.description.trim(),
    });
  }

  return (
    <div className="modal-backdrop">
      <section
        className="publication-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="publication-modal-title"
      >
        <div className="modal-heading">
          <div>
            <p className="eyebrow">Comparte con tu comunidad</p>
            <h2 id="publication-modal-title">
              {isEditing ? "Editar publicación" : "Nueva publicación"}
            </h2>
          </div>
          <button className="modal-close" type="button" onClick={onCancel} aria-label="Cerrar">
            ×
          </button>
        </div>

        {error && <p className="notice failure" role="alert">{error}</p>}

        <form className="publication-form" onSubmit={submitForm}>
          <label>
            Categoría
            <select
              name="categoryKey"
              value={form.categoryKey}
              onChange={updateField}
              required
            >
              <option value="complaint">Queja</option>
              <option value="suggestion">Sugerencia</option>
              <option value="event">Evento</option>
              <option value="notice">Aviso</option>
            </select>
          </label>
          <label>
            Título
            <input
              name="title"
              value={form.title}
              onChange={updateField}
              maxLength={120}
              placeholder="Escribe un título claro"
              required
            />
          </label>
          <label>
            Descripción
            <textarea
              name="description"
              value={form.description}
              onChange={updateField}
              maxLength={2000}
              rows={5}
              placeholder="Cuéntale a la comunidad los detalles"
              required
            />
          </label>
          <div className="modal-actions">
            <button className="secondary" type="button" onClick={onCancel} disabled={busy}>
              Cancelar
            </button>
            <button type="submit" disabled={busy}>
              {busy ? "Guardando..." : isEditing ? "Guardar cambios" : "Publicar"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
