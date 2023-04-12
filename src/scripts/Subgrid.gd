extends Control

signal s_fill_with(num)
signal s_eliminate(num)

@onready var maingame_node: Control = get_node("/root/Control")
@onready var candidates_node: Control = get_node("Candidates")
@onready var answer_node: Control = get_node("Answer")
@onready var answer_label_node: Label = get_node("Answer/Label")
@onready var grid_node = get_node("../..")
@onready var xy = GlobalScript.idx_to_xy(grid_node.get_index(), get_index())
@onready var x = xy[0]
@onready var y = xy[1]


# Called when the node enters the scene tree for the first time.
func _ready():
	connect("s_fill_with", Callable(self, "fill_with"))
	connect("s_eliminate", Callable(self, "eliminate"))


func _on_click_answer(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_RIGHT:
				# redo tree
				fill_with(0)
				accept_event()
				return
			print_debug("clicked on ", answer_label_node.text)
			accept_event()


func fill_with(num):
	maingame_node.cs_board_script_node.FillWith(x, y, num)
	if num > 0:
		candidates_node.visible = false
		answer_node.visible = true
		answer_label_node.text = str(num)
		if GlobalSettings.show_errors and not maingame_node.cs_board_script_node.CheckCorrect(x, y, num):
			answer_label_node.add_theme_color_override("font_color", Color(1, 0, 0))
		maingame_node.cs_board_script_node.Eliminate()
	else:
		candidates_node.visible = true
		answer_node.visible = false
		answer_label_node.text = ""


func eliminate(num):
	candidates_node.get_node("Digit%d" % num).hide_text()


func reset_candidates():
	for i in range(1, 10):
		candidates_node.get_node("Digit%d" % i).show_text()
