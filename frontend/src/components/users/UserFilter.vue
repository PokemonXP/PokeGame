<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import UserAvatar from "./UserAvatar.vue";
import type { UserSummary } from "@/types/users";
import { formatUser } from "@/helpers/format";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    users?: UserSummary[];
  }>(),
  {
    id: "user",
    label: "user.label",
    placeholder: "any",
    users: () => [],
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    props.users.map((user) => ({
      text: formatUser(user),
      value: user.id,
    })),
    "text",
  ),
);
const user = computed<UserSummary | undefined>(() => props.users.find(({ id }) => id === props.modelValue));

defineEmits<{
  (e: "update:model-value", id: string): void;
}>();
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="!options.length"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="user" #append>
      <UserAvatar class="input-group-text" :user="user" />
    </template>
  </TarSelect>
</template>
