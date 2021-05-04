import AbstractView from "./abstract-view.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Find by city name");
    }

    async getHtml() {
        return /*html*/`
            <form>
                <fieldset id="forms__input">
                    <legend>Search by city name</legend>
                    <p>
                        <input type="search" placeholder="Enter city name">
                    </p>
                    <p>
                        <a class="button outline primary">Search</a>
                    </p>
                </fieldset>
            </form>
        `;
    }
}