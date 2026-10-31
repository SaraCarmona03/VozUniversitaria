export default function ConfirmPublicationAction({ action, busy, error, onCancel, onConfirm }) {
  const isDelete = action === "delete";

  return (
    <div className="modal-backdrop">
      <section
        className="publication-modal confirmation-modal"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="publication-confirm-title"
        aria-describedby="publication-confirm-message"
      >
        <p className="eyebrow">Confirma esta acción</p>
        <h2 id="publication-confirm-title">
          {isDelete ? "¿Eliminar publicación?" : "¿Editar publicación?"}
        </h2>
        <p id="publication-confirm-message">
          {isDelete
            ? "Esta publicación se eliminará permanentemente. ¿Estás seguro?"
            : "Se abrirá el formulario para modificar esta publicación. ¿Deseas continuar?"}
        </p>
        {error && <p className="notice failure" role="alert">{error}</p>}
        <div className="modal-actions">
          <button className="secondary" type="button" onClick={onCancel} disabled={busy}>
            No, cancelar
          </button>
          <button
            className={isDelete ? "danger-button" : ""}
            type="button"
            onClick={onConfirm}
            disabled={busy}
          >
            {busy ? "Procesando..." : isDelete ? "Sí, eliminar" : "Sí, editar"}
          </button>
        </div>
      </section>
    </div>
  );
}
