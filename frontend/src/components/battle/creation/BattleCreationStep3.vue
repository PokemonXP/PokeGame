<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();

const { t } = useI18n();
</script>

<template>
  <section>
    <div>
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
      <TarButton class="float-end" disabled icon="fas fa-arrow-right" :text="t('actions.next')" />
      <!-- TODO(fpion): :disabled="!selected.size" @click="next" -->
    </div>
  </section>
</template>
