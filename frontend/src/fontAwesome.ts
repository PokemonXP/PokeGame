import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBriefcaseMedical,
  faBullseye,
  faCartShopping,
  faCheck,
  faCompactDisc,
  faDice,
  faDumbbell,
  faEarthAsia,
  faHandSparkles,
  faHome,
  faList,
  faMars,
  faMasksTheater,
  faPaw,
  faPerson,
  faPlus,
  faQuestion,
  faSeedling,
  faTable,
  faTimes,
  faUser,
  faUserTie,
  faVenus,
  faVial,
  faWandSparkles,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBriefcaseMedical,
  faBullseye,
  faCartShopping,
  faCheck,
  faCompactDisc,
  faDice,
  faDumbbell,
  faEarthAsia,
  faHandSparkles,
  faHome,
  faList,
  faMars,
  faMasksTheater,
  faPaw,
  faPerson,
  faPlus,
  faQuestion,
  faSeedling,
  faTable,
  faTimes,
  faUser,
  faUserTie,
  faVenus,
  faVial,
  faWandSparkles,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
