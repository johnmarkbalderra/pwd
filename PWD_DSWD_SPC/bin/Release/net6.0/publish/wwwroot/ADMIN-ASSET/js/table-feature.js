function handleSort() {
    var select = document.getElementById("sortSelect");
    var columnIndex = select.value; // Get the selected column index
    if (columnIndex) {
        sortTable(columnIndex);
    }
}

function sortTable(columnIndex) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.querySelector(".table");
    switching = true;
    dir = "asc";

    while (switching) {
        switching = false;
        rows = Array.from(table.rows).slice(1); // Convert rows to an array and skip the header

        for (i = 0; i < rows.length - 1; i++) {
            shouldSwitch = false;
            x = rows[i].cells[columnIndex].innerText.toLowerCase();
            y = rows[i + 1].cells[columnIndex].innerText.toLowerCase();

            if (dir === "asc" && x > y) {
                shouldSwitch = true;
                break;
            } else if (dir === "desc" && x < y) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else {
            if (switchcount === 0 && dir === "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}

function searchTable() {
    var input = document.getElementById("searchInput");
    var filter = input.value.toLowerCase();
    var table = document.querySelector(".table");
    var rows = table.getElementsByTagName("tr");

    for (var i = 1; i < rows.length; i++) { // Start at 1 to skip the header row
        var cells = rows[i].getElementsByTagName("td");
        var found = false;

        for (var j = 0; j < cells.length; j++) {
            if (cells[j].innerText.toLowerCase().includes(filter)) {
                found = true;
                break;
            }
        }

        rows[i].style.display = found ? "" : "none"; // Show or hide the row
    }
}
