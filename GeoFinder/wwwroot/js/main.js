import FindByIp from "/views/find-by-ip.js";
import FindByCity from "/views/find-by-city.js";

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

const router = async () => {
    const routes = [
        { path: "/" },
        { path: "/find-by-ip", view: FindByIp },
        { path: "/find-by-city", view: FindByCity }
    ];

    // Test each route for potential match
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

    // remove active
    document.querySelectorAll(`a[data-link]`)
        .forEach(atag => atag.classList.remove(activeHrefClass));
    // set active for <a>
    const aTag = document.querySelectorAll(`a[href='${match.route.path}']`);
    if (aTag && aTag.length) {
        aTag[0].classList.add(activeHrefClass);
    }

    let innerHtml = "";
    if (match.route.view) {
        const view = new match.route.view(getParams(match));
        innerHtml = await view.getHtml();
    }

    document.querySelector("#app").innerHTML = innerHtml;
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