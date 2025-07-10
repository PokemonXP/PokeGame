import type { ValidationResult } from "logitar-validation";
import { computed, provide, ref } from "vue";

import type { FieldActions, FieldEvents, FieldValues, Form } from "@/types/forms";
import { bindFieldKey, unbindFieldKey } from "@/types/forms";

export function useForm(): Form {
  const fields = ref<Map<string, FieldActions>>(new Map());
  const isSubmitting = ref<boolean>(false);
  const validationResults = ref<Map<string, ValidationResult>>(new Map());
  const values = ref<Map<string, FieldValues>>(new Map());

  const hasChanges = computed<boolean>(() => Object.values([...values.value.values()]).some(({ hasChanged }) => hasChanged));
  const isValid = computed<boolean>(() => Object.values([...validationResults.value.values()]).every((result) => result.isValid));

  function onFieldReinitialize(id: string, value: string): void {
    validationResults.value.delete(id);
    values.value.set(id, { initial: value, current: value, hasChanged: false } as FieldValues);
  }
  function onFieldReset(id: string, value: string): void {
    validationResults.value.delete(id);
    values.value.set(id, { initial: value, current: value, hasChanged: false } as FieldValues);
  }
  function onFieldUpdate(id: string, value: string): void {
    const fieldValues: FieldValues = {
      initial: values.value.get(id)?.initial ?? "",
      current: value,
      hasChanged: false,
    };
    fieldValues.hasChanged = fieldValues.initial !== fieldValues.current;
    values.value.set(id, fieldValues);
  }
  function onFieldValidation(id: string, result: ValidationResult): void {
    validationResults.value.set(id, result);
  }
  const fieldEvents: FieldEvents = {
    reinitialized: onFieldReinitialize,
    reset: onFieldReset,
    updated: onFieldUpdate,
    validated: onFieldValidation,
  };
  function bindField(id: string, actions: FieldActions, initialValue?: string): FieldEvents {
    fields.value.set(id, actions);
    validationResults.value.delete(id);
    values.value.set(id, {
      initial: initialValue ?? "",
      current: initialValue ?? "",
      hasChanged: false,
    } as FieldValues);
    return fieldEvents;
  }
  provide(bindFieldKey, bindField);

  function unbindField(id: string): void {
    fields.value.delete(id);
    validationResults.value.delete(id);
    values.value.delete(id);
  }
  provide(unbindFieldKey, unbindField);

  function handleSubmit(submitCallback?: () => void): void {
    if (!isSubmitting.value) {
      try {
        isSubmitting.value = true;
        validate();
        if (isValid.value) {
          if (submitCallback) {
            submitCallback();
          }
          reinitialize();
        } else {
          const ids: string[] = [...validationResults.value.entries()].filter(([, value]) => !value.isValid).map(([id]) => id);
          if (ids.length) {
            const field: FieldActions | undefined = fields.value.get(ids[0]);
            field?.focus();
          }
        }
      } finally {
        isSubmitting.value = false;
      }
    }
  }

  function reinitialize(): void {
    Object.values([...fields.value.values()]).forEach((field) => field.reinitialize());
  }

  function reset(): void {
    Object.values([...fields.value.values()]).forEach((field) => field.reset());
  }

  function validate(): Map<string, ValidationResult> {
    [...fields.value.entries()].forEach(([id, field]) => {
      const result: ValidationResult = field.validate();
      validationResults.value.set(id, result);
    });
    return validationResults.value;
  }

  return { hasChanges, isSubmitting, isValid, handleSubmit, reinitialize, reset, validate };
}
