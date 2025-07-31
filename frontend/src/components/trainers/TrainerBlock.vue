<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import EditIcon from "@/components/icons/EditIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import TrainerGenderIcon from "./TrainerGenderIcon.vue";
import type { Trainer } from "@/types/trainers";

const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<{
    external?: boolean | string;
    size?: number | string;
    trainer: Trainer;
  }>(),
  {
    size: 40,
  },
);

const displayName = computed<string>(() => props.trainer.displayName ?? props.trainer.uniqueName);
const isExternal = computed<boolean>(() => parseBoolean(props.external) ?? false);
const target = computed<string | undefined>(() => (isExternal.value ? "_blank" : undefined));
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <slot name="avatar">
            <RouterLink :to="{ name: 'TrainerEdit', params: { id: trainer.id } }" :target="target">
              <TarAvatar :display-name="displayName" icon="fas fa-person" :size="size" :url="trainer.sprite" />
            </RouterLink>
          </slot>
        </div>
      </div>
      <div>
        <slot name="before"></slot>
        <slot name="contents">
          <RouterLink :to="{ name: 'TrainerEdit', params: { id: trainer.id } }" :target="target">
            <ExternalIcon v-if="isExternal" />
            <EditIcon v-else />
            {{ displayName }}
            <TrainerGenderIcon v-if="!trainer.displayName" :gender="trainer.gender" />
            <template v-if="trainer.displayName">
              <br />
              <TrainerGenderIcon :gender="trainer.gender" /> {{ trainer.uniqueName }}
            </template>
          </RouterLink>
        </slot>
        <slot name="after"></slot>
      </div>
    </div>
  </div>
</template>
