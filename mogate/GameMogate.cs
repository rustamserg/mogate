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
		GraphicsDeviceManager graphics;
		//SpriteBatch spriteBatch;

		readonly int WIDTH = 32;
		readonly int HEIGHT = 23;

		public GameMogate ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 1024;
			graphics.IsFullScreen = true;
			graphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			Services.AddService(typeof(IMapGrid), new MapGrid(WIDTH, HEIGHT));
			Services.AddService(typeof(IItems), new Items(this));
			Services.AddService(typeof(IHero), new Hero(this));

			var gameState = new GameState(this);
			Services.AddService(typeof(IGameState), gameState);

			Components.Add(gameState);
			Components.Add(new MapGridLayer(this));
			Components.Add(new ItemsLayer(this));
			Components.Add(new HeroLayer(this));

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

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit ();
			}
			// TODO: Add your update logic here			
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
			//TODO: Add your drawing code here
            
			base.Draw (gameTime);
		}
	}
}

