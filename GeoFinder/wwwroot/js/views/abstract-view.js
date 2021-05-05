export default class {
    constructor(params) {
        this.params = params;

        this.guid = "id_" + performance.now();
    }

    setTitle = (title) => {
        document.title = title;
    }

    getHtml = async () => {
        return "";
    }
}