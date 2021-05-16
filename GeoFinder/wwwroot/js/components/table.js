import { ShadowElementBase } from "../abstractions/shadow-element-base.js";

const isHiddenClass = "is-hidden"

const template = document.createElement("template");
template.innerHTML = /*html*/`
    <table class="striped ${isHiddenClass}">
        <thead></thead>
        <tbody></tbody>
    </table>
`;

const templateNoData = document.createElement("template");
templateNoData.innerHTML = /*html*/`<div class="${isHiddenClass}">No data found</div>`;

export class SimpleTable extends ShadowElementBase {
    constructor(prms) {
        super();

        const templateNode = document.importNode(template.content, true);
        const templateNoDataNode = document.importNode(templateNoData.content, true);

        const { shadowRoot } = this;
        
        shadowRoot.appendChild(templateNode);
        shadowRoot.appendChild(templateNoDataNode);

        this.tableNoData = shadowRoot.querySelector("div");
        this.table = shadowRoot.querySelector("table");
        
        if (prms && prms.header) {
            this.header = prms.header;
        }
    }

    set header(value) {
        this._header = value;

        this.constructHeader();
    }

    get header() {
        return this._header;
    }

    set items(value) {
        this._items = value;

        this.constructRows();
    }

    get items() {
        return this._items;
    }

    constructHeader() {
        const { table, header } = this;

        const ths = header.map((item) => /*html*/`<th>${item}</th>`);

        table.querySelector("thead").innerHTML = /*html*/`
            <tr>${ths.join("")}</tr>
        `;
    }

    constructRows() {
        const { tableNoData, table, items } = this;

        tableNoData.classList.add(isHiddenClass);
        table.classList.add(isHiddenClass);

        if (!items) {
            return;
        }

        if (!items.length) {
            tableNoData.classList.remove(isHiddenClass);
            return;
        }

        table.classList.remove(isHiddenClass);

        const tdsFunc = (item) => Object.entries(item).map(([_, value]) => /*html*/`<td>${value}</td>`);
        const trs = items.map(item => /*html*/`<tr>${tdsFunc(item).join("")}</tr>`);

        table.querySelector("tbody").innerHTML = trs.join("");
    }
}

customElements.define("table-cmp", SimpleTable);