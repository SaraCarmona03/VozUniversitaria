import { useEffect, useState } from "react";
import Home from "./components/Home.jsx";
import Register from "./components/Register.jsx";
import { request } from "./api/client.js";
import { getCsrfToken } from "./api/auth.js";

export default function App() {
  const [session, setSession] = useState(null);
  const [csrfToken, setCsrfToken] = useState("");
  const [view, setView] = useState(
    window.location.hash === "#registro" ? "register" : "login",
  );
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [code, setCode] = useState("");
  const [verificationEmail, setVerificationEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    const syncViewWithHash = () => {
      setView(window.location.hash === "#registro" ? "register" : "login");
    };
    window.addEventListener("hashchange", syncViewWithHash);
    return () => window.removeEventListener("hashchange", syncViewWithHash);
  }, []);

  useEffect(() => {
    Promise.all([getCsrfToken(), request("/api/session")])
      .then(([token, currentSession]) => {
        setCsrfToken(token);
        setSession(currentSession);
      })
      .catch((requestError) => setError(requestError.message));
  }, []);

  async function refreshSession() {
    const [token, currentSession] = await Promise.all([
      getCsrfToken(),
      request("/api/session"),
    ]);
    setCsrfToken(token);
    setSession(currentSession);
  }

  function navigateTo(nextView) {
    setView(nextView);
    setError("");
    setMessage("");
    const hash = nextView === "register" ? "#registro" : "";
    window.history.replaceState(
      null,
      "",
      `${window.location.pathname}${window.location.search}${hash}`,
    );
  }

  async function handleRegister(form) {
    setError("");
    setMessage("");
    setBusy(true);
    try {
      const result = await request("/api/auth/register", {
        method: "POST",
        headers: { "X-CSRF-TOKEN": csrfToken },
        body: JSON.stringify(form),
      });

      setEmail(result.email);
      setVerificationEmail(result.email);
      setCode("");
      setView("login");
      window.history.replaceState(
        null,
        "",
        `${window.location.pathname}${window.location.search}`,
      );
      setMessage(result.message || "Cuenta creada. Revisa tu correo para obtener el código.");
    } finally {
      setBusy(false);
    }
  }

  async function handleLogin(event) {
    event.preventDefault();
    setBusy(true);
    setError("");
    setMessage("");
    try {
      await request("/api/auth/login", {
        method: "POST",
        headers: { "X-CSRF-TOKEN": csrfToken },
        body: JSON.stringify({ email, password }),
      });
      setSession(await request("/api/session"));
    } catch (requestError) {
      if (requestError.details?.requiresEmailVerification) {
        setVerificationEmail(requestError.details.email);
      }
      setError(requestError.message);
    } finally {
      setBusy(false);
    }
  }

  async function handleVerify(event) {
    event.preventDefault();
    setBusy(true);
    setError("");
    setMessage("");
    try {
      await request("/api/auth/verification/verify", {
        method: "POST",
        headers: { "X-CSRF-TOKEN": csrfToken },
        body: JSON.stringify({ email: verificationEmail, code }),
      });
      setCode("");
      setEmail(verificationEmail);
      setVerificationEmail("");
      setMessage("Correo verificado. Ahora puedes iniciar sesión.");
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setBusy(false);
    }
  }

  async function handleResend() {
    setBusy(true);
    setError("");
    setMessage("");
    try {
      const result = await request("/api/auth/verification/resend", {
        method: "POST",
        headers: { "X-CSRF-TOKEN": csrfToken },
        body: JSON.stringify({ email: verificationEmail }),
      });
      setMessage(result.message);
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setBusy(false);
    }
  }

  async function handleLogout() {
    setBusy(true);
    setError("");
    try {
      await request("/api/auth/logout", {
        method: "POST",
        headers: { "X-CSRF-TOKEN": csrfToken },
      });
      await refreshSession();
      setMessage("Sesión cerrada.");
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setBusy(false);
    }
  }

  if (session?.authenticated) {
    return (
      <Home
        session={session}
        onLogout={handleLogout}
        isLoggingOut={busy}
        error={error}
      />
    );
  }

  if (view === "register") {
    return (
      <Register
        csrfToken={csrfToken}
        busy={busy}
        onSubmit={handleRegister}
        onLogin={() => navigateTo("login")}
      />
    );
  }

  if (verificationEmail) {
    return (
      <main className="page">
        <section className="card">
          <p className="eyebrow">VozUniversitaria</p>
          <h1>Verifica tu correo</h1>
          <p className="intro">Ingresa el código de seis dígitos enviado a {verificationEmail}.</p>
          {message && <p className="notice success" role="status">{message}</p>}
          {error && <p className="notice failure" role="alert">{error}</p>}
          <form onSubmit={handleVerify}>
            <label>
              Código de verificación
              <input
                inputMode="numeric"
                pattern="[0-9]{6}"
                maxLength="6"
                required
                value={code}
                onChange={(event) => setCode(event.target.value)}
              />
            </label>
            <button disabled={busy || !csrfToken}>
              {busy ? "Procesando..." : "Verificar correo"}
            </button>
          </form>
          <button className="link-button" onClick={handleResend} disabled={busy || !csrfToken}>
            Reenviar código
          </button>
          <button className="text-button" onClick={() => setVerificationEmail("")}>
            Volver al inicio de sesión
          </button>
        </section>
      </main>
    );
  }

  return (
    <main className="page">
      <section className="card">
        <p className="eyebrow">VozUniversitaria · prueba piloto</p>
        <h1>Acceso a la comunidad</h1>
        <p className="intro">Inicia sesión para entrar al portal universitario.</p>
        {message && <p className="notice success" role="status">{message}</p>}
        {error && <p className="notice failure" role="alert">{error}</p>}

        <form onSubmit={handleLogin}>
          <label>
            Correo electrónico
            <input
              type="email"
              autoComplete="username"
              required
              value={email}
              onChange={(event) => setEmail(event.target.value)}
            />
          </label>
          <label>
            Contraseña
            <input
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </label>
          <button disabled={busy || !csrfToken}>
            {busy ? "Procesando..." : "Iniciar sesión"}
          </button>
        </form>
        <p className="account-links">
          ¿Aún no tienes cuenta?{" "}
          <a
            href="#registro"
            onClick={(event) => {
              event.preventDefault();
              navigateTo("register");
            }}
          >
            Crear cuenta
          </a>
        </p>
        <p className="recovery-note">La recuperación de contraseña aún no está disponible.</p>
        <p className="footnote">Registro y demás pantallas siguen disponibles en MVC.</p>
      </section>
    </main>
  );
}