<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import GameBagItems from "./GameBagItems.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import type { Inventory } from "@/types/game";

const { orderBy } = arrayUtils;
const { n, rt, t, tm } = useI18n();

defineProps<{
  inventory: Inventory;
}>();

type Category = {
  text: string;
  value: string;
};
const categories = computed<Category[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.category.options"))).map(([value, text]) => ({ text, value })),
    "text",
  ),
);
</script>

<template>
  <section>
    <table class="table table-striped">
      <tbody>
        <tr>
          <th scope="row">{{ t("trainers.money") }}</th>
          <td>
            <span class="float-end">{{ n(inventory.money, "integer") }} <PokeDollarIcon height="20" /></span>
          </td>
        </tr>
      </tbody>
    </table>
    <TarTabs class="justify-content-center nav-fill" id="inventory">
      <TarTab v-for="(category, index) in categories" :key="category.value" :active="index === 0" :id="category.value" :title="category.text">
        <GameBagItems :category="category.value" :items="inventory.items" />
      </TarTab>
    </TarTabs>
  </section>
</template>
