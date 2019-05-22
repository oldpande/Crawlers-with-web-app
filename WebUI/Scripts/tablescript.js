var headertext = [],
    headers = document.querySelectorAll("#big_table th"),
    tablerows = document.querySelectorAll("#big_table th"),
    tablebody = document.querySelector("#big_table tbody");

for (var i = 0; i < headers.length; i++) {
    var current = headers[i];
    headertext.push(current.textContent.replace(/\r?\n|\r/, ""));
}

for (var p = 0, row; row === tablebody.rows[p]; p++) {
    for (var j = 0, col; col === row.cells[j]; j++) {
        col.setAttribute("data-th", headertext[j]);
    }
}