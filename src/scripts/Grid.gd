extends Control

var type = GlobalScript.NodeType.Grid

@onready var dispatcher = GlobalScript.dispatcher_script.new(
	range(1, 10).map(func(i): return get_node("%%Subgrid%d" % i)), self)


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
