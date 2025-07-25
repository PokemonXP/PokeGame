<script setup lang="ts">
import { ref, watch } from "vue";

import CatchMultiplierInput from "./CatchMultiplierInput.vue";
import FriendshipInput from "@/components/pokemon/FriendshipInput.vue";
import FriendshipMultiplierInput from "./FriendshipMultiplierInput.vue";
import HealCheckbox from "./HealCheckbox.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const props = defineProps<{
  item: Item;
}>();

const baseFriendship = ref<number>(0);
const catchMultiplier = ref<number>(1);
const friendshipMultiplier = ref<number>(1);
const heal = ref<boolean>(false);
const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", value: Item): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateItemPayload = {
          pokeBall: {
            catchMultiplier: catchMultiplier.value,
            heal: heal.value,
            baseFriendship: baseFriendship.value,
            friendshipMultiplier: friendshipMultiplier.value,
          },
        };
        const item: Item = await updateItem(props.item.id, payload);
        reinitialize();
        emit("updated", item);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.item,
  (item) => {
    catchMultiplier.value = item.pokeBall?.catchMultiplier ?? 1;
    heal.value = item.pokeBall?.heal ?? false;
    baseFriendship.value = item.pokeBall?.baseFriendship ?? 0;
    friendshipMultiplier.value = item.pokeBall?.friendshipMultiplier ?? 1;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <CatchMultiplierInput class="col" required v-model="catchMultiplier" />
        <FriendshipInput class="col" label="pokemon.friendship.base" v-model="baseFriendship" />
        <FriendshipMultiplierInput class="col" required v-model="friendshipMultiplier" />
      </div>
      <HealCheckbox v-model="heal" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
