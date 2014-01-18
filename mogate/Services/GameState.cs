using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public enum EState
	{
		WorldLoading,
		WorldLoaded,
		LevelCreated,
		HeroCreated,
		ItemsCreated,
		MonstersCreated,
		GameStarted
	};

	public interface IGameState
	{
		EState State { get; set; }
		int Level { get; set; }

		void NewGame();
		void NextLevel();
	}

	public class GameState : GameComponent, IGameState
	{
		public EState State { get; set; }
		public int Level { get; set; }

		public GameState (Game game) : base(game)
		{
			State = EState.WorldLoading;
		}

		public override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Game.Exit ();
			}
			if (Keyboard.GetState ().IsKeyDown (Keys.N)) {
				NewGame ();
			}
			if (Keyboard.GetState ().IsKeyDown (Keys.Space)) {
				var director = (IDirector)Game.Services.GetService (typeof(IDirector));
				var sc = director.GetActiveScene ();
				if (sc.Name != "game") {
					director.ActivateScene ("game");
				}
			}

			if (State == EState.WorldLoading)
				NewGame ();

			if (State == EState.MonstersCreated)
				State = EState.GameStarted;

			base.Update(gameTime);
		}

		public void NewGame()
		{
			var world = (IWorld)Game.Services.GetService(typeof(IWorld));
			world.GenerateLevels (Globals.MAX_LEVELS);

			Level = 0;
			State = EState.WorldLoaded;
		}

		public void NextLevel()
		{
			Level = Math.Min (Level + 1, Globals.MAX_LEVELS - 1);
			State = EState.WorldLoaded;
		}
	}
}

