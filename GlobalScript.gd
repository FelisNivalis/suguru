extends Node

signal s_restart

var cs_board_script = load("res://BoardNode.cs")
var cs_board_node

func _ready():
	cs_board_node = cs_board_script.new()
	connect("s_restart",Callable(self,"restart"))

func restart():
	cs_board_node.Generate()
	cs_board_node.UpdateGrids(get_node("/root/Control/BoardArea/BoardContainer/Board"))

#func updateGrids():
#	var board_node = get_node("/root/Control/BoardArea/BoardContainer/Board")
#	var board = cs_board_node.GetBoard()
#	for i in range(9):
#		for j in range(9):
#			var num = board[(i / 3 * 3 + j / 3) * 9 + (i % 3 * 3 + j % 3)]
#			(board_node.get_child(i).get_child(0).get_child(j) as Label).text = num as String if num > 0 else ""
