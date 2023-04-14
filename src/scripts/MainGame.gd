extends Control

var type = GlobalScript.NodeType.Game
@onready var cs_board_script_node = get_node("%CSScript")
@onready var dispatcher = GlobalScript.dispatcher_script.new(
	range(1, 10).map(func(i): return get_node("%%Grid%d" % i)), self)


var selected_node = null


func _ready():
	restart()


func restart():
	cs_board_script_node.Restart()


func execute(node, strategy):
	match node.type:
		GlobalScript.NodeType.Game:
			cs_board_script_node.ExecuteStrategiesOnBoard(strategy)
		GlobalScript.NodeType.Subgrid:
			cs_board_script_node.ExecuteStrategiesOnSubgrid(node.idx_grid, node.idx_subgrid, strategy)
		GlobalScript.NodeType.Digit:
			cs_board_script_node.ExecuteStrategiesOnDigit(node.subgrid_node.idx_grid, node.subgrid_node.idx_subgrid, node.digit, strategy)


func execute_on_board(strategy):
	execute(self, strategy)
