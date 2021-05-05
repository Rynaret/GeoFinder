import AbstractView from "./abstract-view.js";
import Table from "../components/table.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Find by city name");
        this.table = new Table({
            headers: [
                "Country",
                "Region",
                "Postal",
                "City",
                "Organization",
                "Latitude",
                "Longitude",
            ],
            items: undefined
        });

        document.addEventListener('submit', this.onsubmit);
    }

    state = {
        city: ""
    };

    onsubmit = async (e) => {
        e.preventDefault();

        if(e.target && e.target.id== `${this.guid}_submit_form`){
            await this.findByCity(new FormData(e.target));
        }
    }

    findByCity = async (formData) => {
        const city = formData.get("city");

        const response = await fetch(`/city/locations?city=${city}`);
        const result = await response.json();

        this.state = { ...this.state, ...{ city } };
        this.table.state = { ...this.table.state, ...{ items: result } };

        await this.params.render();
    }

    getHtml = async () => {
        return /*html*/`
            <form id="${this.guid}_submit_form">
                <fieldset>
                    <legend>Search by city name</legend>
                    <p>
                        <input type="search" placeholder="Enter city name" name="city" value="${this.state.city}">
                    </p>
                    <p>
                        <button type="submit" class="button outline primary">Search</button>
                    </p>
                </fieldset>
            </form>

            ${await this.table.getHtml()}
        `;
    }

    dispose = async () => {
        document.removeEventListener('submit', onsubmit);
    }
}