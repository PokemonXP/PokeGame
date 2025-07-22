<script setup lang="ts">
import type { RouteLocationRaw } from "vue-router";
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Actor } from "@/types/aggregate";

const { d, t } = useI18n();

const props = defineProps<{
  actor: Actor;
  date: string;
  format: string;
}>();

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
const icon = computed<string | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return "fas fa-key";
    case "User":
      return "fas fa-user";
  }
  return "fas fa-robot";
});
const route = computed<RouteLocationRaw | undefined>(() => undefined);
const variant = computed<string | undefined>(() => (props.actor.type === "ApiKey" ? "info" : undefined));
</script>

<template>
  <span>
    {{ t(format, { date: d(date, "medium") }) }}
    <RouterLink v-if="route" :to="route" target="_blank">
      <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }}
    </RouterLink>
    <span v-else class="text-muted">
      <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }}
    </span>
  </span>
</template>
