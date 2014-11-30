using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace mogate
{
	public class GameScene : Scene
	{
		private Texture2D m_lightTexture;
		private RenderTarget2D m_lightTarget;
		private Effect m_lightEffect;

		public GameScene (Game game, string name) : base(game, name)
		{
		}

		protected override void OnInitialized()
		{
			int backBufWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int backBufHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

			m_lightTarget = new RenderTarget2D (Game.GraphicsDevice, backBufWidth, backBufHeight);
			m_lightTexture = Game.Content.Load<Texture2D> ("Sprites/lightmask");
			#if !__IOS__
			using (var reader = new BinaryReader(File.Open("Content/Shaders/lighting.xnb", FileMode.Open))) {
				m_lightEffect = new Effect(Game.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
			}
			#endif
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, RenderTarget2D mainTarget, Matrix worldToScreen, GameTime gameTime)
		{
			#if !__IOS__
			Game.GraphicsDevice.SetRenderTarget (m_lightTarget);
			Game.GraphicsDevice.Clear (Color.Black);
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, worldToScreen);
			var lightPos = GetLayer("player").GetEntityByTag("player").Get<Drawable>().DrawPos;
			lightPos.X -= 100;
			lightPos.Y -= 100;
			spriteBatch.Draw(m_lightTexture, lightPos, Color.White);
			spriteBatch.End ();
			#endif

			Game.GraphicsDevice.SetRenderTarget (null);  
			Game.GraphicsDevice.Clear (Color.Black);  
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, worldToScreen);  
			#if !__IOS__
			m_lightEffect.Parameters["lightMask"].SetValue(m_lightTarget);  
			m_lightEffect.CurrentTechnique.Passes[0].Apply();  
			#endif
			spriteBatch.Draw (mainTarget, Vector2.Zero, Color.White);  
			spriteBatch.End ();  
		}

		protected override void LoadLayers()
		{
			AddLayer (new MapGridLayer (Game, "map", this, 0));
			AddLayer (new ItemsLayer (Game, "items", this, 1));
			AddLayer (new PlayerLayer (Game, "player", this, 2));
			AddLayer (new MonstersLayer (Game, "monsters", this, 3));
			AddLayer (new EffectsLayer (Game, "effects", this, 4));

			// TODO: redesign fog by hiding AI
			#if !__IOS__
			//AddLayer (new FogLayer (Game, "fog", this, 5));
			#endif
			AddLayer (new HUDLayer (Game, "hud", this, 6));
		}

		protected override void OnActivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.CountPlaytime = true;
		}

		protected override void OnDeactivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.CountPlaytime = false;
		}
	}
}

