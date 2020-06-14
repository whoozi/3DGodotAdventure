using Godot;
using static Godot.GD;

public class World : Spatial
{
	public override void _Ready()
	{
		Randomize();

		Node environment = GetNode("Environment");
		PackedScene scene = ResourceLoader.Load<PackedScene>("res://src/env/Tree.tscn");

		SpatialMaterial[] materials = new SpatialMaterial[4] {
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_0.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_1.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_2.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_3.tres")
		};

		for (int i = 0; i < 2500; i++)
		{
			StaticBody tree = (StaticBody)scene.Instance();

			tree.Translation = new Vector3((float)RandRange(-50f, 50f), 0f, (float)RandRange(-50f, 50f));
			tree.Scale = Vector3.One * (float)RandRange(1f, 1.5f);

			MeshInstance mesh = tree.GetNode<MeshInstance>("MeshInstance");
			mesh.MaterialOverride = materials[Randi() % 4];

			AddChildBelowNode(environment, tree);
		}
	}
}
