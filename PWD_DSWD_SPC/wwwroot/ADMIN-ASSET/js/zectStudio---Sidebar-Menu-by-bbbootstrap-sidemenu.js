document.addEventListener("DOMContentLoaded", function (event) {

    const seeNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // Toggle class for showing/hiding navbar
                nav.classList.toggle('see')
                // Toggle class for changing toggle icon
                toggle.classList.toggle('bx-x')
                // Add padding to body
                bodypd.classList.toggle('body-pd')
                // Add padding to header
                headerpd.classList.toggle('body-pd')

            })
        }
    }

    // Call the function with appropriate IDs
    seeNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link')

    function colorLink() {
        // Remove 'active' class from all links and add to clicked link
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'))
            this.classList.add('active')
        } 
    }
    // Attach event listener to each link
    linkColor.forEach(l => l.addEventListener('click', colorLink))

    // Your code to run since DOM is loaded and ready
});
