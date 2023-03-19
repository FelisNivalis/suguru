extends Control

func _ready():
	GlobalScript.emit_signal("s_restart")
