import { class_type } from "../.fable/fable-library.3.1.11/Reflection.js";
import { createElement } from "react";
import * as react from "react";
import { useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.38.0/React.fs.js";
import { tail, head, isEmpty, ofArray } from "../.fable/fable-library.3.1.11/List.js";
import { Interop_reactApi } from "../.fable/Feliz.1.38.0/Interop.fs.js";
import { RouterModule_router, RouterModule_urlSegments } from "../.fable/Feliz.Router.3.7.0/Router.fs.js";
import { singleton, delay, toList } from "../.fable/fable-library.3.1.11/Seq.js";

export class Components {
    constructor() {
    }
}

export function Components$reflection() {
    return class_type("App.Components", void 0, Components);
}

export function Components_HelloWorld() {
    return createElement("h1", {
        children: ["Hello World"],
    });
}

export function Components_Counter() {
    const patternInput = useFeliz_React__React_useState_Static_1505(0);
    const count = patternInput[0] | 0;
    const children = ofArray([createElement("h1", {
        children: [count],
    }), createElement("button", {
        onClick: (_arg1) => {
            patternInput[1](count + 1);
        },
        children: "Increment",
    })]);
    return createElement("div", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
}

export function Components_Router() {
    const patternInput = useFeliz_React__React_useState_Static_1505(RouterModule_urlSegments(window.location.hash, 1));
    const currentUrl = patternInput[0];
    return RouterModule_router({
        onUrlChanged: patternInput[1],
        application: react.createElement(react.Fragment, {}, ...toList(delay(() => ((!isEmpty(currentUrl)) ? ((head(currentUrl) === "hello") ? (isEmpty(tail(currentUrl)) ? singleton(createElement(Components_HelloWorld, null)) : singleton(createElement("h1", {
            children: ["Not found"],
        }))) : ((head(currentUrl) === "counter") ? (isEmpty(tail(currentUrl)) ? singleton(createElement(Components_Counter, null)) : singleton(createElement("h1", {
            children: ["Not found"],
        }))) : singleton(createElement("h1", {
            children: ["Not found"],
        })))) : singleton(createElement("h1", {
            children: ["Index"],
        })))))),
    });
}

