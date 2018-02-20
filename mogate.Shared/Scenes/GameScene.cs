using Elizabeth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace mogate
{
    public class GameScene : Scene
	{
		private Sprite2D m_lightSpritePointSmall;
		private Sprite2D m_lightSpritePointNormal;
		private Sprite2D m_lightSpritePointBig;
		private Sprite2D m_lightSpriteDirectUp;
		private Sprite2D m_lightSpriteDirectDown;
		private Sprite2D m_lightSpriteDirectLeft;
		private Sprite2D m_lightSpriteDirectRight;

		private RenderTarget2D m_lightTarget;
		private Effect m_lightEffect;
		private Color m_lightColor;

		public GameScene (Game game, string name) : base(game, name, Globals.VIEWPORT_WIDTH, Globals.VIEWPORT_HEIGHT)
		{
		}

		protected override void OnInitialized()
		{
			var res = (IGameResources)Game.Services.GetService (typeof(IGameResources));

			int backBufWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int backBufHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

			m_lightTarget = new RenderTarget2D (Game.GraphicsDevice, backBufWidth, backBufHeight);
			m_lightSpritePointSmall = res.GetSprite("lightmask_small");
			m_lightSpritePointNormal = res.GetSprite ("lightmask_normal");
			m_lightSpritePointBig = res.GetSprite ("lightmask_big");
			m_lightSpriteDirectUp = res.GetSprite ("lightmask_up");
			m_lightSpriteDirectDown = res.GetSprite ("lightmask_down");
			m_lightSpriteDirectLeft = res.GetSprite ("lightmask_left");
			m_lightSpriteDirectRight = res.GetSprite ("lightmask_right");
			m_lightEffect = res.GetEffect ("light");
		}

		protected override void OnPostDraw(SpriteBatch spriteBatch, RenderTarget2D mainTarget, Matrix worldToScreen, GameTime gameTime)
		{
			var player = GetLayer ("player");
			var items = GetLayer ("items");
			var maps = GetLayer ("map");
			var monsters = GetLayer ("monsters");

			var toLightPoint = player.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()).ToList();
			toLightPoint.AddRange (items.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()));
			toLightPoint.AddRange (maps.GetAllEntities ().Where (e => e.Has<PointLight> () && e.Has<Drawable> ()));

			var toLightPointSmall = toLightPoint.Where (e => e.Get<PointLight> ().Distance == PointLight.DistanceType.Small);
			var toLightPointNormal = toLightPoint.Where (e => e.Get<PointLight> ().Distance == PointLight.DistanceType.Normal);
			var toLightPointBig = toLightPoint.Where (e => e.Get<PointLight> ().Distance == PointLight.DistanceType.Big);

			var toLightDirect = monsters.GetAllEntities().Where(e => e.Has<DirectLight> () && e.Has<Drawable> () && e.Has<LookDirection>());
			var toLightDirectUp = toLightDirect.Where (e => e.Get<LookDirection> ().Direction == Utils.Direction.Up);
			var toLightDirectDown = toLightDirect.Where (e => e.Get<LookDirection> ().Direction == Utils.Direction.Down);
			var toLightDirectLeft = toLightDirect.Where (e => e.Get<LookDirection> ().Direction == Utils.Direction.Left);
			var toLightDirectRight = toLightDirect.Where (e => e.Get<LookDirection> ().Direction == Utils.Direction.Right);

			Game.GraphicsDevice.SetRenderTarget (m_lightTarget);
			Game.GraphicsDevice.Clear (m_lightColor);
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, worldToScreen);

			DrawPointLightMasks (toLightPointSmall, m_lightSpritePointSmall, spriteBatch);
			DrawPointLightMasks (toLightPointNormal, m_lightSpritePointNormal, spriteBatch);
			DrawPointLightMasks (toLightPointBig, m_lightSpritePointBig, spriteBatch);
			DrawDirectLightMasks (toLightDirectUp, m_lightSpriteDirectUp, spriteBatch);
			DrawDirectLightMasks (toLightDirectDown, m_lightSpriteDirectDown, spriteBatch);
			DrawDirectLightMasks (toLightDirectLeft, m_lightSpriteDirectLeft, spriteBatch);
			DrawDirectLightMasks (toLightDirectRight, m_lightSpriteDirectRight, spriteBatch);

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

		private void DrawDirectLightMasks(IEnumerable<Entity> toLight, Sprite2D lightSprite, SpriteBatch spriteBatch)
		{
			foreach (var pe in toLight) {
				var lightPos = pe.Get<Drawable>().DrawPos;
				var lightColor = pe.Get<DirectLight> ().LightColor;
				lightPos.X -= (lightSprite.Rect.Width - Globals.CELL_WIDTH)/2;
				lightPos.Y -= (lightSprite.Rect.Height - Globals.CELL_HEIGHT)/2;
				spriteBatch.Draw(lightSprite.Texture, lightPos, lightSprite.Rect, lightColor);
			}
		}

		private void DrawPointLightMasks(IEnumerable<Entity> toLight, Sprite2D lightSprite, SpriteBatch spriteBatch)
		{
			foreach (var pe in toLight) {
				var lightPos = pe.Get<Drawable>().DrawPos;
				var lightColor = pe.Get<PointLight> ().LightColor;
				lightPos.X -= (lightSprite.Rect.Width - Globals.CELL_WIDTH)/2;
				lightPos.Y -= (lightSprite.Rect.Height - Globals.CELL_HEIGHT)/2;
				spriteBatch.Draw(lightSprite.Texture, lightPos, lightSprite.Rect, lightColor);
			}
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

			m_lightColor = (gameState.Level == Globals.MAX_LEVELS - 1) ? Color.Blue : Color.Green;
		}

		protected override void OnDeactivated()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			gameState.CountPlaytime = false;
		}
	}
}

