extends Control

var type = GlobalScript.NodeType.Digit

@onready var digit = get_index() + 1
@onready var maingame_node: Control = get_node("/root/MainGame")
@onready var subgrid_node: Control = get_node("../..")
@onready var label_node: Label = get_node("%DigitLabel")
@onready var content_node: Control = get_node("%Content")
@onready var bg_node: Panel = get_node("%BgCircle")

# Called when the node enters the scene tree for the first time.
func _ready():
	label_node.text = str(digit)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_click_digit(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_RIGHT:
				print_debug("clicked on candidate ", digit)
				if content_node.visible:
					maingame_node.execute(self, "eliminate_candidate")
				else:
					maingame_node.execute(self, "uneliminate_candidate")
				accept_event()
				return
			if event.button_index == MOUSE_BUTTON_LEFT and event.double_click:
				if content_node.visible:
					maingame_node.execute(self, "fill_with")
					accept_event()
					return
			print_debug("clicked on candidate ", digit)
			accept_event()


func hide_text():
	content_node.visible = false


func show_text():
	content_node.visible = true
