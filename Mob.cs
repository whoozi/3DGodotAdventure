using Godot;
using static Godot.GD;

public class Mob : KinematicBody
{
	const float Gravity = -9.8f;

	[Export] protected float speed = 3f;

	protected Vector3 velocity;

	public override void _PhysicsProcess(float delta)
	{
		velocity.y += Gravity * delta;

		if (IsOnFloor())
			velocity.y = Gravity;
	}

	protected virtual Vector3 PerformMovement(Vector3 movement)
	{
		return MoveAndSlide(movement, Vector3.Up, true);
	}
}
