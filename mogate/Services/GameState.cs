using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace mogate
{
	public enum EState
	{
		LevelStarting,
		MapGenerated,
		HeroCreated,
		ItemsCreated,
		LevelStarted
	};

	public interface IGameState
	{
		EState State { get; set; }
	}

	public class GameState : GameComponent, IGameState
	{
		public EState State { get; set; }

		public GameState (Game game) : base(game)
		{
			State = EState.LevelStarting;
		}

		public override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.Space))
				State = EState.LevelStarting;

			if (State == EState.ItemsCreated)
				State = EState.LevelStarted;

			base.Update(gameTime);
		}
	}
}

