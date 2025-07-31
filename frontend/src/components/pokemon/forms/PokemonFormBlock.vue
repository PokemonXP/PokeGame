<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/icons/EditIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import type { Form } from "@/types/pokemon-forms";

const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<{
    external?: boolean | string;
    form: Form;
    size?: number | string;
  }>(),
  {
    size: 40,
  },
);

const displayName = computed<string>(() => props.form.displayName ?? props.form.uniqueName);
const isExternal = computed<boolean>(() => parseBoolean(props.external) ?? false);
const target = computed<string | undefined>(() => (isExternal.value ? "_blank" : undefined));
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <slot name="avatar">
            <RouterLink :to="{ name: 'FormEdit', params: { id: form.id } }" :target="target">
              <TarAvatar :display-name="displayName" icon="fas fa-masks-theater" :size="size" :url="form.sprites.default" />
            </RouterLink>
          </slot>
        </div>
      </div>
      <div>
        <slot name="before"></slot>
        <slot name="contents">
          <RouterLink :to="{ name: 'FormEdit', params: { id: form.id } }" :target="target">
            <ExternalIcon v-if="isExternal" />
            <EditIcon v-else />
            {{ displayName }}
            <template v-if="form.displayName">
              <br />
              {{ form.uniqueName }}
            </template>
          </RouterLink>
        </slot>
        <slot name="after"></slot>
      </div>
    </div>
  </div>
</template>
