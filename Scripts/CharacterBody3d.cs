using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	[Export]
	private AudioStreamPlayer JumpingSound;
	[Export]
	private AudioStreamPlayer RandomJumpSound1;

	[Export]
	private Node3D CameraPivot;
	[Export]
	private Node3D Character;
	[Export]
	SpringArm3D springArm3D;

	Vector2 CameraInputDirection;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 6f;

	public const float TiltLimitUp = 90F;
	public const float TiltLimitDown = -90F;

	bool MouseDown = false;

    public override void _Ready()
    {
        JumpingSound.Playing = false;
    }

    public override void _Input(InputEvent @event)
    {
		if(MouseDown)
		{
			if(@event is InputEventMouseMotion motion)
			{
				Vector3 rotation = CameraPivot.Rotation;
				rotation.X -= motion.Relative.Y * 0.005F;
				rotation.X = Math.Clamp(rotation.X, -Mathf.DegToRad(75), Mathf.DegToRad(75));
				rotation.Y += -motion.Relative.X * 0.005F;
				CameraPivot.Rotation = rotation;
			}
		}
    } 

    public override void _Process(double delta)
    {
		if(Input.IsMouseButtonPressed(MouseButton.Right))
		{
			MouseDown = true;
		}
		else
		{
			MouseDown = false;
		}

        if(Input.IsActionPressed("ui_left"))
		{
			CameraPivot.RotateY(1F * (float)delta);
		}
		if(Input.IsActionPressed("ui_right"))
		{
			CameraPivot.RotateY(-1F * (float)delta);
		}

	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			Console.WriteLine(delta);
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			JumpingSound.Play();
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Rotated(Vector3.Up, CameraPivot.Rotation.Y);
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		Character.LookAt(GlobalPosition + direction);
		MoveAndSlide();
	}
}
