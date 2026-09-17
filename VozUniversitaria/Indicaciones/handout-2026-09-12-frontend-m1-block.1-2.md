# Student Handout — Your First Frontend (M1, Session 2)

**Date:** Sep 12, 2026 · **Duration:** 2 hours · **Work in pairs** (driver types, navigator reads this doc aloud — **swap every 20 min**)
**Your framework:** React · Vue · or Angular — the **concepts are identical**, only the syntax changes. This handout shows the idea first, then the three syntaxes.
**Rules:** work inside the **dev container**, on a **branch** (`git switch -c feat/m1-frontend`), commit after every block that works.

> **The one sentence to remember all class:**
> _A component turns **data** into **markup**, and the framework **re-runs it for you** whenever its data changes._

---

## The 5 ideas every frontend framework shares

| #   | Shared idea        | What it means                                                   | React                         | Vue (Composition)              | Angular            |
| --- | ------------------ | --------------------------------------------------------------- | ----------------------------- | ------------------------------ | ------------------ |
| 1   | **Component**      | A named, reusable unit that renders markup                      | Function returning JSX        | `.vue` file (`<script setup>`) | `@Component` class |
| 2   | **Inputs**         | Data passed _down_ from the parent; read-only in the child      | `props`                       | `defineProps()`                | `@Input()`         |
| 3   | **Reactive state** | Data the component owns; when it changes, the UI updates itself | `useState`                    | `ref()`                        | `signal()`         |
| 4   | **Events**         | User actions (click, type) call _your_ function                 | `onClick={...}`               | `@click="..."`                 | `(click)="..."`    |
| 5   | **Mounting**       | One root component attached to an element in `index.html`       | `createRoot().render(<App/>)` | `createApp(App).mount('#app')` | `<app-root>`       |

Keep this table open all class — it's your **Rosetta Stone**.

---

## Block 1 (0:10–0:25) — Your first component: `Greeting`

**Goal:** build a component with an **input**, **state**, and an **event** — and see it live in the browser.

Requirements (same for every framework):

1. Takes one input: `name` (string), passed from the parent.
2. Shows `Hello, <name>!`
3. Owns state: `count`, starts at 0.
4. A `<button>` increments `count` → displays `Clicked N times`.
5. Visible at your dev server: React/Vue → `http://localhost:5173`, Angular → `http://localhost:4200`.

### React — `src/Greeting.tsx`

```tsx
import { useState } from "react";

export function Greeting({ name }: { name: string }) {
  const [count, setCount] = useState(0);
  return (
    <div>
      <h2>Hello, {name}!</h2>
      <button onClick={() => setCount(count + 1)}>Clicked {count} times</button>
    </div>
  );
}
```

Use it in `App.tsx`: `<Greeting name="World" />`

### Vue — `src/Greeting.vue`

```vue
<script setup>
import { ref } from "vue";
defineProps({ name: String });
const count = ref(0);
</script>

<template>
  <div>
    <h2>Hello, {{ name }}!</h2>
    <button @click="count++">Clicked {{ count }} times</button>
  </div>
</template>
```

Use it in `App.vue`: `<Greeting name="World" />`

### Angular — `src/app/greeting/greeting.ts`

```ts
import { Component, Input, signal } from "@angular/core";

@Component({
  selector: "app-greeting",
  template: `
    <h2>Hello, {{ name }}!</h2>
    <button (click)="count.set(count() + 1)">
      Clicked {{ count() }} times
    </button>
  `,
})
export class Greeting {
  @Input() name = "";
  count = signal(0);
}
```

Use it in the root template: `<app-greeting name="World" />`

### ✅ Check

- [ ] Greeting shows with the passed name
- [ ] Clicking the button updates the number **without a page reload**
- [ ] Change the text `"World"` in the parent, save → browser updates by itself (HMR)
- [ ] **Debrief with your partner:** point to the input, the state, the event handler. Now point to where you updated the DOM by hand — _there is no such place._

**Commit:** `feat: first Greeting component`

---

## Block 2 (0:25–0:35) — Add Tailwind CSS

**Concept:** Tailwind replaces hand-written CSS with **tiny single-purpose utility classes** in your markup: `p-4` = padding, `rounded-lg` = rounded corners, `text-sm` = small text. Responsive design is a **prefix**: `md:grid-cols-2` = "from the md breakpoint up, 2 columns". No prefix = mobile. That's **mobile-first**.

### Steps (Vite-based: React & Vue)

```bash
cd frontend
npm install tailwindcss @tailwindcss/vite
```

Register the plugin in `vite.config.ts`:

```ts
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react"; // Vue: import vue from "@vitejs/plugin-vue"
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
  plugins: [react(), tailwindcss()], // Vue: plugins: [vue(), tailwindcss()]
});
```

Replace `src/index.css` (or `src/style.css`) with:

```css
@import "tailwindcss";
```

### Steps (Angular)

```bash
npm install tailwindcss
npx tailwindcss init
```

Add to `angular.json` → `build.options.styles`: `"src/styles.css"`, and in `src/styles.css`:

```css
@import "tailwindcss";
```

### ✅ Check — style your Greeting

Wrap it in utilities and confirm the styles render:

```html
<main class="min-h-screen bg-gray-50 p-8">
  <h1 class="text-2xl font-bold text-gray-900">My First Frontend</h1>
  <!-- your Greeting component here -->
</main>
```

- [ ] Background is light gray, heading is large and bold
- [ ] Nothing styled? → plugin not registered or CSS import missing

**Commit:** `feat: add Tailwind CSS`

---
