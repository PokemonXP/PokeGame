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
        // Abilities
        {
          name: "AbilityList",
          path: "abilities",
          component: () => import("./views/abilities/AbilityList.vue"),
        },
        {
          name: "AbilityEdit",
          path: "abilities/:id",
          component: () => import("./views/abilities/AbilityEdit.vue"),
        },
        // Items
        {
          name: "ItemList",
          path: "items",
          component: () => import("./views/items/ItemList.vue"),
        },
        {
          name: "ItemEdit",
          path: "items/:id",
          component: () => import("./views/items/ItemEdit.vue"),
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
        // Regions
        {
          name: "RegionList",
          path: "regions",
          component: () => import("./views/regions/RegionList.vue"),
        },
        {
          name: "RegionEdit",
          path: "regions/:id",
          component: () => import("./views/regions/RegionEdit.vue"),
        },
        // Trainers
        {
          name: "TrainerList",
          path: "trainers",
          component: () => import("./views/trainers/TrainerList.vue"),
        },
        {
          name: "TrainerEdit",
          path: "trainers/:id",
          component: () => import("./views/trainers/TrainerEdit.vue"),
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
