$(function () {

    var tree = $("#file-tree");
    var searchInput = $('#search');
    var renameBtn = $('#rename-btn');
    var deleteBtn = $('#delete-btn');
    var updateTarget = $('#update-target');

    searchInput.on('input', searchItem);
    renameBtn.on('click', renameItem);
    deleteBtn.on('click', deleteItem);

    updateTarget.hide();

    var typesConfig = {
        "default": { "valid_children": ["default", "file", "root"] },
        "file": { "icon": "glyphicon glyphicon-file", "valid_children": [] },
        "root": { "icon": "glyphicon glyphicon-folder-open", "valid_children": [] }
    }

    var pluginConfig = [
                    "contextmenu", "dnd", "search",
                    "state", "types", "wholerow"
    ];

    var themeConfig = {
        'url': '/ExternalLibraries/jstree/themes/default/style.css',
        'responsive': true,
        'variant': 'large',
        'stripes': true
    };

    tree.jstree({
        "plugins": pluginConfig,
        'core': {
            "multiple": false,
            "animation": 1,
            "check_callback": true,
            "themes": { "default-dark": true },
            'data': {
                'url': '/FileSystem/PopulateData',
                'data': function (node) {
                    return { 'id': node.id };
                }
            },
            'check_callback': function (o, n, p, i, m) {
                if (m && m.dnd && m.pos !== 'i') { return false; }
                if (o === "move_node" || o === "copy_node") {
                    if (this.get_node(n).parent === this.get_node(p).id) { return false; }
                }
                return true;
            },
            'themes': themeConfig
        },
        'types': typesConfig,
        'state': 'close_all'
    }).bind("rename_node.jstree", function (e, data) {

        var nodeType = data.node.type;
        var sendData = { directory: data.node.id, newName: data.text, oldName: data.old };

        if (nodeType == 'root') {
            callAjax('/FileSystem/RenameFolder', 'POST', sendData);
        }
        if (nodeType == 'file') {
            callAjax('/FileSystem/RenameFile', 'POST', sendData);
        }

    }).bind("delete_node.jstree", function (e, data) {

        var nodeType = data.node.type;
        var sendData = { path: data.node.id };

        if (nodeType == 'root') {
            callAjax('/FileSystem/DeleteFolder', 'POST', sendData);
        }
        if (nodeType == 'file') {
            callAjax('/FileSystem/DeleteFile', 'POST', sendData);
        }

    }).bind("select_node.jstree", function (e, data) {
        var nodeType = data.node.type;

        if (nodeType == 'root') {
            updateTarget.show();
            $('#Directory').val(data.node.id);
        }
        else {
            updateTarget.hide();
        }
    })

    function callAjax(url, type, data) {

        $.ajax({
            url: url,
            type: type,
            async: false,
            data: data,
            error: function (result) {
                alert('Error')
            },
            success: function (result) {
                if (!result.success) {
                    alert(result.message);
                    var node = tree.jstree(true).get_selected();
                    tree.jstree(true).refresh(node[0]);
                }
            }
        });
    }

    function renameItem() {
        var tree = $('#file-tree').jstree(true);
        var selectedItem = tree.get_selected();

        if (!selectedItem.length) {
            return false;
        }

        selectedItem = selectedItem[0];
        tree.edit(selectedItem);
    };

    function deleteItem() {
        var tree = $('#file-tree').jstree(true);
        var selectedItem = tree.get_selected();

        if (!selectedItem.length) {
            return false;
        }

        selectedItem = selectedItem[0];
        tree.delete_node(selectedItem);
    };

    function searchItem() {
        var value = $(this).val();
        tree.jstree(true).search(value);
    };
})

