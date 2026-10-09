# Exponer la app con Cloudflare Tunnel

## 1. Preparar la base de datos (solo la primera vez o tras cambiar de rama)
    cd VozUniversitaria
    dotnet tool restore
    dotnet ef database update

## 2. Instalar cloudflared (una vez por contenedor)
    bash scripts/install-cloudflared.sh

## 3. Ejecutar la app (terminal 1)
    cd VozUniversitaria
    dotnet run
Debe mostrar `Now listening on: http://localhost:5032`.

## 4. Abrir el túnel (terminal 2)
    cloudflared tunnel --url http://localhost:5032
Se genera una URL tipo `https://xxxx.trycloudflare.com`. Cambia en cada ejecución.

## Notas
- Cada persona abre su propio túnel con su propia URL.
- Mientras el túnel esté abierto, cualquiera con el enlace puede entrar. Usar solo datos de prueba y cerrarlo con Ctrl+C al terminar.
- Si `cloudflared` desaparece, el contenedor fue reconstruido: repetir el paso 2.
