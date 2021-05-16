import { ShadowElementBase } from "../abstractions/shadow-element-base.js";

export class Home extends ShadowElementBase {
    constructor() {
        super();
    }
}

customElements.define("home-view", Home);