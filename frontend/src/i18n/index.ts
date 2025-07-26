import { createI18n } from "vue-i18n";

import en from "./en";
import fr from "./fr";

type MessageSchema = typeof en;

export default createI18n<[MessageSchema], "en" | "fr">({
  legacy: false,
  locale: "en",
  fallbackLocale: "en",
  messages: {
    en,
    fr,
  },
  datetimeFormats: {
    en: {
      medium: {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
      },
    },
    fr: {
      medium: {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
      },
    },
  },
  numberFormats: {
    en: {
      currency: {
        style: "currency",
        currency: "CAD",
        currencyDisplay: "narrowSymbol",
        notation: "standard",
      },
      decimal: {
        style: "decimal",
        minimumFractionDigits: 2,
      },
      height: {
        style: "unit",
        unit: "meter",
        unitDisplay: "short",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
      integer: {
        style: "decimal",
        maximumFractionDigits: 0,
      },
      integer_percent: {
        style: "percent",
        maximumFractionDigits: 0,
      },
      percent: {
        style: "percent",
        maximumFractionDigits: 1,
        minimumFractionDigits: 1,
      },
      weight: {
        style: "unit",
        unit: "kilogram",
        unitDisplay: "short",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
    },
    fr: {
      currency: {
        style: "currency",
        currency: "CAD",
        currencyDisplay: "narrowSymbol",
        notation: "standard",
      },
      decimal: {
        style: "decimal",
        minimumFractionDigits: 2,
      },
      height: {
        style: "unit",
        unit: "meter",
        unitDisplay: "short",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
      integer: {
        style: "decimal",
        maximumFractionDigits: 0,
      },
      integer_percent: {
        style: "percent",
        maximumFractionDigits: 0,
      },
      percent: {
        style: "percent",
        maximumFractionDigits: 1,
        minimumFractionDigits: 1,
      },
      weight: {
        style: "unit",
        unit: "kilogram",
        unitDisplay: "short",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
    },
  },
});
