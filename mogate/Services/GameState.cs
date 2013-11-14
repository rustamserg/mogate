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

	public interface IGameSerializer
	{
		void Save();
		void Load();
	}

	public interface IGameState
	{
		EState State { get; set; }
		int Level { get; set; }
	}

	public class GameState : GameComponent, IGameState
	{
		public EState State { get; set; }
		public int Level { get; set; }

		public GameState (Game game) : base(game)
		{
			State = EState.WorldLoading;
			Level = 0;
		}

		public override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.Space)) {
				var world = (IWorld)Game.Services.GetService(typeof(IWorld));
				world.GenerateLevels (10);
				State = EState.WorldLoaded;
			}

			if (State == EState.MonstersCreated)
				State = EState.GameStarted;

			base.Update(gameTime);
		}
	}
}

