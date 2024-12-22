// main.js

// Menu Toggle
let toggle = document.querySelector(".toggle");
let navigation = document.querySelector(".navigation");
let main = document.querySelector(".main");

toggle.onclick = function () {
    navigation.classList.toggle("active");
    main.classList.toggle("active");

    // Smooth content transition when toggling the sidebar
    main.style.transition = "all 0.5s ease";
    navigation.style.transition = "all 0.5s ease";
};

// FullCalendar Initialization
document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth', // Change the initial view as needed
        dateClick: function (info) {
            // Replace the default alert with a custom message/modal
            let messageEl = document.createElement('div');
            messageEl.innerHTML = `<p>You clicked on: ${info.dateStr}</p>`;
            messageEl.style.padding = "10px";
            messageEl.style.background = "#f1f1f1";
            messageEl.style.border = "1px solid #ccc";
            messageEl.style.borderRadius = "5px";
            messageEl.style.marginTop = "10px";

            // Ensure previous message is removed
            let previousMessage = document.querySelector('.calendar-message');
            if (previousMessage) {
                previousMessage.remove();
            }

            // Assign a class for easy identification and removal
            messageEl.classList.add('calendar-message');
            calendarEl.appendChild(messageEl);
        },
    });

    calendar.render();
});
