<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import TrainerGameBag from "@/components/inventory/TrainerGameBag.vue";
import type { ChangePokemonItemPayload, Inventory, InventoryItem, ItemSummary, PokemonSummary } from "@/types/game";
import { getInventory } from "@/api/game/trainers";
import { changeHeldItem } from "@/api/game/pokemon";

const route = useRoute();
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    pokemon: PokemonSummary;
  }>(),
  {
    id: "change-held-item",
  },
);

const inventory = ref<Inventory>();
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<InventoryItem>();

const heldItem = computed<ItemSummary | undefined>(() => props.pokemon.heldItem ?? undefined);
const icon = computed<string>(() => (heldItem.value ? "fas fa-rotate" : "fas fa-hand"));
const text = computed<string>(() => (heldItem.value ? "items.held.swap" : "items.held.give"));

const emit = defineEmits<{
  (e: "changed", item?: InventoryItem): void;
  (e: "error", error: unknown): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  selected.value = undefined;
  hide();
}
async function open(): Promise<void> {
  if (!isLoading.value) {
    if (!inventory.value) {
      isLoading.value = true;
      try {
        const trainerId: string = route.params.trainer.toString();
        if (!trainerId) {
          throw new Error("ChangeHeldItem: a trainer ID is required.");
        }
        inventory.value = await getInventory(trainerId);
      } catch (e: unknown) {
        emit("error", e);
      } finally {
        isLoading.value = false;
      }
    }
    show();
  }
}

function onToggle(item: InventoryItem) {
  if (selected.value?.id === item.id) {
    selected.value = undefined;
  } else {
    selected.value = item;
  }
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: ChangePokemonItemPayload = {
        heldItem: selected.value?.id,
      };
      await changeHeldItem(props.pokemon.id, payload);
      emit("changed", selected.value);
      cancel();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <span>
    <TarButton :disabled="isLoading" :icon="icon" :loading="isLoading" :status="t('loading')" :text="t(text)" @click="open" />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" size="x-large" :title="t(text)">
      <TrainerGameBag v-if="inventory" clickable :inventory="inventory" :selected="selected" @toggled="onToggle" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading || !selected"
          :icon="icon"
          :loading="isLoading"
          :status="t('loading')"
          :text="t(text)"
          variant="primary"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
