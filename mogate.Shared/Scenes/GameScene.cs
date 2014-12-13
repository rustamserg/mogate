using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;

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

			using (var reader = new BinaryReader(File.Open("Content/Shaders/lighting.xnb", FileMode.Open, FileAccess.Read))) {
				m_lightEffect = new Effect(Game.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
			}
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, RenderTarget2D mainTarget, Matrix worldToScreen, GameTime gameTime)
		{
			var player = GetLayer ("player");
			var items = GetLayer ("items");
			var maps = GetLayer ("map");

			var toLight = player.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()).ToList();
			toLight.AddRange (items.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()));
			toLight.AddRange (maps.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()));

			Game.GraphicsDevice.SetRenderTarget (m_lightTarget);
			Game.GraphicsDevice.Clear (Color.Black);
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, worldToScreen);

			foreach (var pe in toLight) {
				var lightPos = pe.Get<Drawable>().DrawPos;
				lightPos.X -= 100; lightPos.Y -= 100;
				spriteBatch.Draw(m_lightTexture, lightPos, Color.White);
			}
			spriteBatch.End ();

			Game.GraphicsDevice.SetRenderTarget (null);  
			Game.GraphicsDevice.Clear (Color.Black);  
			Game.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);  
			m_lightEffect.Parameters["lightMask"].SetValue(m_lightTarget);  
			m_lightEffect.CurrentTechnique.Passes[0].Apply();  
			spriteBatch.Draw (mainTarget, Vector2.Zero, Color.White);  
			spriteBatch.End ();

			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, worldToScreen);  
			GetLayer ("hud").Draw (spriteBatch, gameTime);
			spriteBatch.End ();
		}

		protected override void LoadLayers()
		{
			AddLayer (new MapGridLayer (Game, "map", this, 0));
			AddLayer (new ItemsLayer (Game, "items", this, 1));
			AddLayer (new PlayerLayer (Game, "player", this, 2));
			AddLayer (new MonstersLayer (Game, "monsters", this, 3));
			AddLayer (new EffectsLayer (Game, "effects", this, 4));
			AddLayer (new HUDLayer (Game, "hud", this, 5));
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

