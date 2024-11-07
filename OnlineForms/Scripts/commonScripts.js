 /**
 * --------------------------------------------------------------------------
 * Index sort function
 * --------------------------------------------------------------------------
 */
function sortTable(n, sort) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById("submittedForms");
    switching = true;

    var rows = Array.from(table.rows);    

    // Sort direction ascending
    dir = table.getAttribute("order"); 
    if (dir == "asc") {
        dir = "desc";
    } else {
        dir = "asc";
    }

    // Sort the rows based on the first column (Name)
    rows.sort(function (a, b) {
        if (sort == "checkSort") {
            var nameA = a.cells[n].firstChild.checked; // Ignore case
            var nameB = b.cells[n].firstChild.checked;
        }
        else if (sort == "numbersort") {
            var nameA = parseFloat(a.cells[n].textContent.replace(/\$|,/g, '')); // Ignore case
            var nameB = parseFloat(b.cells[n].textContent.replace(/\$|,/g, ''));            
        }
        else {
            var nameA = a.cells[n].textContent.toUpperCase(); // Ignore case
            var nameB = b.cells[n].textContent.toUpperCase();
        }

        if (dir == "asc") {
            if (nameA > nameB) return 1;
            if (nameA < nameB) return -1;
            return 0; // names must be equal
        } else {
            if (nameA < nameB) return 1;
            if (nameA > nameB) return -1;
            return 0; // names must be equal
        }
    });
    table.setAttribute("order", dir);

    // Clear the table
    table.innerHTML = '';

    // Re-add sorted rows
    rows.forEach(row => table.appendChild(row));

    //while (switching) {
    //    switching = false;
    //    rows = table.rows;

    //    for (i = 0; i < (rows.length - 1); i++) {
    //        shouldSwitch = false;
    //        x = rows[i].getElementsByTagName("TD")[n];
    //        y = rows[i + 1].getElementsByTagName("TD")[n];
    //        if (sort == "numbersort") {
    //            xnum = x.innerHTML.replace(/(\d*),(\d+).\d\d/, '$1$2');
    //            ynum = y.innerHTML.replace(/(\d*),(\d+).\d\d/, '$1$2');

    //            if (dir == "asc") {
    //                if (parseFloat(xnum.match(/(\d*)(\d+).\d\d/)[0]) > parseFloat(ynum.match(/(\d*)(\d+).\d\d/)[0])) {
    //                    shouldSwitch = true;
    //                    break;
    //                }
    //            } else if (dir == "desc") {
    //                if (parseFloat(xnum.match(/(\d*)(\d+).\d\d/)[0]) < parseFloat(ynum.match(/(\d*)(\d+).\d\d/)[0])) {
    //                    shouldSwitch = true;
    //                    break;
    //                }
    //            }
    //        } else {
    //            if (dir == "asc") {
    //                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
    //                    shouldSwitch = true;
    //                    break;
    //                }
    //            } else if (dir == "desc") {
    //                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
    //                    shouldSwitch = true;
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    if (shouldSwitch) {
    //        rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
    //        switching = true;

    //        switchcount++;
    //    } else {
    //        if (switchcount == 0 && dir == "asc") {
    //            dir = "desc";
    //            switching = true;
    //        }
    //    }
    //}
}