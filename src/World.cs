using Godot;
using static Godot.GD;

public class World : Spatial
{
	public static Player player;

	public override void _Ready()
	{
		Randomize();
		Node environment = GetNode("Environment");

		PopulateTrees(environment);
		PopulateMobs();
	}

	void PopulateTrees(Node parent)
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>("res://src/env/Tree.tscn");

		SpatialMaterial[] trees = new SpatialMaterial[8] {
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_0.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_1.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_2.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_3.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_dead_0.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_dead_1.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_dead_2.tres"),
			ResourceLoader.Load<SpatialMaterial>("res://assets/env/tree_dead_3.tres")
		};

		for (int i = 0; i < 2500; i++)
		{
			StaticBody tree = (StaticBody)scene.Instance();
			tree.Translation = new Vector3((float)RandRange(-50f, 50f), 0f, (float)RandRange(-50f, 50f));

			bool tiny = Randf() <= 0.015f;
			tree.Scale = Vector3.One * (float)RandRange(1f, 1.5f) * (tiny ? 0.5f : 1f);

			MeshInstance mesh = tree.GetNode<MeshInstance>("MeshInstance");
			bool dead = Randf() <= 0.15f;
			mesh.MaterialOverride = trees[Randi() % 4 + (dead ? 4 : 0)];

			AddChildBelowNode(parent, tree);
		}
	}

	void PopulateMobs()
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>("res://src/mobs/Mob.tscn");

		for (int i = 0; i < 70; i++)
		{
			Mob mob = (Mob)scene.Instance();
			mob.Translation = new Vector3((float)RandRange(-50f, 50f), 0f, (float)RandRange(-50f, 50f));
			mob.RotationDegrees = Vector3.Up * (Randi() % 360);

			AddChild(mob);
		}
	}
}
