extends Control

var type = GlobalScript.NodeType.Subgrid

@onready var maingame_node: Control = get_node("/root/MainGame")
@onready var candidates_node: Control = get_node("%Candidates")
@onready var answer_node: Control = get_node("%Answer")
@onready var answer_label_node: Label = get_node("%AnswerLabel")
@onready var grid_node = get_node("../..")
@onready var idx_grid = grid_node.get_index() + 1
@onready var idx_subgrid = get_index() + 1
@onready var dispatcher = GlobalScript.dispatcher_script.new(
	range(1, 10).map(func(i): return get_node("%%Digit%d" % i)), self)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass


func _on_click_answer(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_RIGHT:
				if maingame_node.selected_node == self:
					maingame_node.execute(maingame_node.selected_node, "unselect_subgrid")
					maingame_node.selected_node = null
				maingame_node.execute(self, "unfill_subgrid")
				accept_event()
				return
			if event.button_index == MOUSE_BUTTON_LEFT:
				if maingame_node.selected_node:
					maingame_node.execute(maingame_node.selected_node, "unselect_subgrid")
				maingame_node.execute(self, "select_subgrid")
				maingame_node.selected_node = self
				accept_event()
				return
			print_debug("clicked on ", answer_label_node.text)
			accept_event()


func fill_with(num):
	if num > 0:
		candidates_node.visible = false
		answer_label_node.text = str(num)
		answer_node.visible = true
	else:
		answer_node.visible = false
		answer_label_node.text = ""
		candidates_node.visible = true
