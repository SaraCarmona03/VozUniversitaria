const { createApp, ref } = Vue;

const Greeting = {
  props: {
    name: {
      type: String,
      required: true,
    },
  },
  setup() {
    const count = ref(0);
    return { count };
  },
  template: `
    <div class="rounded-xl bg-white p-6 shadow-md">
      <h2 class="text-2xl font-semibold text-gray-900">Hello, {{ name }}!</h2>
      <button
        class="mt-4 rounded-lg bg-blue-600 px-4 py-2 text-white transition hover:bg-blue-700"
        @click="count++"
      >
        Clicked {{ count }} times
      </button>
    </div>
  `,
};

createApp({
  components: {
    Greeting,
  },
  data() {
    return {
      name: 'World',
    };
  },
  template: `
    <main class="min-h-screen bg-gray-50 p-8">
      <div class="mx-auto max-w-xl">
        <h1 class="mb-6 text-3xl font-bold text-gray-900">My First Frontend</h1>
        <Greeting :name="name" />
      </div>
    </main>
  `,
}).mount('#app');
