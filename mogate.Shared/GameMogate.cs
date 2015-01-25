#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Elizabeth;

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
			var director = new Director (this);
			var gameState = new GameState (this);
			var sprites = new SpriteSheets (this);
			var checkpoint = new Checkpoint<MogateSaveData> (this);

			Services.AddService (typeof(IWorld), new World());
			Services.AddService (typeof(IGameState), gameState);
			Services.AddService (typeof(ISpriteSheets), sprites);
			Services.AddService (typeof(IStatistics), new Statistics ());
			Services.AddService (typeof(IDirector), director);
			Services.AddService (typeof(ICheckpoint<MogateSaveData>), checkpoint);

			Components.Add (sprites);
			Components.Add (gameState);
			Components.Add (director);
			Components.Add (checkpoint);

			director.RegisterScene (new GameScene (this, "game"));
			director.RegisterScene (new MainScene (this, "main"));
			director.RegisterScene (new InterScene (this, "inter"));
			director.RegisterScene (new PlayerSelectScene (this, "player_select"));

			sprites.AddSpriteSheet ("Content/Sprites/sprites.plist", "Sprites/sprites", Globals.CELL_WIDTH, Globals.CELL_HEIGHT);
			#if __IOS__
			sprites.AddSpriteFont ("SpriteFont1", "Fonts/arial-22");
			#else
			sprites.AddSpriteFont ("SpriteFont1", "Fonts/SpriteFont1");
			#endif

			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			var director = (IDirector)Services.GetService (typeof(IDirector));
			director.ActivateScene ("main");
		}
	}
}

