import { stringUtils } from "logitar-js";

import type { Form } from "@/types/pokemon";

const cmsBaseUrl: string = import.meta.env.VITE_APP_CMS_BASE_URL ?? "";
const { trimEnd } = stringUtils;

export function getFormUrl(form: Form): string {
  return `${trimEnd(cmsBaseUrl, "/")}/admin/contents/${form.id}`;
}

// TODO(fpion): remove this file
