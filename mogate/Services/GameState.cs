using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public interface IGameState
	{
		int Level { get; set; }

		void NewGame();
		void NextLevel();
	}

	public class GameState : GameComponent, IGameState
	{
		public int Level { get; set; }

		public GameState (Game game) : base(game)
		{
		}

		public override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Game.Exit ();
			}
			base.Update(gameTime);
		}

		public void NewGame()
		{
			var world = (IWorld)Game.Services.GetService(typeof(IWorld));
			world.GenerateLevels (Globals.MAX_LEVELS);

			Level = 0;
		}

		public void NextLevel()
		{
			Level = Math.Min (Level + 1, Globals.MAX_LEVELS - 1);
		}
	}
}

