using Godot;

public class WeaponTable : Node
{
	public static PackedScene sword, axe;

	public override void _Ready()
	{
		sword = ResourceLoader.Load<PackedScene>("res://src/weapons/Sword.tscn");
		axe = ResourceLoader.Load<PackedScene>("res://src/weapons/Axe.tscn");
	}
}
