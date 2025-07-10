<script setup lang="ts">
import { TarInput, type InputStatus, type InputType } from "logitar-vue3-ui";
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
    type?: InputType;
  }>(),
  {
    id: "password",
    label: "users.signIn.password",
    required: true,
    type: "password",
  },
);

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);
const touched = ref<boolean>(false);

const status = computed<InputStatus | undefined>(() => (touched.value ? (props.modelValue?.length ? "valid" : "invalid") : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: string): void {
  touched.value = true;
  emit("update:model-value", value);
}

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarInput
    class="mb-3"
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :placeholder="placeholder || label ? t(placeholder ?? label) : undefined"
    ref="inputRef"
    :required="required"
    :status="status"
    :type="type"
    @update:model-value="onModelValueUpdate"
  />
</template>
