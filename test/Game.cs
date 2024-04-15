using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	[Export]
	private PackedScene snakeScene;
	
	private int score = 0;
	private bool gameOver = false;
	private Vector2 _snakeDirection; // Snake starts moving to the right
	private Godot.Collections.Array oldData = new Godot.Collections.Array();
	private Godot.Collections.Array snakeData = new Godot.Collections.Array();
	private Godot.Collections.Array snake = new Godot.Collections.Array();
	private Vector2 berryPosition;
	private Random random = new Random();
	private Vector2 startPos = new Vector2(9,9);
	private Label scoreLabel;
	private Sprite2D berrySprite; // Variable for the berry sprite
	private Timer timer;
	private CanvasLayer menu;
	// Settings for the game
	private const int Cells = 20;
	private const int CellSize = 50; // Assuming each cell is 50px by 50px
	// Height in cells, excluding score area

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		NewGame();
	}

	public void NewGame(){
		score = 0;
		_snakeDirection = Vector2.Up;
		GetTree().CallGroup("segments","queue_free");
		menu = GetNode<CanvasLayer>("GameOverMenu");
		menu.Hide();
		berrySprite = GetNode<Sprite2D>("Berry");
		scoreLabel = GetNode<Label>("HUD/ScoreLabel");
		GenerateSnake();
		PlaceBerryRandomly();
		UpdateScoreLabel();
		StartGame();
	}


	public void GenerateSnake()
	{
		oldData.Clear();
		snakeData.Clear();
		snake.Clear();
		for(int i = 0; i < 3; i++){
			AddSegment(startPos + new Vector2(0, i));
		}
	}
	
	public void AddSegment(Vector2 pos)
	{
		snakeData.Add(pos);
		var snakeSegment = (Panel)snakeScene.Instantiate();
		snakeSegment.Position  = (pos * CellSize) + new Vector2(0, CellSize);
		AddChild(snakeSegment);
		snake.Add(snakeSegment);
		
	}
	
	private void PlaceBerryRandomly(){
		berryPosition = GenerateRandomPosition();
		if (IsBerrySpawnCollidedWith(snakeData)){
			PlaceBerryRandomly();
		}else{
			SetBerrySpritePosition(berryPosition);
			SetBerrySpriteTexture("res://Berry.png");	
		}
	}
	
	private bool IsBerrySpawnCollidedWith(Godot.Collections.Array data){
		return data.Contains(berryPosition);	
	}
	
	private Vector2 GenerateRandomPosition(){
		int berryX = random.Next(0, Cells-1);
		int berryY = random.Next(0, Cells-1);
		return new Vector2(berryX, berryY);
	}
	
	private void SetBerrySpritePosition(Vector2 berryPosition){
		berrySprite.Position = (berryPosition * CellSize) + new Vector2(0, CellSize);
	}
	
	private void SetBerrySpriteTexture(string TexturePath){
		berrySprite.Texture = (Texture2D)GD.Load(TexturePath);
	}

	private void UpdateScoreLabel(){
		scoreLabel.Text = "SCORE: " + score;
	}
	
	public void StartGame(){
		timer = GetNode<Timer>("MoveTimer");
		timer.Start();
	}
	
	private void _on_move_timer_timeout(){
		MoveSnake();
		CheckIfBerryEaten();
		CheckCollision();
		if(gameOver){
			EndGame();
		}
	}

	
	private void MoveSnake(){
		oldData = new Godot.Collections.Array(snakeData);
		snakeData[0] = (Vector2)snakeData[0] + _snakeDirection;
		((Panel)snake[0]).Position = ((Vector2)snakeData[0] * CellSize) + new Vector2(0, CellSize);
		for (int i = 1; i < snakeData.Count; i++)
		{
				snakeData[i] = oldData[i-1];
				((Panel)snake[i]).Position = ((Vector2)snakeData[i] * CellSize) + new Vector2(0, CellSize);
		}
	}
	
	private void CheckIfBerryEaten(){
		if((Vector2)snakeData[0] == berryPosition){
			score += 1;
			UpdateScoreLabel();
			AddSegment((Vector2)oldData[oldData.Count-1]);
			PlaceBerryRandomly();
		}
	}
	
	private void CheckCollision(){
		if(CheckCollisionWithWalls() || CheckSelfCollision()){
			gameOver = true;
		}
	}

	private bool CheckCollisionWithWalls()
	{
		Vector2 snakeHead = (Vector2)snakeData[0];
		return (snakeHead[0] < 0 || snakeHead[0] >= Cells || snakeHead[1] < 0 || snakeHead[1] >= Cells);
	}

	private bool CheckSelfCollision(){
		Vector2 snakeHead = (Vector2)snakeData[0];
		for (int i = 1; i < snakeData.Count; i++)
		{
			Vector2 currentPart = (Vector2)snakeData[i];
			if (snakeHead == currentPart)
			{
				return true;
			}
		}
		
		return false;
	}
	

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.IsActionPressed("ui_up") && _snakeDirection != Vector2.Down)
				_snakeDirection = Vector2.Up;
			else if (eventKey.IsActionPressed("ui_down") && _snakeDirection != Vector2.Up)
				_snakeDirection = Vector2.Down;
			else if (eventKey.IsActionPressed("ui_left") && _snakeDirection != Vector2.Right)
				_snakeDirection = Vector2.Left;
			else if (eventKey.IsActionPressed("ui_right") && _snakeDirection != Vector2.Left)
				_snakeDirection = Vector2.Right;
		}
	}

	
	private void EndGame(){
		menu.Show();
		timer.Stop();
	}

	private void _on_game_over_menu_restart_game()
	{
		gameOver = false;
		NewGame();
	}	

}






