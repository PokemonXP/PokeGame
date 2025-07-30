<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { InventoryItem } from "@/types/game";
import { computed } from "vue";

const { n } = useI18n();
const { parseBoolean } = parsingUtils;

const props = defineProps<{
  clickable?: boolean | string;
  item: InventoryItem;
  selected?: boolean | string;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
</script>

<template>
  <TarCard :class="classes">
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
</template>
