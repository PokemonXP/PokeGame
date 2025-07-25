<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { InventoryItem } from "@/types/game";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  category?: string;
  items: InventoryItem[];
}>();

const selected = ref<number>();

const filteredItems = computed<InventoryItem[]>(() =>
  orderBy(
    props.items.filter(({ category }) => !props.category || category === props.category),
    "name",
  ),
);

function select(index: number): void {
  if (selected.value === index) {
    selected.value = undefined;
  } else {
    selected.value = index;
  }
}
</script>

<template>
  <div>
    <div v-if="filteredItems.length">
      <TarCard
        v-for="(item, index) in filteredItems"
        :key="index"
        :class="{ clickable: true, 'mb-1': true, selected: selected === index }"
        @click="select(index)"
      >
        <div class="d-flex justify-content-between align-items-center">
          <div class="d-flex">
            <div class="d-flex">
              <div class="align-content-center flex-wrap mx-1">
                <TarAvatar :display-name="item.name" size="40" icon="fas fa-cart-shopping" :url="item.sprite" />
              </div>
            </div>
            <div>
              <strong>{{ item.name }}</strong>
              <template v-if="item.description">
                <br />
                {{ item.description }}
              </template>
            </div>
          </div>
          <strong class="text-end">×{{ n(item.quantity) }}</strong>
        </div>
      </TarCard>
    </div>
    <p v-else>{{ t("items.empty") }}</p>
  </div>
</template>
