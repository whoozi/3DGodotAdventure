using Godot;

public class Weapon : Sprite
{
	public AnimationPlayer anim;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(float delta)
	{
		if (anim.CurrentAnimation.Empty())
			anim.Play("idle");
	}

	public void Use()
	{
		anim.Play("swing_left");

		if (anim.CurrentAnimationPosition / anim.CurrentAnimationLength > 0.5f)
		{
			anim.ClearQueue();
			anim.Queue("swing_left");
		}
	}
}
