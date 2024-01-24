using Godot;
using System;

public class TerrainDataGenerator
{
	public static float[,] Generate(Vector2I size, float offset, float scale)
	{
		FastNoiseLite generator = new FastNoiseLite();

		generator.Offset = new Vector3(offset, offset, offset);
		generator.Frequency = scale;
		
		float[,] noise = new float[size.X, size.Y];	

		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++)
			{
				noise[x, y] = generator.GetNoise2D((float)x, (float)y);	
				noise[x, y] = (noise[x, y] + 1f) * 0.5f;
				Vector2 posFromCenter = new Vector2(x - size.X/2, y - size.Y/2);
				float islandFactor = posFromCenter.Length();
				float minside = Mathf.Min(size.X, size.Y);
				float f = -Mathf.Pow(islandFactor/(minside/2f), 2f) + 1f;
				noise[x, y] = noise[x, y] * f;
			}
		}

		return noise;
	}
}

[Tool]
public partial class MapManager : Node3D
{
	[Export] public bool Generate 
	{
		get { return false; }
		set { GenerateMap(); }
	}

	[Export] public MapGenerationData MapGenData;

	private float[,] _noise;

	public override void _Ready()
	{
		MapGenData = ResourceLoader.Load<MapGenerationData>("res://Resources/MapGenData.tres"); 
		GenerateMap();
	}

	public override void _Process(double delta)
	{
		if (MapGenData == null)
			MapGenData = ResourceLoader.Load<MapGenerationData>("res://Resources/MapGenData.tres"); 
	}

	public Vector3 WorldPosFromTilePos(Vector2I tilePos, float hexHeight)
    {
        int column = tilePos.X;
        int row = tilePos.Y;
        float xPosition = 0;
        float yPosition = 0;
        float size = 0.5f;

		bool shouldOffset = (column % 2) == 0;
        float width = 2f * (size + 0.05f);
        float height = Mathf.Sqrt(3f) * (size + 0.05f);

        float horizontalDistance = width * (3f /4f);
        float verticalDistance = height;

        float offset = (shouldOffset) ? height/2 : 0;
        xPosition = (column * horizontalDistance);
        yPosition = (row * verticalDistance) - offset; 

        return new Vector3(xPosition, hexHeight / 2, -yPosition);
    }
	
	public void GenerateMap()
	{
		if (MapGenData == null) return;
		_noise = TerrainDataGenerator.Generate(MapGenData.GridSize, 0f, MapGenData.NoiseScale); 
		
		// Kill children
		Godot.Collections.Array<Node> children = GetChildren();
		foreach (Node c in children) c.QueueFree();

		for (int x = 0; x < MapGenData.GridSize.X; x++)
		{
			for (int y = 0; y < MapGenData.GridSize.Y; y++)
			{
				float elevation = _noise[x, y];
				elevation *= 8f;
				Node3D tile;
				if (elevation < 2f) 
				{
					elevation = 1f;
					tile = MapGenData.WaterTilePrefab.Instantiate<Node3D>();
				}
				else
				{
					tile = MapGenData.GroundTilePrefab.Instantiate<Node3D>();
				}
				tile.Position = WorldPosFromTilePos(new Vector2I(x, y), elevation);
				Vector3 scale = tile.Scale;
				scale.Y = elevation;
				tile.Scale = scale;

				AddChild(tile);
			}
		}
	}
}
