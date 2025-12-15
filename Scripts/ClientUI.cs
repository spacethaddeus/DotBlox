using Godot;
using System;

public partial class ClientUI : Control
{
	[Export]
	Button HelpButton;
	[Export]
	Button ToolsButton;
	[Export]
	Button InsertButton;
	[Export]
	Button FullscreenButton;
	[Export]
	Button ExitButton;

	bool FullscreenMode = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FullscreenButton.Pressed += Fullscreen;
		ExitButton.Pressed += Exit;
	}

	private void Fullscreen()
	{
		if(!FullscreenMode)
		{
			FullscreenMode = true;
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

			FullscreenButton.Text = "x Fullscreen";
		}
		else
		{
			FullscreenMode = false;
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

			FullscreenButton.Text = "  Fullscreen";
		}
	}

	private void Exit()
	{
		GetTree().Quit();
	}
}
