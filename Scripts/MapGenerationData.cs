using Godot;
using System;
using System.Runtime.InteropServices;

[GlobalClass, Tool]
public partial class MapGenerationData : Resource
{
	[Export] public Vector2I GridSize = new Vector2I(16, 16);
	[Export] public float NoiseScale = 0.01f;
	[Export] public PackedScene GroundTilePrefab;
	[Export] public PackedScene WaterTilePrefab;
	[Export] public PackedScene SandTilePrefab;
	[Export] public PackedScene SnowTilePrefab;
}
