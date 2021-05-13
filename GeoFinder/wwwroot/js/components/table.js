export default class {
    constructor(state) {
        this.state = state;
    }

    state = {
        header: [],
        items: undefined
    };

    getHtml = async () => {
        if (!this.state.items) {
            return ``;
        }

        if (!this.state.items.length){
            return /*html*/`<div>No data found</div>`
        }

        return /*html*/`
            <table class="striped">
              <thead>
                ${this.getHeader()}
              </thead>
              <tbody>
                ${this.getRows()}
              </tbody>
            </table>
        `;
    }

    getHeader = () => {
        const ths = this.state.headers.map((item) => /*html*/`<th>${item}</th>`);

        return /*html*/`
            <tr>${ths.join("")}</tr>
        `;
    }

    getRows = () => {
        let tdsFunc = (item) => Object.entries(item).map(([_, value]) => /*html*/`<td>${value}</td>`);
        let trs = this.state.items.map(item => `<tr>${tdsFunc(item).join("")}</tr>`);

        return trs.join("");
    }
}