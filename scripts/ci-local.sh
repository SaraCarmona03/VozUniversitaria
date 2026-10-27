#!/usr/bin/env bash
# Ejecuta las comprobaciones locales de frontend, backend y pruebas.
# El modo estricto hace que cualquier comando fallido detenga el script.
set -Eeuo pipefail

# Calcula la raíz del repositorio a partir de la ubicación de este script,
# para que sus rutas funcionen aunque se invoque desde otra carpeta.
ROOT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
FRONTEND_DIR="$ROOT_DIR/frontend"
TEST_PROJECT="$ROOT_DIR/VozUniversitaria.Tests/VozUniversitaria.Tests.csproj"

# Imprime errores con un prefijo reconocible y termina con código distinto de cero.
fail() {
  printf 'CI local: ERROR: %s\n' "$*" >&2
  exit 1
}

# Verifica los programas que necesita el pipeline antes de empezar.
for command_name in dotnet node npm; do
  command -v "$command_name" >/dev/null 2>&1 || fail "No se encontró '$command_name'. Instala los requisitos del proyecto y vuelve a intentarlo."
done

# Muestra las versiones para facilitar el diagnóstico de diferencias de entorno.
printf 'CI local: Node %s | npm %s | .NET %s\n' \
  "$(node --version)" \
  "$(npm --version)" \
  "$(dotnet --version)"

# Instala exactamente las dependencias registradas en frontend/package-lock.json.
printf '\n[1/4] Instalando dependencias exactas de React...\n'
npm --prefix "$FRONTEND_DIR" ci --no-audit --no-fund

# Compila React para que ASP.NET Core lo sirva desde wwwroot/app.
printf '\n[2/4] Compilando frontend React...\n'
npm --prefix "$FRONTEND_DIR" run build
# Falla explícitamente si el build no dejó el punto de entrada esperado.
[[ -f "$ROOT_DIR/VozUniversitaria/wwwroot/app/index.html" ]] \
  || fail "React no generó VozUniversitaria/wwwroot/app/index.html."

# Construye backend y pruebas; SkipReactFrontendBuild evita repetir el build de React,
# que ya se ejecutó en el paso anterior.
printf '\n[3/4] Compilando backend y proyecto de pruebas...\n'
dotnet build "$TEST_PROJECT" -p:SkipReactFrontendBuild=true

# Ejecuta la suite ya compilada, sin restaurar ni volver a construir proyectos.
printf '\n[4/4] Ejecutando pruebas automatizadas...\n'
dotnet test "$TEST_PROJECT" --no-build --no-restore

# Solo se muestra si todos los pasos anteriores terminaron correctamente.
printf '\nCI local: SUCCESS — frontend, backend y pruebas finalizaron correctamente.\n'
