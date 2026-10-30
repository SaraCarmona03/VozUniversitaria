# Actividad 02: módulo de noticias con Zustand

## Implementación

El frontend consume `https://jsonplaceholder.typicode.com/posts?_limit=5` desde
`frontend/src/api/noticiasApi.js`. El estado compartido vive en
`frontend/src/store/useNoticiasStore.js`: conserva las noticias, indica carga y
error, y evita solicitudes simultáneas o repetidas después de una carga exitosa.
`NoticiasHeader` y `NoticiasList` leen únicamente los datos que necesitan con
selectores de Zustand.

## Comprobación y evidencias

1. Inicia el backend en `http://localhost:5032` y, desde `frontend`, ejecuta
   `npm run dev`. Abre la aplicación, entra a **Ver Módulo de Noticias (EC2)** y
   confirma que se muestran cinco títulos. Guarda una captura de la interfaz y
   de la consola del navegador sin errores.
2. En DevTools > **Network**, filtra por `posts`, recarga el módulo y captura la
   solicitud a `jsonplaceholder.typicode.com/posts?_limit=5` con respuesta 200.
   Las solicitudes `/api` de autenticación usan el proxy de Vite al backend;
   no son la solicitud externa de noticias.
3. Para documentar la prueba de fallo, explica que mutar el arreglo existente
   con `items.push(nuevaNoticia)` no crea una nueva referencia y no notifica a
   los suscriptores. En cambio, `setItems([...items, nuevaNoticia])` crea un
   nuevo arreglo y permite que Zustand notifique los cambios. No dejes la
   mutación directa en el código funcional.
4. Para comparar renderizados, agrega temporalmente `console.count` en el cuerpo
   de `NoticiasHeader` o `NoticiasList`. Compara una suscripción al store completo
   (`useNoticiasStore()`) con los selectores actuales; el primero se actualiza
   ante cambios de cualquier propiedad del store, mientras que los selectores
   limitan las actualizaciones a los datos utilizados. Retira el contador y la
   suscripción amplia después de la prueba.

## Aspecto a mejorar

Agregar una caché con expiración TTL al store permitiría reutilizar las noticias
durante un intervalo definido y volver a consultarlas cuando la caché caduque.
La implementación actual evita volver a consultar después de una carga exitosa,
pero no expira esos datos automáticamente.

## Control de versiones

Rama de la actividad: `feature/modulo-noticias-zustand`. El commit inicial de la
actividad es `b7ff7c0`. Registra los cambios posteriores en un nuevo commit al
terminar la verificación y adjunta las capturas requeridas en la plataforma de
entrega.
