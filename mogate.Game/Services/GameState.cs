using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public interface IGameState
	{
		int Level { get; }

		int PlayerHealth { get; set; }
		int MaxPlayerHealth { get; }

		int PlayerArmor { get; set; }
		int MaxPlayerArmor { get; }

		bool IsGameEnd { get; }

		void NewGame();
		void NextLevel();
	}

	public class GameState : GameComponent, IGameState
	{
		public int Level { get; private set; }
		public bool IsGameEnd { get; private set; }

		public int PlayerHealth { get; set; }
		public int MaxPlayerHealth { get; private set; }

		public int PlayerArmor { get; set; }
		public int MaxPlayerArmor { get; private set; }


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
			PlayerHealth = 3;
			MaxPlayerHealth = 5;
			PlayerArmor = 2;
			MaxPlayerArmor = 4;
			IsGameEnd = false;
		}
		 
		public void NextLevel()
		{
			Level = Level + 1;
		
			if (Level == Globals.MAX_LEVELS) {
				IsGameEnd = true;
				Level = 0;
				return;
			}
		}
	}
}

