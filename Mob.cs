using Godot;

public class Mob : KinematicBody
{
	const float Gravity = -9.8f;

	[Export] protected float speed = 3f;

	protected Vector3 velocity, movement;

	public override void _Process(float delta)
	{
		velocity.y += Gravity * delta;
		movement = MoveAndSlide(velocity, Vector3.Up, true);

		if (IsOnFloor())
			velocity.y = Gravity;
	}
}
