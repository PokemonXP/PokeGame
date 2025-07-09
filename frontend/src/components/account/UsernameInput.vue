<script setup lang="ts">
import { TarInput, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "username",
    label: "users.signIn.username",
    required: true,
  },
);

const touched = ref<boolean>(false);

const status = computed<InputStatus | undefined>(() => (touched.value ? (props.modelValue?.length ? "valid" : "invalid") : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: string): void {
  touched.value = true;
  emit("update:model-value", value);
}
</script>

<template>
  <TarInput
    class="mb-3"
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :placeholder="placeholder || label ? t(placeholder ?? label) : undefined"
    :required="required"
    :status="status"
    @update:model-value="onModelValueUpdate"
  />
</template>
