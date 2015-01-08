#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace mogate
{
	public class GameMogate : Game
	{
		GraphicsDeviceManager m_graphics;
		Director m_director;

		public GameMogate ()
		{
			m_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			m_graphics.PreferredBackBufferHeight = Globals.VIEWPORT_HEIGHT;
			m_graphics.PreferredBackBufferWidth = Globals.VIEWPORT_WIDTH;
			m_graphics.IsFullScreen = false;

			IsMouseVisible = true;
		}

		protected override void Initialize ()
		{
			var gameState = new GameState (this);
			var sprites = new SpriteSheets (this);

			Services.AddService (typeof(IWorld), new World());
			Services.AddService (typeof(IGameState), gameState);
			Services.AddService (typeof(ISpriteSheets), sprites);
			Services.AddService (typeof(IStatistics), new Statistics ());

			Components.Add (sprites);
			Components.Add (gameState);

			// new flow, there is only director game component is added
			m_director = new Director (this);
			Services.AddService (typeof(IDirector), m_director);

			m_director.RegisterScene (new GameScene (this, "game"));
			m_director.RegisterScene (new MainScene (this, "main"));
			m_director.RegisterScene (new InterScene (this, "inter"));
			m_director.RegisterScene (new PlayerSelectScene (this, "player_select"));

			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			m_director.ActivateScene ("main");
		}
	}
}

