using Godot;
using static Godot.GD;

public class World : Spatial
{
	public override void _Ready()
	{
		Node environment = GetNode("Environment");
		PackedScene tree0 = (PackedScene)ResourceLoader.Load("res://tree_0.tscn");
		PackedScene tree1 = (PackedScene)ResourceLoader.Load("res://tree_1.tscn");
		PackedScene tree2 = (PackedScene)ResourceLoader.Load("res://tree_2.tscn");
		PackedScene tree3 = (PackedScene)ResourceLoader.Load("res://tree_3.tscn");
		StaticBody newTree;

		for (int i = 0; i < 5000; i++)
		{
			int tree = Mathf.Abs((int)Randi() % 4);
			switch (tree)
			{
				case 1:
					newTree = (StaticBody)tree1.Instance();
					break;
				case 2:
					newTree = (StaticBody)tree2.Instance();
					break;
				case 3:
					newTree = (StaticBody)tree3.Instance();
					break;
				default:
					newTree = (StaticBody)tree0.Instance();
					break;
			}
			newTree.Translation = new Vector3((float)RandRange(-50f, 50f), 0f, (float)RandRange(-50f, 50f));
			// float scale = (float)RandRange(5f, 7f);
			// newTree.MaterialOverride.Set("Size", new Vector2(scale, scale));
			// newTree.Frame = Mathf.Abs((int)Randi() % 4);
			AddChildBelowNode(environment, newTree);
		}
	}
}
