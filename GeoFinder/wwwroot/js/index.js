import { ShadowElementBase } from "./abstractions/shadow-element-base.js";
import { Menu } from "./components/menu.js";
import { Home } from "./views/home.js";
import { FindByIp } from "./views/find-by-ip.js";
import { FindByCity } from "./views/find-by-city.js";

export const routes = [
    {
        path: "/",
        brand: true,
        component: Home,
        title: "GeoFinder",
        text: "GeoFinder"
    },
    {
        path: "/find-by-ip",
        component: FindByIp,
        title: "GeoFinder: Find by IP",
        text: "Find by IP"
    },
    {
        path: "/find-by-city",
        component: FindByCity,
        title: "GeoFinder: Find by city name",
        text: "Find by city"
    }
];

const brandRoute = routes.find(route => route.brand);

const template = document.createElement('template');
template.innerHTML = /*html*/`
<div class="container">
    <header>
        <nav class="nav">
            <a class="brand" href="${brandRoute.path}" data-link="${brandRoute.title}">
                ${brandRoute.text}
            </a>
        </nav>
    </header>
    <div class="row">
        <div class="col-2" id="menu">
        </div>
        <div class="col-10" id="content">
        </div>
    </div>
</div>
`;

export class App extends ShadowElementBase {
    constructor() {
        super();

        this.link = this.link.bind(this);
        this.route = this.route.bind(this);

        const { shadowRoot } = this;

        const templateNode = document.importNode(template.content, true);

        shadowRoot.appendChild(templateNode);
    }

    link(e) {
        e.preventDefault();

        const prevPath = location.pathname;

        const newRoute = this.findNewRoute(e.target.pathname);

        if (prevPath === newRoute.path) {
            return;
        }

        history.pushState(null, newRoute.title, newRoute.path);
        document.title = newRoute.title;

        this.route(newRoute);
    }

    findNewRoute(pathname) {
        let finding = routes.find(route => pathname === route.path);
    
        if (!finding) {
            finding = routes.find(route => route.brand);
        }

        return finding;
    }

    route(newRoute) {
        const { shadowRoot, menu } = this;
    
        menu.setActiveRoute(newRoute);
    
        const content = shadowRoot.querySelector("#content");

        while (content.firstChild) {
            content.removeChild(content.firstChild);
        }
    
        content.appendChild(new newRoute.component());
    }

    connectedCallback() {
        this.menu = new Menu({ routes });

        const { shadowRoot, menu } = this;

        shadowRoot.querySelector("#menu").appendChild(menu);

        shadowRoot.querySelector("[data-link]").addEventListener("click", this.link);
        menu.shadowRoot.querySelectorAll("[data-link]")
            .forEach(item => {
                item.addEventListener("click", this.link);
            });

        const currentRoute = this.findNewRoute();
        this.route(currentRoute);
    }

    disconnectedCallback() {
        const { menu } = this;

        shadowRoot.querySelector("[data-link]").removeEventListener("click", this.link);
        menu.shadowRoot.querySelectorAll("[data-link]")
            .forEach(item => {
                item.removeEventListener("click", this.link);
            });
    }
}

customElements.define("geo-finder-app", App);