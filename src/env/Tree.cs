using Godot;

public class Tree : StaticBody, IDamageable
{
	public void AttemptDamage()
	{
		QueueFree();
	}
}
