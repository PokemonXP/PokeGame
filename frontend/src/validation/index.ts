import { Validator, rules, type RuleExecutionResult } from "logitar-validation";

const validator = new Validator({ treatWarningsAsErrors: true });

validator.setRule("allowedCharacters", rules.allowedCharacters);
validator.setRule("maximumLength", rules.maximumLength);
validator.setRule("maximumValue", rules.maximumValue);
validator.setRule("minimumLength", rules.minimumLength);
validator.setRule("minimumValue", rules.minimumValue);
validator.setRule("pattern", rules.pattern);
validator.setRule("required", rules.required);
validator.setRule("url", rules.url);

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
