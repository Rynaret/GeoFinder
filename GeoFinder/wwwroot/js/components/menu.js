import { ShadowElementBase } from "../abstractions/shadow-element-base.js";

export class Menu extends ShadowElementBase {
    constructor(prms) {
        super();

        this.setActiveRoute = this.setActiveRoute.bind(this);

        if (prms && prms.routes) {
            this.routes = prms.routes;
        }
    }

    set routes(value) {
        this._routes = value;

        this.constructMenu();
    }

    get routes() {
        return this._routes;
    }

    constructMenu() {
        const { shadowRoot, routes } = this;

        const navs = routes
            .filter(route => !route.brand)    
            .map(route => /*html*/`
                <nav class="tabs is-full">
                    <a href="${route.path}" data-link="${route.title}">
                        ${route.text}
                    </a>
                </nav>
            `);
        
        const div = document.createElement("div");
        div.innerHTML = navs.join("");

        shadowRoot.appendChild(div);
    }

    setActiveRoute(target) {
        const { shadowRoot } = this;
        
        // <a class="... -active">
        shadowRoot
            .querySelectorAll("a")
            .forEach(atag => atag.classList.remove("active"));
    
        // <a class="... +active">
        const aTag = shadowRoot.querySelector(`a[href='${target.path}']`);
        if (aTag) {
            aTag.classList.add("active");
        }
    }
}

customElements.define("menu-cmp", Menu);