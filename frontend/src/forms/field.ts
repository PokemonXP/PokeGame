import type { RuleExecutionResult, ValidationOptions, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { computed, getCurrentInstance, inject, ref, unref, type ComponentInternalInstance } from "vue";

import type { Field, FieldActions, FieldEvents, FieldOptions } from "@/types/forms";
import validator, { isValidationFailure } from "@/validation";
import { bindFieldKey, unbindFieldKey } from "@/types/forms";

export function useField(id: string, options?: FieldOptions): Field {
  options ??= {};

  const vm: ComponentInternalInstance | null = getCurrentInstance();

  const bindField: ((id: string, options: FieldActions, initialValue?: string) => FieldEvents) | undefined = inject(bindFieldKey);
  const unbindField: ((id: string) => void) | undefined = inject(unbindFieldKey);

  const initialValue = ref<string>(options.initialValue ?? "");
  const validationResult = ref<ValidationResult>();
  const value = ref<string>(options.initialValue ?? "");

  const errors = computed<RuleExecutionResult[]>(() => (validationResult.value ? Object.values(validationResult.value.rules).filter(isValidationFailure) : []));
  const isValid = computed<boolean | undefined>(() => validationResult.value?.isValid);
  const name = computed<string>(() => options.name?.trim() || id);
  const rules = computed<ValidationRuleSet | undefined>(() => unref(options.rules));

  let events: FieldEvents | undefined;

  function emit(name: string, value: unknown): void {
    if (vm) {
      vm.emit(name, value);
    }
  }

  function change(newValue: string, skipValidation?: boolean): void {
    value.value = newValue || "";
    events?.updated(id, value.value);
    emit("update:model-value", value.value);

    if (!skipValidation) {
      validate();
    }
  }

  function focus(): void {
    if (options?.focus) {
      options.focus();
    }
  }

  function handleChange(e: Event, skipValidation?: boolean): void {
    const element = e.target as HTMLInputElement;
    if (element?.id !== id) {
      return;
    }
    change(element.value || "", skipValidation);
  }

  function reinitialize(): void {
    validationResult.value = undefined;
    initialValue.value = value.value;
    events?.reinitialized(id, initialValue.value);
  }

  function reset(): void {
    validationResult.value = undefined;
    value.value = initialValue.value;
    events?.reset(id, value.value);
    emit("update:model-value", value.value);
  }

  function validate(): ValidationResult {
    if (!rules.value) {
      return { isValid: true, rules: {}, context: {} };
    }
    const validationOptions: ValidationOptions = { placeholders: options?.placeholders };
    validationResult.value = validator.validate(name.value, value.value, rules.value, validationOptions);
    events?.validated(id, validationResult.value);
    emit("validated", validationResult.value);
    return validationResult.value;
  }

  const actions: FieldActions = { focus, reinitialize, reset, validate };
  if (bindField) {
    events = bindField(id, actions, initialValue.value);
  }

  return { errors, isValid, value, bindField, change, focus, handleChange, reinitialize, reset, unbindField, validate };
}
