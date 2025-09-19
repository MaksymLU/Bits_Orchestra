$(document).ready(function () {

    let currentSort = { column: null, asc: true };

    function sortTable(column) {
        let rows = $("#contactsTable tbody tr").get();

        rows.sort(function (a, b) {
            let valA = $(a).find(`td[data-field="${column}"]`).text().trim();
            let valB = $(b).find(`td[data-field="${column}"]`).text().trim();

            if (!isNaN(valA) && !isNaN(valB)) {
                valA = parseFloat(valA);
                valB = parseFloat(valB);
            }
            else if (Date.parse(valA) && Date.parse(valB)) {
                valA = new Date(valA);
                valB = new Date(valB);
            }

            if (valA < valB) return currentSort.asc ? -1 : 1;
            if (valA > valB) return currentSort.asc ? 1 : -1;
            return 0;
        });

        $.each(rows, function (index, row) {
            $("#contactsTable tbody").append(row);
        });
    }

    $(document).on("click", ".sort-btn", function () {
        let column = $(this).data("column");

        if (currentSort.column === column) {
            currentSort.asc = !currentSort.asc; 
        } else {
            currentSort.column = column;
            currentSort.asc = true;
        }

        $(".sort-btn").removeClass("active");
        $(".sort-indicator").text("");

        $(this).addClass("active");
        $(this).find(".sort-indicator").text(currentSort.asc ? "▲" : "▼");

        sortTable(column);
    });



    $("#uploadForm").submit(function (e) {
        e.preventDefault();

        let formData = new FormData(this);

        $.ajax({
            url: "/FileUpload/upload",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            beforeSend: function () {
                $("#uploadStatus").text("Uploading...");
            },
            success: function () {
                $("#uploadStatus").text("Upload successful!");
                loadContacts();
            },
            error: function () {
                $("#uploadStatus").text("Upload failed. Сheck the file!");
            }
        });
    });

    function loadContacts() {
        $.get("/Users/GetAll", function (data) {
            let rows = "";
            data.forEach(user => {
                rows += `
                <tr data-id="${user.userId}">
                    <td contenteditable="true" class="editable" data-field="Name">${user.name}</td>
                    <td contenteditable="true" class="editable" data-field="DateOfBirth">${user.dateOfBirth.split("T")[0]}</td>
                    <td contenteditable="true" class="editable" data-field="Married">${user.married}</td>
                    <td contenteditable="true" class="editable" data-field="Phone">${user.phone}</td>
                    <td contenteditable="true" class="editable" data-field="Salary">${user.salary}</td>
                    <td>
                        <button class="btn btn-sm btn-save saveBtn">Save</button>
                        <button class="btn btn-sm btn-danger deleteBtn">Delete</button>
                    </td>
                </tr>`;
            });
            $("#contactsTable tbody").html(rows);
        });
    }


    $(document).on("focus", ".editable", function () {
        $(this).data("original", $(this).text().trim());
    });

    $(document).on("click", ".saveBtn", function () {
        let row = $(this).closest("tr");
        let userId = row.data("id");
        let updatedUser = {};

        try {
            row.find(".editable").each(function () {
                let field = $(this).data("field");
                let value = $(this).text().trim();

                if ((field === "Name" || field === "Phone") && !value) {
                    throw { field, message: field + " cannot be empty." };
                }

                if (field === "Salary") {
                    if (!/^\d+(\.\d+)?$/.test(value)) {
                        throw { field, message: "Salary must be a valid number." };
                    }
                    value = parseFloat(value);
                }

                if (field === "DateOfBirth") {
                    let date = new Date(value);
                    if (isNaN(date.getTime())) {
                        throw { field, message: "DateOfBirth is not valid." };
                    }
                    value = date.toISOString();
                }

                if (field === "Married") {
                    let lower = value.toLowerCase();
                    if (lower !== "true" && lower !== "false") {
                        throw { field, message: "Married must be true or false." };
                    }
                    value = lower === "true";
                }
                if (field === "Phone") {
                    if (!value) {
                        throw { field, message: "Phone cannot be empty." };
                    }

                    if (/[a-zA-Zа-яА-ЯіїєґІЇЄҐ]/.test(value)) {
                        throw { field, message: "Phone cannot contain letters." };
                    }

                    if (!/^[0-9+\-()\s]+$/.test(value)) {
                        throw { field, message: "Phone contains invalid characters." };
                    }
                }


                updatedUser[field] = value;
            });

            $.ajax({
                url: "/Users/Update/" + userId,
                type: "PUT",
                contentType: "application/json",
                data: JSON.stringify(updatedUser),
                success: function () {
                    loadContacts();
                    alert("User updated =)");
                },
                error: function (xhr) {
                    console.error(xhr.responseText);
                    alert("Update failed!");
                }
            });

        } catch (error) {
            // Повертаємо тільки неправильну клітинку
            let cell = row.find(`[data-field='${error.field}']`);
            let original = cell.data("original");
            if (original !== undefined) {
                cell.text(original);
            }

            alert("Validation error: " + error.message);
        }
    });




    $(document).on("click", ".deleteBtn", function () {
        let userId = $(this).closest("tr").data("id");

        if (!confirm("Are you sure you want to delete this user?")) return;

        $.ajax({
            url: "/Users/Delete/" + userId,
            type: "DELETE",
            success: function () {
                loadContacts();
            },
            error: function () {
                alert("Delete failed!");
            }
        });
    });

    $("#searchBox").on("keyup", function () {
        let value = $(this).val().toLowerCase();
        $("#contactsTable tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    $("#contactsTable th[data-column]").on("click", function () {
        let column = $(this).data("column");
        let rows = $("#contactsTable tbody tr").get();

        rows.sort(function (a, b) {
            let A = $(a).find(`[data-field='${column}']`).text().toUpperCase();
            let B = $(b).find(`[data-field='${column}']`).text().toUpperCase();

            if ($.isNumeric(A) && $.isNumeric(B)) {
                return A - B;
            }
            return A.localeCompare(B);
        });

        $.each(rows, function (index, row) {
            $("#contactsTable tbody").append(row);
        });
    });

    loadContacts();
});
