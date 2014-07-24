#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace mogate
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GameMogate : Game
	{
		GraphicsDeviceManager m_graphics;
		Director m_director;

		public GameMogate ()
		{
			m_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			m_graphics.PreferredBackBufferHeight = 768;
			m_graphics.PreferredBackBufferWidth = 1024;
			m_graphics.IsFullScreen = false;

			IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			var gameState = new GameState (this);
			var sprites = new SpriteSheets (this);

			Services.AddService (typeof(IWorld), new World());
			Services.AddService (typeof(IGameState), gameState);
			Services.AddService (typeof(ISpriteSheets), sprites);

			Components.Add (sprites);
			Components.Add (gameState);

			// new flow, there is only director game component is added
			m_director = new Director (this);
			Services.AddService (typeof(IDirector), m_director);

			m_director.RegisterScene (new GameScene (this, "game"));
			m_director.RegisterScene (new MainScene (this, "main"));
			m_director.RegisterScene (new InterScene (this, "inter"));

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			//spriteBatch = new SpriteBatch (GraphicsDevice);
			m_director.ActivateScene ("main", TimeSpan.Zero);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			m_graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
			base.Draw (gameTime);
		}
	}
}

