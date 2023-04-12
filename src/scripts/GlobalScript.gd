extends Node


func idx_to_xy(i, j):
	return [i / 3 * 3 + j / 3,
			i % 3 * 3 + j % 3]
