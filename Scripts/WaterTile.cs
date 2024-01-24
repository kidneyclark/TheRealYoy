using Godot;
using System;

[Tool]
public partial class WaterTile : Node3D
{
	public static FastNoiseLite Noise;
	public override void _Ready()
	{
		if (Noise == null) 
		{
			Noise = new FastNoiseLite();
			Noise.Frequency = 0.05f;
		}
	}

	public override void _Process(double delta)
	{
		if (Noise == null) return;
		Vector3 p = Position;
		float t = Time.GetTicksMsec() / 1000f;
		t *= 2f;
		float f = (Noise.GetNoise2D(Position.X + t, Position.Z) + 1f) * 0.5f;		
		p.Y = f * 2f;
		Position = p;
	}
}
