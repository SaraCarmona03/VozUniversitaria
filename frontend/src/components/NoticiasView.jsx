import { NoticiasHeader } from "./NoticiasHeader.jsx";
import { NoticiasList } from "./NoticiasList.jsx";

export default function NoticiasView({ onBack }) {
  return (
    <div style={{ padding: "20px", maxWidth: "800px", margin: "0 auto" }}>
      {onBack && (
        <button
          onClick={onBack}
          style={{ marginBottom: "20px", cursor: "pointer", padding: "8px 12px" }}
        >
          ← Volver al Sistema Principal
        </button>
      )}
      <h2>Módulo de Noticias - EC2 (Zustand)</h2>
      <hr style={{ marginBottom: "20px" }} />
      <NoticiasHeader />
      <hr style={{ margin: "20px 0" }} />
      <NoticiasList />
    </div>
  );
}