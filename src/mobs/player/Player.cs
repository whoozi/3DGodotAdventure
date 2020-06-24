using Godot;

public class Player : Mob
{
	const float MouseSensitivity = 0.4f;
	const float JoySensitivity = 3f;

	Vector2 mouseDelta, lookInput, moveInput;
	float canvasWalkDegrees;
	Vector3 lastOrigin, currentOrigin;
	bool isOriginDirty;
	Vector3 cameraOffset;
	Weapon weapon;
	bool isUseQueued;

	// Nodes
	Camera camera;
	CanvasLayer canvas;
	RayCast ray;

	bool IsMouseCaptured => Input.GetMouseMode() == Input.MouseMode.Captured;

	// Used by the camera to reduce stutter caused by fixed physics update loop
	void RefreshOrigin()
	{
		isOriginDirty = false;

		lastOrigin = currentOrigin;
		currentOrigin = Transform.origin;
	}

	void AnimateCanvas(float delta)
	{
		// Lerp canvas back to center
		if (weapon != null)
			canvas.Offset = new Vector2(
				Mathf.Lerp(canvas.Offset.x - (200f * lookInput.x), 0, 20f * delta),
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
		}
	}

	void ChangeWeapon(Weapon target)
	{
		if (weapon != null)
			weapon.QueueFree();

		weapon = target;
		canvas.GetNode("Control").AddChild(weapon);
		canvas.Offset = new Vector2(0f, 576f);
	}

	public override void _Ready()
	{
		World.player = this;

		camera = GetNode<Camera>("WorldCamera");
		canvas = GetNode<CanvasLayer>("CanvasLayer");
		ray = camera.GetNode<RayCast>("RayCast");

		cameraOffset = camera.Transform.origin;
		camera.SetAsToplevel(true);

		Input.SetMouseMode(Input.MouseMode.Captured);
	}

	public override void _PhysicsProcess(float delta)
	{
		moveInput = new Vector2(
			Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
		).Clamped(1f);

		velocity.y += Gravity * delta;

		if (IsOnFloor())
			velocity.y = Gravity;

		velocity = PerformMovement(
			new Vector3(moveInput.x, 0, moveInput.y).Rotated(Vector3.Up, camera.Rotation.y) * speed + (velocity.y * Vector3.Up));

		if (isOriginDirty)
			RefreshOrigin();

		isOriginDirty = true;
	}

	public override void _Process(float delta)
	{
		if (isOriginDirty)
			RefreshOrigin();

		lookInput = mouseDelta * (GetViewport().Size / OS.GetScreenSize()) * MouseSensitivity / 100f;
		mouseDelta = Vector2.Zero;

		lookInput += new Vector2(
			Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
			Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up")
		) * JoySensitivity * delta;

		// Rotate player and camera
		Rotate(Vector3.Up, -lookInput.x);
		camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.x - lookInput.y, Mathf.Deg2Rad(-75f), Mathf.Deg2Rad(75f)), Rotation.y, 0f);
		// Position camera; apply physics interpolation fraction to reduce stutter caused by fixed physics loop
		camera.Translation = lastOrigin + ((currentOrigin - lastOrigin) * Engine.GetPhysicsInterpolationFraction()) + cameraOffset;

		AnimateCanvas(delta);

		if (IsOnFloor())
		{
			if (weapon != null)
			{
				if (!isUseQueued)
				{
					if (Input.IsActionJustPressed("attack"))
						weapon.AttemptUse(ray, ref isUseQueued);
				}
				else weapon.AttemptUse(ray, ref isUseQueued);
			}
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
			if (IsMouseCaptured)
				mouseDelta = motion.Relative;

		if (@event is InputEventKey key && key.Pressed)
			switch ((KeyList)key.Scancode)
			{
				case KeyList.Escape:
					Input.SetMouseMode(IsMouseCaptured ? Input.MouseMode.Visible : Input.MouseMode.Captured);
					break;
				case KeyList.F11:
					OS.WindowFullscreen = !OS.WindowFullscreen;
					break;
				case KeyList.F9:
					GetTree().ReloadCurrentScene();
					break;
				case KeyList.Key1:
					if (weapon?.CanUse ?? true)
						ChangeWeapon((Weapon)WeaponTable.sword.Instance());
					break;
				case KeyList.Key2:
					if (weapon?.CanUse ?? true)
						ChangeWeapon((Weapon)WeaponTable.axe.Instance());
					break;
			}
	}
}
