extends PanelContainer

@onready var digit = get_index() + 1
@onready var subgrid_node: Control = get_node("../..")
var has_text = false
@onready var label_node: Label = get_node("%Label")


# Called when the node enters the scene tree for the first time.
func _ready():
	show_text()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_click_digit(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_RIGHT:
				toggle_label_text()
				accept_event()
				return
			if event.button_index == MOUSE_BUTTON_LEFT and event.double_click:
				if has_text:
					subgrid_node.fill_with(digit)
					accept_event()
					return
			print_debug("clicked on candidate ", digit)
			accept_event()


func toggle_label_text():
	if has_text:
		hide_text()
	else:
		show_text()


func hide_text():
	has_text = false
	label_node.text = ""


func show_text():
	has_text = true
	label_node.text = str(digit)
