using Godot;

public class RotateableSprite : MeshInstance
{
	[Export]
	Texture front, frontLeft, frontRight, back, backLeft, backRight, left, right;

	Spatial parent;
	SpatialMaterial material;

	public override void _Ready()
	{
		parent = GetParentSpatial();
		material = (SpatialMaterial)GetSurfaceMaterial(0);
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector3 dir = parent.Transform.origin - GetViewport().GetCamera().Transform.origin;
		dir.y = 0f;

		float angle = Mathf.Rad2Deg(parent.Transform.basis.z.AngleTo(dir));
		float sign = Mathf.Sign(Vector3.Up.Dot(parent.Transform.basis.z.Cross(dir)));
		float signed = angle * sign; // (signedAngle + 180) % 360

		if (signed >= -22.5f && signed <= 22.5f)
			material.AlbedoTexture = front;
		else if (signed < -22.5f && signed > -67.5f)
			material.AlbedoTexture = frontLeft;
		else if (signed > 22.5f && signed < 67.5f)
			material.AlbedoTexture = frontRight;
		else if (signed <= -67.5f && signed >= -112.5f)
			material.AlbedoTexture = left;
		else if (signed >= 67.5f && signed <= 112.5f)
			material.AlbedoTexture = right;
		else if (signed < -112.5f && signed > -157.5f)
			material.AlbedoTexture = backLeft;
		else if (signed > 112.5f && signed < 157.5f)
			material.AlbedoTexture = backRight;
		else if (signed >= 157.5f || signed <= -157.5f)
			material.AlbedoTexture = back;
	}
}
