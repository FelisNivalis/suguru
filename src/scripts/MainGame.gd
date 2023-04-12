extends Control

var cs_board_script = load("res://scripts/BoardScriptNode.cs")
@onready var board_node: Control = get_node("%Board")
@onready var cs_board_script_node = new_board_script_node()


func new_board_script_node():
	var script_node = cs_board_script.new()
	script_node.boardNode = board_node
	return script_node


func _ready():
	restart()


func _on_eliminate_button_pressed():
	cs_board_script_node.Eliminate()


func restart():
	cs_board_script_node.Generate()
	cs_board_script_node.InitGrids()
	for i in range(1, 10):
		for j in range(1, 10):
			var node = board_node.get_node("%%Grid%d" % i).get_node("%%Subgrid%d" % j)
			node.reset_candidates()
