function SetActiveLink() {
    let currentLocation = window.location.pathname;

    let navLinks = document.querySelectorAll('.navbar-link');

    navLinks.forEach(linkItem => {
        let linkHref = linkItem.getAttribute("href");

        if (linkHref === currentLocation) {
            linkItem.classList.add('active');
        }
        else {
            linkItem.classList.remove('active');
        }
    });
}

function toggleNavbar() {
    let navbarCollapse = document.getElementById("navbar-collapse-section");

    navbarCollapse.classList.toggle("show");

    /*
    navbarCollapse.style.height = 0;

    navbarCollapse.classList.add("navbar-collapse-animation");
    navbarCollapse.style.height = `${navbarCollapse.getBoundingClientRect().height}px`;
    navbarCollapse.offsetHeight;

    setTimeout(() => {
        navbarCollapse.classList.remove("navbar-collapse-animation");
        navbarCollapse.style.height = "";
    }, 500);
    */
}

Blazor.addEventListener('enhancedload', () => {
    window.scrollTo(0, 0);

    SetActiveLink();

    let navbarCollapse = document.getElementById("navbar-collapse-section");

    if (navbarCollapse.classList.contains("show")) {
        navbarCollapse.classList.remove("show");
    }
});

window.addEventListener("load", () => {
    let navbarToggler = document.getElementById("navbar-toggler-button");

    navbarToggler.addEventListener("click", toggleNavbar);

    let navbarLinks = document.querySelectorAll(".navbar-link");

    navbarLinks.forEach(linkItem => {
        linkItem.addEventListener("click", () => {
            let navbarCollapse = document.getElementById("navbar-collapse-section");

            if (navbarCollapse.classList.contains("show")) {
                navbarCollapse.classList.remove("show");
            }
        });
    });
});