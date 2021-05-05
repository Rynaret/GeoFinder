import AbstractView from "./abstract-view.js";
import Table from "../components/table.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Find by IP");
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
        ip: "",
        results: undefined
    };

    onsubmit = async (e) => {
        e.preventDefault();

        if(e.target && e.target.id== `${this.guid}_submit_form`){
            await this.findByIP(new FormData(e.target));
        }
    }

    findByIP = async (formData) => {
        const ip = formData.get("ip");

        const response = await fetch(`/ip/locations?ip=${ip}`);
        const result = await response.json();

        this.state = { ...this.state, ...{ ip } };
        this.table.state = { ...this.table.state, ...{ items: [result] } };

        await this.params.render();
    }

    getHtml = async () => {
        return /*html*/`
            <form id="${this.guid}_submit_form">
                <fieldset>
                    <legend>Search by IP</legend>
                    <p>
                        <input type="search" placeholder="Enter IP" name="ip" value="${this.state.ip}">
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