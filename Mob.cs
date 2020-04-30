using Godot;

public class Mob : KinematicBody
{
	const float Gravity = -9.8f;

	[Export] protected float speed = 3f;

	protected Vector3 velocity;

	public override void _PhysicsProcess(float delta)
	{
		velocity.y += Gravity * delta;
		MoveAndSlide(velocity, Vector3.Up, true);

		if (IsOnFloor())
			velocity.y = 0f;
	}
}
