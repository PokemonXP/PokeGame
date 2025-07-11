import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      component: () => import("./views/account/ProfileView.vue"),
    },
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // Admin
    {
      path: "/admin",
      children: [
        {
          name: "Admin",
          path: "",
          component: () => import("./views/AdminView.vue"),
        },
        // Pokémon
        {
          name: "PokemonList",
          path: "pokemon",
          component: () => import("./views/pokemon/PokemonList.vue"),
        },
        {
          name: "CreatePokemon",
          path: "pokemon/create",
          component: () => import("./views/pokemon/CreatePokemon.vue"),
        },
        {
          name: "PokemonEdit",
          path: "pokemon/:id",
          component: () => import("./views/pokemon/PokemonEdit.vue"),
        },
      ],
      meta: { isAdmin: true },
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
  if (to.meta.isAdmin && !account.currentUser?.isAdmin) {
    return { name: "Home" };
  }
});

export default router;
