import { render } from "react-dom";
import { createElement } from "react";
import { Components_HelloWorld } from "./Components.js";
import "../../../src/styles/global.scss";


render(createElement(Components_HelloWorld, null), document.getElementById("feliz-app"));

