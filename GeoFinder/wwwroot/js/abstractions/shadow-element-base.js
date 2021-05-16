import { globalCssLink } from "../configs.js";

export class ShadowElementBase extends HTMLElement {
    constructor() {
        super();

        const shadow = this.attachShadow({ mode: "open" });

        // Apply external styles to the shadow dom
        if (globalCssLink) {
            const linkElem = document.createElement("link");
            linkElem.setAttribute("rel", "stylesheet");
            linkElem.setAttribute("href", globalCssLink);
           
            shadow.appendChild(linkElem);
        }
    }
}