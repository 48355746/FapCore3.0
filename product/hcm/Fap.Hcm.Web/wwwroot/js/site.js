// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$().on('delete_node.jstree', function (e, data) {
	$.post('##editurl##', { operation: 'delete_node', 'id': data.node.id }, function (rv) {
		if (!rv.success) {
			bootbox.alert('删除失败，可能被占用。');
		}
		data.instance.refresh();
	});
}).on('create_node.jstree', function (e, data) {
	$.post('##editurl##', { operation: 'create_node', 'id': data.node.parent, 'position': data.position, 'text': data.node.text }, function (rv) {
		if (rv.success) {
			data.instance.set_id(data.node, rv.data);
			data.instance.edit(data.node, '自定义');
		}
		data.instance.refresh();
	});
}).on('rename_node.jstree', function (e, data) {
	$.post('##editurl##', { operation: 'rename_node', 'id': data.node.id, 'text': data.text }, function (rv) {
		data.instance.refresh();
	});
}).on('move_node.jstree', function (e, data) {
	$.post('##editurl##', { operation: 'move_node', 'id': data.node.id, 'parent': data.parent, 'position': data.position }, function (rv) {
		data.instance.refresh();
	});
}).on('copy_node.jstree', function (e, data) {
	$.post('##editurl##', { operation: 'copy_node', 'id': data.original.id, 'parent': data.parent, 'position': data.position }, function (rv) {
		data.instance.refresh();
	});
});	