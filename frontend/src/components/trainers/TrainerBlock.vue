<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";

import EditIcon from "@/components/icons/EditIcon.vue";
import TrainerGenderIcon from "./TrainerGenderIcon.vue";
import type { Trainer } from "@/types/trainers";

const props = defineProps<{
  trainer: Trainer;
}>();

const displayName = computed<string>(() => props.trainer.displayName ?? props.trainer.uniqueName);
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <RouterLink :to="{ name: 'TrainerEdit', params: { id: trainer.id } }">
            <TarAvatar :display-name="displayName" icon="fas fa-person" :url="trainer.sprite" />
          </RouterLink>
        </div>
      </div>
      <div>
        <RouterLink :to="{ name: 'TrainerEdit', params: { id: trainer.id } }">
          <EditIcon /> {{ displayName }} <TrainerGenderIcon v-if="!trainer.displayName" :gender="trainer.gender" />
          <template v-if="trainer.displayName">
            <br />
            <TrainerGenderIcon :gender="trainer.gender" /> {{ trainer.uniqueName }}
          </template>
        </RouterLink>
      </div>
    </div>
  </div>
</template>
