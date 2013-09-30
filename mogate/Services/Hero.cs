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

	public class HeroEntity : Entity
	{
		public HeroState EState;

		public HeroEntity ()
		{
			EState = HeroState.Idle;
			Register (new Execute ());
		}
	};

	public interface IHero
	{
		HeroEntity Player { get; }
	};

	public class Hero : GameComponent, IHero
	{
		HeroEntity m_player;

		public Hero (Game game) : base(game)
		{
			m_player = new HeroEntity ();
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
				Player.Get<Execute> ().Update ();
			}

			base.Update (gameTime);
		}

		public HeroEntity Player { get { return m_player; } }

		private void Init ()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			Player.EState = HeroState.Idle;
			Player.Register (new Health (200));
			Player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			Player.Register (new Drawable (sprites.GetSprite ("hero"), new Rectangle (0, 0, 32, 32)));
		}

		private void UpdateHero()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

			if (Player.EState == HeroState.Idle) {
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
				if (mt != MapGridTypes.ID.Blocked) {
					var seq = new Sequence ();
					seq.Add (new MoveTo (Player, newPos, 4));
					seq.Add (new ActionEntity (Player, OnEndMove));
					Player.Get<Execute>().Start(seq);
					Player.EState = HeroState.Moving;
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
			Player.EState = HeroState.Idle;
		}
	}
}

