using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class GameCamera : CharacterBody3D
{
	private Vector3 _rotation;
	private Vector3 _startPos;

	private double minFOV = 30;
	private double maxFOV = 90;
	private double zoomSpeed = 30;

	private Camera3D _cam;
	public override void _Ready()
	{
		_rotation = Rotation;
		_cam = GetNode<Camera3D>("Camera3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		_CameraMovement(delta);
		_startPos = Position;

		if (Input.IsActionPressed("zoom_up"))
		{
			ZoomIn();
		}

		// Zoom out
		if (Input.IsActionPressed("zoom_down"))
		{
			ZoomOut();
		}
	}
	public void _CameraMovement(double delta)
	{
		Vector3 fMovement = Basis.X * Input.GetAxis("ui_down", "ui_up");
		Vector3 rMovement = Basis.Z * Input.GetAxis("ui_left", "ui_right");

		Vector3 movement = fMovement + rMovement;


		float rot = 0f;
		if (Input.IsActionJustPressed("move_rotleft")) rot -= MathF.PI / 4f;
		if (Input.IsActionJustPressed("move_rotright")) rot += MathF.PI / 4f;

		_rotation = _rotation + new Vector3(0f, rot, 0f);

		Quaternion current = new Quaternion(Basis);
		Quaternion mod = Quaternion.FromEuler(_rotation);
		mod = mod.Normalized();
		current = current.Normalized();

		current = current.Slerp(mod, 0.1f);
		Rotation = current.GetEuler();

		movement *= 4f;
		Velocity = Velocity.Lerp(movement, 0.1f);
		MoveAndSlide();
	}
	private void ZoomIn()
	{
		// Decrease the Field of View (FOV) within the specified range
		_cam.Fov = (float)Mathf.Clamp(_cam.Fov - zoomSpeed * GetProcessDeltaTime(), minFOV, maxFOV);
	}

	private void ZoomOut()
	{
		// Increase the Field of View (FOV) within the specified range
		_cam.Fov = (float)Mathf.Clamp(_cam.Fov + zoomSpeed * GetProcessDeltaTime(), minFOV, maxFOV);
	}
}
