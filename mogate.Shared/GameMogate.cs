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
			var director = new Director (this);

			Services.AddService (typeof(IWorld), new World());
			Services.AddService (typeof(IGameState), gameState);
			Services.AddService (typeof(ISpriteSheets), sprites);
			Services.AddService (typeof(IStatistics), new Statistics ());
			Services.AddService (typeof(IDirector), director);

			Components.Add (sprites);
			Components.Add (gameState);
			Components.Add (director);

			director.RegisterScene (new GameScene (this, "game"));
			director.RegisterScene (new MainScene (this, "main"));
			director.RegisterScene (new InterScene (this, "inter"));
			director.RegisterScene (new PlayerSelectScene (this, "player_select"));

			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			var director = (IDirector)Services.GetService (typeof(IDirector));
			director.ActivateScene ("main");
		}
	}
}

