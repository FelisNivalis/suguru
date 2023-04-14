extends Node
class_name Dispatcher


var dispatch_list
var node


func _init(dispatch_list, node):
	self.dispatch_list = dispatch_list
	self.node = node


func dispatch(func_name):
	for child in dispatch_list:
		if child.has_method(func_name):
			child.call(func_name)
		if child.get("dispatcher"):
			child.dispatcher.dispatch(func_name)
