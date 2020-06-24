using Godot;

public class Mob : KinematicBody, IDamageable
{
	protected const float Gravity = -9.8f;

	[Export]
	protected float speed = 3f;

	protected Vector3 velocity;

	float moveTime;
	Vector3 moveDir;

	AnimationPlayer animation;

	void AIStep(float delta)
	{
		if (World.player == null)
			return;

		Vector3 target = World.player.Transform.origin;

		if ((moveTime -= delta) <= 0f)
		{
			Vector3 dir = target - Transform.origin;
			dir.y = 0f;
			dir = dir.Normalized();

			if (!TestMove(Transform, dir))
			{
				moveTime = GD.Randf();
				moveDir = dir;
			}
			else
			{
				dir = Vector3.Forward.Rotated(Vector3.Up, GD.Randi() % 360f).Normalized();

				if (!TestMove(Transform, dir))
				{
					moveTime = GD.Randf();
					moveDir = dir;
				}
				else moveDir = Vector3.Zero;
			}
		}

		velocity = moveDir * speed + (velocity.y * Vector3.Up);

		if (moveDir != Vector3.Zero)
			LookAt(Transform.origin + moveDir, Vector3.Up);
	}

	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public virtual void AttemptDamage()
	{
		QueueFree();
	}

	public override void _PhysicsProcess(float delta)
	{
		velocity.y += Gravity * delta;

		if (IsOnFloor())
			velocity.y = Gravity;

		AIStep(delta);

		PerformMovement(velocity);

		if (IsOnFloor() && moveDir != Vector3.Zero)
			animation.Play("move");
		else
			animation.Play("idle");
	}

	protected virtual Vector3 PerformMovement(Vector3 movement)
	{
		return MoveAndSlide(movement, Vector3.Up, true);
	}
}
