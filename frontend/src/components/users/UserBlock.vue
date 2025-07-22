<script setup lang="ts">
import type { UserSummary } from "@/types/users";
import UserAvatar from "./UserAvatar.vue";

defineProps<{
  user: UserSummary;
}>();
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <UserAvatar :user="user" />
        </div>
      </div>
      <div>
        {{ user.fullName ?? user.uniqueName }}
        <template v-if="user.fullName">
          <br />
          {{ user.uniqueName }}
        </template>
      </div>
    </div>
  </div>
</template>
