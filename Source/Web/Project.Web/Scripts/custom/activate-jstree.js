$(function () {

    var searchInput = $('#search');
    var tree = $("#FileTree");
    var uploadForm = $('#upload-form');

    searchInput.on('input', function () {
        var value = $(this).val();
        tree.jstree(true).search(value);
    });

    function contextMenuConfiguration(node) {

        if (node.type === 'file') {
            return {
                "Delete": {
                    "label": "Delete File",
                    "action": function (obj) {

                        $.ajax({
                            url: '/FileSystem/DeleteFile',
                            type: 'POST',
                            async: false,
                            data: { dir: node.id },
                            error: function (data) {
                                alert('Unable to Delete')
                            },
                            success: function (data) {
                                tree.jstree(true).delete_node(node)
                            }
                        });
                    }
                }
            }
        };
        if (node.type === 'root') {
            return {
                "Delete": {
                    "label": "Delete folder",
                    "action": function (obj) {

                        $.ajax({
                            url: '/FileSystem/DeleteFolder',
                            type: 'POST',
                            async: false,
                            data: { dir: node.id },
                            error: function (data) {
                                alert('Unable to Delete')
                            },
                            success: function (data) {
                                tree.jstree(true).delete_node(node)
                            }
                        });
                    }
                }
            }
        }
    }

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

    var contextmenuConfig = {
        "items": contextMenuConfiguration
    };

    $('#FileTree').jstree({
        "plugins": pluginConfig,
        "contextmenu": contextmenuConfig,
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
        'types': typesConfig
    }).bind("select_node.jstree", function (e, data) {
        var nodeType = data.node.type;

        if (nodeType == 'root') {
            uploadForm.show()
            $('#Directory').val(data.node.id);
        }
        else {
            uploadForm.hide();
        }
    })
})

