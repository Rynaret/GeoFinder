﻿import FindByIp from "./views/find-by-ip.js";
import FindByCity from "./views/find-by-city.js";

const pathToRegex = path => new RegExp("^" + path.replace(/\//g, "\\/").replace(/:\w+/g, "(.+)") + "$");

const getParams = match => {
    const values = match.result.slice(1);
    const keys = Array.from(match.route.path.matchAll(/:(\w+)/g)).map(result => result[1]);

    return Object.fromEntries(keys.map((key, i) => {
        return [key, values[i]];
    }));
};

const navigateTo = url => {
    history.pushState(null, null, url);
    router();
};

const activeHrefClass = "active";

let currentView = undefined;

const render = async () => {
    let innerHtml = "";
    if (currentView) {
        innerHtml = await currentView.getHtml();
    }

    document.querySelector("#app").innerHTML = innerHtml;
};

const router = async () => {
    const routes = [
        { path: "/" },
        { path: "/find-by-ip", view: FindByIp },
        { path: "/find-by-city", view: FindByCity }
    ];

    // проверяем каждый путь на потенациальное совпадение
    const potentialMatches = routes.map(route => {
        return {
            route: route,
            result: location.pathname.match(pathToRegex(route.path))
        };
    });

    let match = potentialMatches.find(potentialMatch => potentialMatch.result !== null);

    if (!match) {
        match = {
            route: routes[0],
            result: [location.pathname]
        };
    }

    // убираем класс active
    document.querySelectorAll(`a[data-link]`)
        .forEach(atag => atag.classList.remove(activeHrefClass));
    // добавляем active для <a>
    const aTag = document.querySelectorAll(`a[href='${match.route.path}']`);
    if (aTag && aTag.length) {
        aTag[0].classList.add(activeHrefClass);
    }

    let view = match.route.view;
    if (view) {
        view = new match.route.view({ ...getParams(match), render });
    }

    // dispose предыдущий роут
    if (currentView) {
        await currentView.dispose();
    }
    
    currentView = view;

    render();
};

window.addEventListener("popstate", router);

document.addEventListener("DOMContentLoaded", () => {
    document.body.addEventListener("click", e => {
        if (e.target.matches("[data-link]")) {
            e.preventDefault();
            navigateTo(e.target.href);
        }
    });

    router();
});