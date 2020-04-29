using Godot;

public class Player : Mob
{
	const float MouseSensitivity = 0.25f;
	const float JoySensitivity = 2f;

	Vector2 mouseDelta;
	float swayDegrees;
	AnimationPlayer animation;
	Camera camera;
	CanvasLayer canvas;

	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<Camera>("WorldCamera");
		canvas = GetNode<CanvasLayer>("CanvasLayer");

		Input.SetMouseMode(Input.MouseMode.Captured);
	}

	public override void _Process(float delta)
	{
		Vector2 move = new Vector2(
		  Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
		  Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
		).Clamped(1f);

		Vector2 look = mouseDelta * MouseSensitivity;
		look += new Vector2(
		  Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
		  Input.GetActionStrength("look_backward") - Input.GetActionStrength("look_forward")
		) * JoySensitivity;
		mouseDelta = Vector2.Zero;

		// Rotate player/camera
		RotationDegrees -= new Vector3(0, look.x, 0);
		camera.RotationDegrees = camera.RotationDegrees.X(Mathf.Clamp(camera.RotationDegrees.x - look.y, -75, 75));

		// Perform movement
		velocity = new Vector3(move.x, 0, move.y).Rotated(Vector3.Up, Rotation.y) * speed + (velocity.y * Vector3.Up);

		// Animate canvas
		canvas.Offset = new Vector2(
		  Mathf.Lerp(canvas.Offset.x - look.x, 0, 20f * delta),
		  Mathf.Lerp(canvas.Offset.y, Mathf.Max(0, camera.RotationDegrees.x), 20f * delta)
		);

		if (IsOnFloor())
		{
			if (move != Vector2.Zero)
			{
				swayDegrees += 6f * delta;
				canvas.Offset += new Vector2(Mathf.Sin(swayDegrees) * 10f, 2f + (Mathf.Cos(swayDegrees * 2f) * 2f));
			}

			if (Input.IsActionJustPressed("attack"))
			{
				animation.Play("swing_left");

				if (animation.CurrentAnimationPosition / animation.CurrentAnimationLength >= 0.5f)
				{
					animation.ClearQueue();
					animation.Queue("swing_left");
				}
			}
		}

		if (animation.CurrentAnimation.Empty())
			animation.Play("idle");

		base._Process(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
			mouseDelta = motion.Relative;

		if (@event is InputEventKey key && key.Pressed)
			switch ((KeyList)key.Scancode)
			{
				case KeyList.Escape:
					GetTree().Quit();
					break;
				case KeyList.F11:
					OS.WindowFullscreen = !OS.WindowFullscreen;
					break;
				case KeyList.F9:
					GetTree().ReloadCurrentScene();
					break;
			}
	}
}

namespace Godot
{
	public static class Vector3Ext
	{
		/// <summary>Returns Vector3 with modified x.</summary>
		public static Vector3 X(this Vector3 vec3, float x)
		{
			return new Vector3(x, vec3.y, vec3.z);
		}
	}
}