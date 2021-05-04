import AbstractView from "./abstract-view.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Find by IP");
    }

    async getHtml() {
        return /*html*/`
            <form>
                <fieldset id="forms__input">
                    <legend>Search by IP</legend>
                    <p>
                        <input type="search" placeholder="Enter IP">
                    </p>
                    <p>
                        <a class="button outline primary">Search</a>
                    </p>
                </fieldset>
            </form>
        `;
    }
}