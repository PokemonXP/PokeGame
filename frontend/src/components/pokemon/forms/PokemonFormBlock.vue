<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";

import EditIcon from "@/components/icons/EditIcon.vue";
import type { Form } from "@/types/pokemon-forms";

const props = defineProps<{
  form: Form;
}>();

const displayName = computed<string>(() => props.form.displayName ?? props.form.uniqueName);
</script>

<template>
  <div>
    <div class="d-flex">
      <div class="d-flex">
        <div class="align-content-center flex-wrap mx-1">
          <RouterLink :to="{ name: 'FormEdit', params: { id: form.id } }">
            <TarAvatar :display-name="displayName" icon="fas fa-masks-theater" :url="form.sprites.default" />
          </RouterLink>
        </div>
      </div>
      <div>
        <RouterLink :to="{ name: 'FormEdit', params: { id: form.id } }">
          <EditIcon /> {{ displayName }}
          <template v-if="form.displayName">
            <br />
            {{ form.uniqueName }}
          </template>
        </RouterLink>
      </div>
    </div>
  </div>
</template>
