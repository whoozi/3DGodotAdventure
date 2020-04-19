class_name Mob
extends KinematicBody

const GRAVITY: float = -9.8

export var speed: float = 2

var velocity: Vector3


func _process(delta: float) -> void:
	velocity.y += GRAVITY * delta
	move_and_slide(velocity, Vector3.UP, true)

	if is_on_floor():
		velocity.y = 0
