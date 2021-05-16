import { ShadowElementBase } from "../abstractions/shadow-element-base.js";
import { SimpleTable } from "../components/table.js";

const template = document.createElement('template');
template.innerHTML = /*html*/`
<form>
    <fieldset>
        <legend>Search by IP</legend>
        <p>
            <input type="search" placeholder="Enter IP" name="ip">
        </p>
        <p>
            <button type="submit" class="button outline primary">Search</button>
        </p>
    </fieldset>
</form>
`;

export class FindByIp extends ShadowElementBase {
    constructor() {
        super();
        this.submit = this.submit.bind(this);

        this.table = new SimpleTable({
            header: [
                "Country",
                "Region",
                "Postal",
                "City",
                "Organization",
                "Latitude",
                "Longitude",
            ]
        });

        const { shadowRoot, table } = this;

        const templateNode = document.importNode(template.content, true);
        shadowRoot.appendChild(templateNode);
        shadowRoot.appendChild(table);
    }

    async submit(e) {
        e.preventDefault();

        const formData = new FormData(e.target);
        const ip = formData.get("ip");

        const response = await fetch(`/ip/location?ip=${ip}`);
        const result = response.status === 200
            ? [await response.json()]
            : [];

        this.table.items = result;
    }

    connectedCallback() {
        const { shadowRoot } = this;

        shadowRoot.querySelector("form").addEventListener("submit", this.submit);
    }

    disconnectedCallback() {
        const { shadowRoot } = this;

        shadowRoot.querySelector('form').removeEventListener('submit', this.submit);
    }
}

customElements.define("find-by-ip-view", FindByIp);