<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";

import EditIcon from "@/components/icons/EditIcon.vue";
import type { Item } from "@/types/items";

const props = defineProps<{
  item: Item;
}>();

const displayName = computed<string>(() => props.item.displayName ?? props.item.uniqueName);
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }">
            <TarAvatar :display-name="displayName" icon="fas fa-cart-shopping" :url="item.sprite" />
          </RouterLink>
        </div>
      </div>
      <div>
        <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }">
          <EditIcon /> {{ displayName }}
          <template v-if="item.displayName">
            <br />
            {{ item.uniqueName }}
          </template>
        </RouterLink>
      </div>
    </div>
  </div>
</template>
