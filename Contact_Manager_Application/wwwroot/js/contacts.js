$(document).ready(function () {

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
                $("#uploadStatus").text("Upload failed!");
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
                            <button class="btn btn-sm btn-success saveBtn">Save</button>
                            <button class="btn btn-sm btn-danger deleteBtn">Delete</button>
                        </td>
                    </tr>`;
            });
            $("#contactsTable tbody").html(rows);
        });
    }

    $(document).on("click", ".saveBtn", function () {
        let row = $(this).closest("tr");
        let userId = row.data("id");
        let updatedUser = {};

        row.find(".editable").each(function () {
            let field = $(this).data("field");
            let value = $(this).text().trim();

            if (field === "Married") {
                value = value.toLowerCase() === "true"; 
            } else if (field === "Salary") {
                value = parseFloat(value);
            } else if (field === "DateOfBirth") {
                value = new Date(value).toISOString();
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
