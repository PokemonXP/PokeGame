import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBriefcaseMedical,
  faBullseye,
  faCartShopping,
  faCompactDisc,
  faDumbbell,
  faEarthAsia,
  faHandSparkles,
  faHome,
  faList,
  faMasksTheater,
  faPaw,
  faPerson,
  faPlus,
  faSeedling,
  faUser,
  faUserTie,
  faVial,
  faWandSparkles,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faBriefcaseMedical,
  faBullseye,
  faCartShopping,
  faCompactDisc,
  faDumbbell,
  faEarthAsia,
  faHandSparkles,
  faHome,
  faList,
  faMasksTheater,
  faPaw,
  faPerson,
  faPlus,
  faSeedling,
  faUser,
  faUserTie,
  faVial,
  faWandSparkles,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
