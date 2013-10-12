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
	
			if (gameState.State == EState.MapGenerated) {
				Init();
				gameState.State = EState.HeroCreated;
			}

			if (gameState.State == EState.LevelStarted) {
				UpdateHero ();
				Player.Get<Execute> ().Update (gameTime);
			}

			base.Update (gameTime);
		}

		public Entity Player { get { return m_player; } }

		private void Init ()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			Player.Register (new State<HeroState> (HeroState.Idle));
			Player.Register (new Health (200));
			Player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			Player.Register (new Execute ());
			Player.Register (new Drawable (sprites.GetSprite ("hero"), "idle",
			                               new Vector2(mapGrid.StairDown.X*32, mapGrid.StairDown.Y*32)));

			StartIdle (Player);
		}

		private void UpdateHero()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

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
					spawn.Add (new MoveSpriteTo (Player, new Vector2 (newPos.X * 32, newPos.Y * 32), 300));
					spawn.Add (new AnimSprite (Player, "move", 300));
					seq.Add (spawn);
					seq.Add (new ActionEntity (Player, (_) => {
						Player.Get<Position> ().MapPos = newPos;
					}));
					seq.Add (new ActionEntity (Player, OnEndMove));

					Player.Get<Execute> ().Cancel ("idle");
					Player.Get<Execute> ().Start (seq, "move");
					Player.Get<State<HeroState>> ().EState = HeroState.Moving;
				}
			}
		}
		
		private void OnEndMove(Entity hero)
		{
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

			var mt = mapGrid.GetID (Player.Get<Position>().MapPos.X, Player.Get<Position>().MapPos.Y);
			if (mt == MapGridTypes.ID.StairUp) {
				var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
				gameState.State = EState.LevelStarting;
			}
			StartIdle (hero);
		}

		private void StartIdle(Entity hero)
		{
			var seq = new Sequence ();
			seq.Add (new AnimSprite (Player, "idle", 600));
			seq.Add (new ActionEntity (Player, StartIdle));

			Player.Get<Execute> ().Start (seq, "idle");
			Player.Get<State<HeroState>>().EState = HeroState.Idle;
		}
	}
}

