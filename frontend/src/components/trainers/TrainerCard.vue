<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import TrainerGenderIcon from "./TrainerGenderIcon.vue";
import type { TrainerSheet } from "@/types/trainers";

const { n, t } = useI18n();
const { parseBoolean } = parsingUtils;

const props = defineProps<{
  clickable?: boolean | string;
  trainer: TrainerSheet;
}>();

const alt = computed<string>(() => `${props.trainer.name}'s Sprite'`);
const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  return classes;
});
</script>

<template>
  <TarCard :class="classes">
    <div class="row">
      <div class="col-xs-12 col-md-6">
        <h5 class="card-title">
          <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="24" />
          {{ t("trainers.card") }}
          <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="24" />
        </h5>
        <div class="mb-3"></div>
        <table class="table table-striped">
          <tbody>
            <tr>
              <th scope="row">{{ t("trainers.license.label") }}</th>
              <td>{{ trainer.license }}</td>
            </tr>
            <tr>
              <th scope="row">{{ t("name.label") }}</th>
              <td>{{ trainer.name }}</td>
            </tr>
            <tr>
              <th scope="row">{{ t("trainers.gender.label") }}</th>
              <td>{{ t(`trainers.gender.options.${trainer.gender}`) }} <TrainerGenderIcon :gender="trainer.gender" /></td>
            </tr>
            <tr>
              <th scope="row">{{ t("trainers.money") }}</th>
              <td>{{ n(trainer.money, "integer") }} <PokeDollarIcon height="20" /></td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="col-xs-12 col-md-6">
        <!-- TODO(fpion): placeholder when the trainer does not have a sprite -->
        <img v-if="trainer.sprite" :src="trainer.sprite" :alt="alt" class="img-fluid" />
      </div>
    </div>
  </TarCard>
</template>

<style scoped>
h5 {
  text-align: center;
}

td {
  text-align: right;
}
</style>
