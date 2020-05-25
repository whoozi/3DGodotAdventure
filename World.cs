using Godot;
using static Godot.GD;

public class World : Spatial
{
	public override void _Ready()
	{
		Node environment = GetNode("Environment");
		PackedScene tree = (PackedScene)ResourceLoader.Load("res://tree.tscn");
		Sprite3D newTree;

		for (int i = 0; i < 500; i++)
		{
			newTree = (Sprite3D)tree.Instance();
			newTree.Translation = new Vector3(Randf() * 50, 0, Randf() * 50);
			float scale = (float)RandRange(1.5f, 3f);
			newTree.Scale = new Vector3(scale, scale, 1f);
			newTree.Frame = Mathf.Abs((int)Randi() % 4);
			AddChildBelowNode(environment, newTree);
		}
	}
}
