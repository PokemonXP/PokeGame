import { Validator, rules, type RuleExecutionResult } from "logitar-validation";

const validator = new Validator({ treatWarningsAsErrors: true });

validator.setRule("required", rules.required);

export default validator;

export function isValidationFailure(result: RuleExecutionResult): boolean {
  switch (result.severity) {
    case "warning":
    case "error":
    case "critical":
      return true;
  }
  return false;
}
