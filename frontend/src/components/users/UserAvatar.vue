<script setup lang="ts">
import type { UserSummary } from "@/types/users";
import { TarAvatar } from "logitar-vue3-ui";

defineProps<{
  user: UserSummary;
}>();
</script>

<template>
  <TarAvatar :display-name="user.fullName ?? user.uniqueName" :email-address="user.emailAddress" :url="user.pictureUrl" />
</template>
