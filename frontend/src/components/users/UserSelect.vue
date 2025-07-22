<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import UserAvatar from "./UserAvatar.vue";
import type { SearchResults } from "@/types/search";
import type { UserSummary } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { searchUsers } from "@/api/users";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "user",
    label: "user.label",
    placeholder: "user.placeholder",
  },
);

const users = ref<UserSummary[]>([]);

const user = computed<UserSummary | undefined>(() => users.value.find(({ id }) => id === props.modelValue));

const options = computed<SelectOption[]>(() =>
  orderBy(
    users.value.map((user) => ({
      text: formatUser(user),
      value: user.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", user: UserSummary | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedUser: UserSummary | undefined = users.value.find((user) => user.id === id);
  emit("selected", selectedUser);
}

onMounted(async () => {
  try {
    const results: SearchResults<UserSummary> = await searchUsers();
    users.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  >
    <template v-if="user" #append>
      <UserAvatar class="input-group-text" :user="user" />
    </template>
  </FormSelect>
</template>
