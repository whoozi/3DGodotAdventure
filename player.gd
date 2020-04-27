extends Mob

const MOUSE_SENSITIVITY: float = 0.25
const JOY_SENSITIVITY: float = 2.0

var mouse_delta: Vector2
var is_mouse_captured: bool = true
var sway_degree: float

onready var anim_player: AnimationPlayer = $AnimationPlayer
onready var camera: Camera = $WorldCamera
onready var canvas: CanvasLayer = $CanvasLayer


func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)


func _process(delta: float) -> void:
	var move := Vector2(
		Input.get_action_strength("move_right") - Input.get_action_strength("move_left"),
		Input.get_action_strength("move_backward") - Input.get_action_strength("move_forward")
	)
	move = move.clamped(1)

	var look := mouse_delta * MOUSE_SENSITIVITY
	look += (
		Vector2(
			Input.get_action_strength("look_right") - Input.get_action_strength("look_left"),
			Input.get_action_strength("look_down") - Input.get_action_strength("look_up")
		)
		* JOY_SENSITIVITY
	)
	mouse_delta = Vector2.ZERO

	# Rotate player/camera
	rotation_degrees.y -= look.x
	camera.rotation_degrees.x = clamp(camera.rotation_degrees.x - look.y, -75, 75)

	# Perform movement
	velocity = (
		Vector3(move.x, 0, move.y).rotated(Vector3.UP, rotation.y) * speed
		+ (velocity.y * Vector3.UP)
	)

	# Animate canvas
	canvas.offset.x = lerp(canvas.offset.x - look.x, 0, 20 * delta)
	canvas.offset.y = lerp(canvas.offset.y, max(0, camera.rotation_degrees.x), 20 * delta)

	if is_on_floor():
		if move != Vector2.ZERO:
			sway_degree += 6 * delta
			canvas.offset += Vector2(sin(sway_degree) * 10, 2 + (cos(sway_degree * 2) * 2))

		if Input.is_action_just_pressed("attack"):
			anim_player.play("swing")
			if anim_player.current_animation_position / anim_player.current_animation_length >= 0.5:
				anim_player.clear_queue()
				anim_player.queue("swing")

	if anim_player.current_animation == "":
		anim_player.play("idle")

	._process(delta)


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		if is_mouse_captured:
			mouse_delta = event.relative

	if event is InputEventKey:
		if event.pressed:
			match event.scancode:
				KEY_ESCAPE:
					is_mouse_captured = ! is_mouse_captured
					Input.set_mouse_mode(
						Input.MOUSE_MODE_CAPTURED if is_mouse_captured else Input.MOUSE_MODE_VISIBLE
					)
				KEY_F11:
					OS.window_fullscreen = ! OS.window_fullscreen
				KEY_F9:
					get_tree().reload_current_scene()
