#region Using Statements

using Elizabeth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            m_graphics.GraphicsProfile = GraphicsProfile.HiDef;
            m_graphics.ApplyChanges();

            IsMouseVisible = true;
		}

		protected override void Initialize ()
		{
			var director = new Director (this);
			var gameState = new GameState (this);
			var sprites = new SpriteSheets (this);

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

			sprites.AddSpriteSheet ("Content/Sprites/sprites.plist", "Sprites/sprites", Globals.CELL_WIDTH, Globals.CELL_HEIGHT);
			#if __IOS__
			sprites.AddSpriteFont ("SpriteFont1", "Fonts/arial-22");
			#else
			sprites.AddSpriteFont ("SpriteFont1", "Fonts/SpriteFont1");
			#endif
			sprites.AddEffect ("light", "Shaders/lighting");
            sprites.AddEffect("fade", "Shaders/fade");

            base.Initialize ();
		}

		protected override void LoadContent ()
		{
			var director = (IDirector)Services.GetService (typeof(IDirector));
			director.ActivateScene ("main");
		}
	}
}

