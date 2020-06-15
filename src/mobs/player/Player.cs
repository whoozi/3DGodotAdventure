using Godot;

public class Player : Mob
{
	const float MouseSensitivity = 1f;
	const float JoySensitivity = 2f;

	Vector2 mouseDelta, lookInput, moveInput;
	float canvasWalkDegrees;
	Vector3 lastOrigin, currentOrigin;
	bool isOriginDirty;
	Transform initialCamera;

	Camera camera;
	CanvasLayer canvas;
	Weapon weapon;

	private bool IsMouseCaptured() => Input.GetMouseMode() == Input.MouseMode.Captured;

	// Used by the camera to reduce stutter caused by fixed physics update loop
	private void RefreshOrigin()
	{
		isOriginDirty = false;

		lastOrigin = currentOrigin;
		currentOrigin = Transform.origin;
	}

	public override void _Ready()
	{
		camera = GetNode<Camera>("WorldCamera");
		canvas = GetNode<CanvasLayer>("CanvasLayer");
		// weapon = (Weapon)WeaponTable.sword.Instance();
		// canvas.GetNode("Control").AddChild(weapon);

		initialCamera = camera.Transform;
		camera.SetAsToplevel(true);

		Input.SetMouseMode(Input.MouseMode.Captured);
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		moveInput = new Vector2(
			Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
		).Clamped(1f);

		velocity = PerformMovement(new Vector3(moveInput.x, 0, moveInput.y).Rotated(Vector3.Up, camera.Rotation.y) * speed + (velocity.y * Vector3.Up));

		if (isOriginDirty)
			RefreshOrigin();

		isOriginDirty = true;
	}

	public override void _Process(float delta)
	{
		if (isOriginDirty)
			RefreshOrigin();

		lookInput = mouseDelta * MouseSensitivity * (GetViewport().Size * 0.0001f);
		lookInput.y *= GetViewport().Size.x / GetViewport().Size.y;
		mouseDelta = Vector2.Zero;

		lookInput += new Vector2(
			Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
			Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up")
		) * JoySensitivity * delta * 100f;

		// Rotate player and camera
		Rotate(Vector3.Up, Mathf.Deg2Rad(-lookInput.x));
		camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.x - Mathf.Deg2Rad(lookInput.y), -75, 75), Rotation.y, 0f);
		// Position camera; apply physics interpolation fraction to reduce stutter caused by fixed physics loop
		camera.Translation = lastOrigin + ((currentOrigin - lastOrigin) * Engine.GetPhysicsInterpolationFraction()) + initialCamera.origin;

		// Lerp canvas back to center
		if (weapon != null)
			canvas.Offset = new Vector2(
				Mathf.Lerp(canvas.Offset.x - lookInput.x, 0, 20f * delta),
				Mathf.Lerp(canvas.Offset.y, Mathf.Max(0, camera.RotationDegrees.x), 20f * delta)
			);
		else canvas.Offset = new Vector2(0f, 576f);

		if (IsOnFloor())
		{
			// Do canvas "walking" animation
			if (!moveInput.IsEqualApprox(Vector2.Zero))
			{
				canvasWalkDegrees += 6f * (velocity.Length() / speed) * delta;
				canvas.Offset += new Vector2(Mathf.Sin(canvasWalkDegrees) * 10f, 2f + (Mathf.Cos(canvasWalkDegrees * 2f) * 2f)) * delta * 100f;
			}

			if (weapon != null & Input.IsActionJustPressed("attack"))
				weapon.Use();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
			if (IsMouseCaptured())
				mouseDelta = motion.Relative;

		if (@event is InputEventKey key && key.Pressed)
			switch ((KeyList)key.Scancode)
			{
				case KeyList.Escape:
					Input.SetMouseMode(IsMouseCaptured() ? Input.MouseMode.Visible : Input.MouseMode.Captured);
					break;
				case KeyList.F11:
					OS.WindowFullscreen = !OS.WindowFullscreen;
					break;
				case KeyList.F9:
					GetTree().ReloadCurrentScene();
					break;
				case KeyList.Key1:
					if (weapon != null) weapon.QueueFree();
					weapon = (Weapon)WeaponTable.sword.Instance();
					canvas.GetNode("Control").AddChild(weapon);
					canvas.Offset = new Vector2(0f, 576f);
					break;
				case KeyList.Key2:
					if (weapon != null) weapon.QueueFree();
					weapon = (Weapon)WeaponTable.axe.Instance();
					canvas.GetNode("Control").AddChild(weapon);
					canvas.Offset = new Vector2(0f, 576f);
					break;
			}
	}
}
