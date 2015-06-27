$(function () {

    var tree = $("#file-tree");
    var uploadForm = $('#update-target');
   
    $('form').submit(function (e) {
        e.preventDefault();
        var formData = new FormData($(this)[0]);

        $.ajax({
            url: this.action,
            type: this.method,
            data: formData,
            async: true,
            cache: false,
            contentType: false,
            processData: false,
            error: function (result) {
                alert('Error')
            },
            success: function (result) {
                uploadForm.html(result);
                var node = tree.jstree(true).get_selected();
                tree.jstree(true).refresh(node[0]);
            }
        });
    });
});