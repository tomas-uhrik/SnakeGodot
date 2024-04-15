using Godot;
using System;

public partial class game_over_menu : CanvasLayer
{
	[Signal]
	public delegate void RestartGameEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Metoda pro obsluhu kliknutí na tlačítko "Restart"
	private void _on_restart_button_pressed()
	{
		EmitSignal(nameof(RestartGame));
	}
}


