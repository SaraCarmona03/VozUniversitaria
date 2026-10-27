import { useState } from "react";

const initialForm = {
  name: "",
  lastName: "",
  caseNumber: "",
  email: "",
  password: "",
  confirmPassword: "",
};

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

function validateForm(form) {
  const errors = {};
  for (const field of Object.keys(initialForm)) {
    if (!form[field].trim()) {
      errors[field] = "Este campo es obligatorio.";
    }
  }

  if (form.email.trim() && !emailPattern.test(form.email.trim())) {
    errors.email = "Ingresa un correo electrónico válido.";
  }

  if (form.password && !passwordPattern.test(form.password)) {
    errors.password = "Usa al menos 8 caracteres, una mayúscula, una minúscula y un número.";
  }

  if (form.confirmPassword && form.password !== form.confirmPassword) {
    errors.confirmPassword = "Las contraseñas no coinciden.";
  }

  return errors;
}

function fieldErrorsFor(errors, field) {
  const matchingKey = Object.keys(errors).find(
    (key) => key.toLowerCase() === field.toLowerCase(),
  );
  if (!matchingKey) {
    return [];
  }

  const messages = errors[matchingKey];
  return Array.isArray(messages) ? messages : [messages];
}

function SpeakerIcon() {
  return (
    <svg
      aria-hidden="true"
      width="28"
      height="28"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M3 10v4a1 1 0 0 0 1 1h3l5 4V5l-5 4H4a1 1 0 0 0-1 1Z" />
      <path d="M16 9a5 5 0 0 1 0 6" />
      <path d="M19 6a9 9 0 0 1 0 12" />
    </svg>
  );
}

export default function Register({ csrfToken, busy, onSubmit, onLogin }) {
  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState({});
  const [notice, setNotice] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  function updateField(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
    setErrors((current) => {
      const next = { ...current };
      delete next[name];
      return next;
    });
    setNotice("");
  }

  async function submitForm(event) {
    event.preventDefault();
    setNotice("");
    const validationErrors = validateForm(form);
    setErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) {
      return;
    }

    try {
      await onSubmit({
        ...form,
        email: form.email.trim(),
      });
    } catch (requestError) {
      setNotice(requestError.details?.error || requestError.message);
      setErrors(requestError.details?.errors || {});
    }
  }

  function renderFieldError(field) {
    const messages = fieldErrorsFor(errors, field);
    if (!messages.length) {
      return null;
    }

    return (
      <span className="field-error" id={`${field}-error`}>
        {messages[0]}
      </span>
    );
  }

  return (
    <main className="page register-page">
      <section className="register-card">
        <header className="register-header">
          <span className="register-icon"><SpeakerIcon /></span>
          <h1>Your voice counts</h1>
          <p>Create your account to participate.</p>
        </header>

        <div className="register-body">
          {notice && <p className="notice failure" role="alert">{notice}</p>}

          <form className="register-form" onSubmit={submitForm} noValidate>
            <div className="register-name-row">
              <label>
                Name
                <input
                  autoComplete="given-name"
                  name="name"
                  required
                  value={form.name}
                  onChange={updateField}
                  aria-invalid={Boolean(fieldErrorsFor(errors, "name").length)}
                  aria-describedby={fieldErrorsFor(errors, "name").length ? "name-error" : undefined}
                />
                {renderFieldError("name")}
              </label>
              <label>
                Last Name
                <input
                  autoComplete="family-name"
                  name="lastName"
                  required
                  value={form.lastName}
                  onChange={updateField}
                  aria-invalid={Boolean(fieldErrorsFor(errors, "lastName").length)}
                  aria-describedby={fieldErrorsFor(errors, "lastName").length ? "lastName-error" : undefined}
                />
                {renderFieldError("lastName")}
              </label>
            </div>

            <label>
              File number
              <input
                name="caseNumber"
                required
                value={form.caseNumber}
                onChange={updateField}
                aria-invalid={Boolean(fieldErrorsFor(errors, "caseNumber").length)}
                aria-describedby={fieldErrorsFor(errors, "caseNumber").length ? "caseNumber-error" : undefined}
              />
              {renderFieldError("caseNumber")}
            </label>

            <label>
              Email
              <input
                type="email"
                autoComplete="email"
                name="email"
                required
                value={form.email}
                onChange={updateField}
                aria-invalid={Boolean(fieldErrorsFor(errors, "email").length)}
                aria-describedby={fieldErrorsFor(errors, "email").length ? "email-error" : undefined}
              />
              {renderFieldError("email")}
            </label>

            <label>
              Password
              <span className="password-input-wrap">
                <input
                  type={showPassword ? "text" : "password"}
                  autoComplete="new-password"
                  name="password"
                  required
                  value={form.password}
                  onChange={updateField}
                  aria-invalid={Boolean(fieldErrorsFor(errors, "password").length)}
                  aria-describedby={fieldErrorsFor(errors, "password").length ? "password-error password-help" : "password-help"}
                />
                <button
                  className="password-visibility"
                  type="button"
                  onClick={() => setShowPassword((shown) => !shown)}
                  aria-label={showPassword ? "Hide password" : "Show password"}
                >
                  {showPassword ? "Hide" : "Show"}
                </button>
              </span>
              <span className="password-help" id="password-help">
                Minimum 8 characters, with uppercase, lowercase and number.
              </span>
              {renderFieldError("password")}
            </label>

            <label>
              Confirm Password
              <span className="password-input-wrap">
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  autoComplete="new-password"
                  name="confirmPassword"
                  required
                  value={form.confirmPassword}
                  onChange={updateField}
                  aria-invalid={Boolean(fieldErrorsFor(errors, "confirmPassword").length)}
                  aria-describedby={fieldErrorsFor(errors, "confirmPassword").length ? "confirmPassword-error" : undefined}
                />
                <button
                  className="password-visibility"
                  type="button"
                  onClick={() => setShowConfirmPassword((shown) => !shown)}
                  aria-label={showConfirmPassword ? "Hide confirmation password" : "Show confirmation password"}
                >
                  {showConfirmPassword ? "Hide" : "Show"}
                </button>
              </span>
              {renderFieldError("confirmPassword")}
            </label>

            <button className="create-account-button" type="submit" disabled={busy || !csrfToken}>
              {busy ? "Creating account..." : "Create Account"}
            </button>
          </form>

          <p className="register-login-link">
            ¿Ya tienes cuenta?{" "}
            <a
              href="/app/index.html"
              onClick={(event) => {
                event.preventDefault();
                onLogin();
              }}
            >
              Inicia sesión
            </a>
          </p>
        </div>
      </section>
    </main>
  );
}
