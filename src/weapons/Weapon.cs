using Godot;
using static Godot.GD;

public class Weapon : Sprite
{
	AnimationPlayer anim;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(float delta)
	{
		if (anim.CurrentAnimation.Empty())
			anim.Play("idle");
	}

	public virtual bool CanUse => anim.CurrentAnimation != "swing_left";

	public virtual bool CanBeQueued => !CanUse && anim.CurrentAnimationPosition / anim.CurrentAnimationLength > 0.5f;

	public virtual void AttemptUse(RayCast ray, ref bool queueUse)
	{
		if (!CanUse)
		{
			queueUse = CanBeQueued;
			return;
		}

		queueUse = false;
		anim.Play("swing_left");

		ray.CastTo = Vector3.Forward * 2f;
		ray.ForceRaycastUpdate();

		if (ray.IsColliding())
			Print((ray.GetCollider() as Node).Name, ray.GetCollisionPoint());
	}
}
