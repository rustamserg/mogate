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

		public GameMogate ()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 1024;
			graphics.IsFullScreen = false;		
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			var monsters = new Monsters (this);
			var gameState = new GameState (this);
			var items = new Items (this);
			var sprites = new SpriteSheets (this);
			var effects = new Effects (this);

			Services.AddService (typeof(IWorld), new World());
			Services.AddService (typeof(IItems), items);
			Services.AddService (typeof(IMonsters), monsters);
			Services.AddService (typeof(IGameState), gameState);
			Services.AddService (typeof(ISpriteSheets), sprites);
			Services.AddService (typeof(IEffects), effects);

			Components.Add (sprites);
			Components.Add (gameState);
			Components.Add (monsters);
			Components.Add (items);
			Components.Add (effects);

			Components.Add (new MapGridLayer(this));
			Components.Add (new ItemsLayer(this));
			Components.Add (new MonstersLayer (this));
			Components.Add (new EffectsLayer (this));

			// new flow, there is only director game component is added
			var director = new Director (this);
			Services.AddService (typeof(IDirector), director);

			director.RegisterScene (new GameScene (this, "game"));
			director.ActivateScene ("game");

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

