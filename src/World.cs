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

		Texture[] trees = new Texture[2] {
			ResourceLoader.Load<Texture>("res://assets/env/tree.png"),
			ResourceLoader.Load<Texture>("res://assets/env/tree_dead.png")
		};

		for (int i = 0; i < 2500; i++)
		{
			StaticBody tree = (StaticBody)scene.Instance();
			tree.Translation = new Vector3((float)RandRange(-50f, 50f), 0f, (float)RandRange(-50f, 50f));

			bool tiny = Randf() <= 0.015f;
			tree.Scale = Vector3.One * (float)RandRange(1f, 1.5f) * (tiny ? 0.5f : 1f);

			Sprite3D sprite = tree.GetNode<Sprite3D>("Sprite3D");
			bool dead = Randf() <= 0.15f;
			sprite.Texture = trees[dead ? 1 : 0];
			sprite.Frame = Mathf.Abs((int)Randi() % 4);

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
