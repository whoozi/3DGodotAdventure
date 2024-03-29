using Godot;

public class Mob : KinematicBody, IDamageable
{
	const float Gravity = -9.8f;

	[Export] protected float speed = 3f;

	protected Vector3 velocity;

	public virtual void AttemptDamage()
	{
		throw new System.NotImplementedException();
	}

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
