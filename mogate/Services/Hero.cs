using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public enum HeroState
	{
		Idle,
		Moving
	};

	public interface IHero
	{
		Entity Player { get; }
	};

	public class Hero : GameComponent, IHero
	{
		Entity m_player;

		public Hero (Game game) : base(game)
		{
			m_player = new Entity ();
		}

		public override void Update(GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
	
			if (gameState.State == EState.LevelCreated) {
				Init();
				gameState.State = EState.HeroCreated;
			}

			if (gameState.State == EState.GameStarted) {
				UpdateHero ();
				Player.Get<Execute> ().Update (gameTime);
			}

			base.Update (gameTime);
		}

		public Entity Player { get { return m_player; } }

		private void Init ()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			Player.Register (new State<HeroState> (HeroState.Idle));
			Player.Register (new Health (200));
			Player.Register (new Attack (30));
			Player.Register (new Attackable (OnAttacked));
			Player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			Player.Register (new Execute ());
			Player.Register (new Drawable (sprites.GetSprite ("hero"), "hero_idle",
				new Vector2(mapGrid.StairDown.X*Globals.CELL_WIDTH, mapGrid.StairDown.Y*Globals.CELL_HEIGHT)));

			StartIdle (Player);
		}

		private void UpdateHero()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			if (Player.Get<State<HeroState>>().EState == HeroState.Idle) {
				Point newPos = m_player.Get<Position>().MapPos;

				if (Keyboard.GetState ().IsKeyDown (Keys.Up))
					newPos.Y--;
				else if (Keyboard.GetState ().IsKeyDown (Keys.Down))
					newPos.Y++;
				else if (Keyboard.GetState ().IsKeyDown (Keys.Left))
					newPos.X--;
				else if (Keyboard.GetState ().IsKeyDown (Keys.Right))
					newPos.X++;

				var mt = mapGrid.GetID (newPos.X, newPos.Y);
				if (mt != MapGridTypes.ID.Blocked && newPos != m_player.Get<Position> ().MapPos) {
					var seq = new Sequence ();
					var spawn = new Spawn ();
					spawn.Add (new MoveSpriteTo (Player, new Vector2 (newPos.X * Globals.CELL_WIDTH, newPos.Y * Globals.CELL_HEIGHT), 300));
					spawn.Add (new AnimSprite (Player, "hero_move", 300));
					seq.Add (spawn);
					seq.Add (new ActionEntity (Player, (_) => {
						Player.Get<Position> ().MapPos = newPos;
					}));
					seq.Add (new ActionEntity (Player, OnEndMove));

					Player.Get<Execute> ().AddNew (seq, "movement");
					Player.Get<State<HeroState>> ().EState = HeroState.Moving;
				}
			}
		}
		
		private void OnEndMove(Entity hero)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			var mt = mapGrid.GetID (Player.Get<Position>().MapPos.X, Player.Get<Position>().MapPos.Y);
			if (mt == MapGridTypes.ID.StairUp) {
				gameState.NextLevel ();
			}
			StartIdle (hero);
		}

		private void OnAttacked(Entity attacker)
		{
			var effects = (IEffects)Game.Services.GetService (typeof(IEffects));
			effects.SpawnEffect (Player, "damage", 400);
		}

		private void StartIdle(Entity hero)
		{
			var loop = new Loop (new AnimSprite (Player, "hero_idle", 600));

			Player.Get<Execute> ().AddNew (loop, "movement");
			Player.Get<State<HeroState>>().EState = HeroState.Idle;
		}
	}
}

