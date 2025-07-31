<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/icons/EditIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import type { Item } from "@/types/items";

const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<{
    external?: boolean | string;
    item: Item;
    size?: number | string;
  }>(),
  {
    size: 40,
  },
);

const displayName = computed<string>(() => props.item.displayName ?? props.item.uniqueName);
const isExternal = computed<boolean>(() => parseBoolean(props.external) ?? false);
const target = computed<string | undefined>(() => (isExternal.value ? "_blank" : undefined));
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <slot name="avatar">
            <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }" :target="target">
              <TarAvatar :display-name="displayName" icon="fas fa-cart-shopping" :size="size" :url="item.sprite" />
            </RouterLink>
          </slot>
        </div>
      </div>
      <div>
        <slot name="before"></slot>
        <slot name="contents">
          <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }" :target="target">
            <ExternalIcon v-if="isExternal" />
            <EditIcon v-else />
            {{ displayName }}
            <template v-if="item.displayName">
              <br />
              {{ item.uniqueName }}
            </template>
          </RouterLink>
        </slot>
        <slot name="after"></slot>
      </div>
    </div>
  </div>
</template>
