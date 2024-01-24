using Godot;
using System;

[Tool]
public partial class GameCamera : CharacterBody3D
{
	private Vector3 _rotation;
	public override void _Ready()
	{
		_rotation = Rotation;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 movement = Vector3.Zero;
		movement.X = Input.GetAxis("ui_down", "ui_up"); 
		movement.Z = Input.GetAxis("ui_left", "ui_right"); 
		
		float rot = 0f;
		if (Input.IsActionJustPressed("move_rotleft")) rot -= MathF.PI / 4f;
		if (Input.IsActionJustPressed("move_rotright")) rot += MathF.PI / 4f;

		_rotation = _rotation + new Vector3(0f, rot, 0f);

		Quaternion current = new Quaternion(Basis);
		Quaternion mod = Quaternion.FromEuler(_rotation);
		mod = mod.Normalized();
		current = current.Normalized();

		current = current.Slerp(mod, 0.1f);

		GD.Print(_rotation);
		Rotation = current.GetEuler();

		movement *= 4f;
		Velocity = Velocity.Lerp(movement, 0.1f);
		MoveAndSlide();
	}
}
