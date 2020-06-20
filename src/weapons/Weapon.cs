using Godot;
using static Godot.GD;

public class Weapon : Sprite
{
	AnimationPlayer anim;
	RayCast ray;

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
		this.ray = ray;
	}

	protected virtual void Anim_Hit()
	{
		if (ray == null)
			return;

		ray.CastTo = Vector3.Forward;
		ray.ForceRaycastUpdate();

		if (ray.IsColliding() && ray.GetCollider() is IDamageable)
			(ray.GetCollider() as IDamageable).AttemptDamage();
	}
}
