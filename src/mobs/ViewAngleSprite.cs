using Godot;

public class ViewAngleSprite : Sprite3D
{
	[Export]
	Texture front, frontLeft, frontRight, back, backLeft, backRight, left, right;

	Spatial parent;

	public override void _Ready()
	{
		parent = GetParentSpatial();
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector3 dir = parent.Transform.origin - GetViewport().GetCamera().Transform.origin;
		dir.y = 0f;

		float angle = Mathf.Rad2Deg(parent.Transform.basis.z.AngleTo(dir));
		float sign = Mathf.Sign(Vector3.Up.Dot(parent.Transform.basis.z.Cross(dir)));
		float signed = angle * sign; // (signedAngle + 180) % 360

		if (signed >= -22.5f && signed <= 22.5f)
			Texture = front;
		else if (signed < -22.5f && signed > -67.5f)
			Texture = frontLeft;
		else if (signed > 22.5f && signed < 67.5f)
			Texture = frontRight;
		else if (signed <= -67.5f && signed >= -112.5f)
			Texture = left;
		else if (signed >= 67.5f && signed <= 112.5f)
			Texture = right;
		else if (signed < -112.5f && signed > -157.5f)
			Texture = backLeft;
		else if (signed > 112.5f && signed < 157.5f)
			Texture = backRight;
		else if (signed >= 157.5f || signed <= -157.5f)
			Texture = back;
	}
}
