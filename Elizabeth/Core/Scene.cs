using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Elizabeth
{
	public enum SceneState
	{
		Deactivated,
		Activated
	};

	public class Scene : DrawableGameComponent
	{
		private SpriteBatch m_spriteBatch;
		private Dictionary<string, Layer> m_layersByName = new Dictionary<string, Layer>();
		private List<Layer> m_orderedLayers = new List<Layer> ();
		private Matrix m_worldToScreenMtx;

		private RenderTarget2D m_mainTarget;

		public string Name { get; private set; }
		public SceneState State { get; private set; }
		public int ViewportWidth { get; private set; }
		public int ViewportHeight { get; private set; }

		public Scene (Game game, string name, int viewportWidth, int viewportHeight) : base(game)
		{
			Name = name;
			State = SceneState.Deactivated;
			ViewportWidth = viewportWidth;
			ViewportHeight = viewportHeight;
		}

		protected void SetFadeEffect(string effectName)
		{
			
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);

			int backBufWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int backBufHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

			float horScaling = (float)backBufWidth / ViewportWidth;
			float verScaling = (float)backBufHeight / ViewportHeight;
			var screenScalingFactor = new Vector3(horScaling, verScaling, 1);
			m_worldToScreenMtx = Matrix.CreateScale (screenScalingFactor);

			m_mainTarget = new RenderTarget2D (Game.GraphicsDevice, backBufWidth, backBufHeight);

			OnInitialized ();
		}

		public override void Update (GameTime gameTime)
		{
			if (State != SceneState.Deactivated) {
				var mouse = Mouse.GetState ();
				var touch = TouchPanel.GetState ();

				foreach (var la in m_orderedLayers) {
					if (State == SceneState.Activated)
						la.Update (gameTime, mouse, touch);
				}
			}
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			if (State != SceneState.Deactivated) {
				Game.GraphicsDevice.SetRenderTarget (m_mainTarget);
				Game.GraphicsDevice.Clear (Color.Black);
				m_spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, m_worldToScreenMtx); 

				foreach (var la in m_orderedLayers) {
					la.Draw (m_spriteBatch, gameTime);
				}
				m_spriteBatch.End ();

				OnPostDraw (m_spriteBatch, m_mainTarget, m_worldToScreenMtx, gameTime);
			}
			base.Draw (gameTime);
		}

		protected virtual void OnPostDraw(SpriteBatch spriteBatch, RenderTarget2D mainTarget, Matrix worldToScreen, GameTime gameTime)
		{
			Game.GraphicsDevice.SetRenderTarget (null);  
			Game.GraphicsDevice.Clear (Color.Black);  
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);  
			spriteBatch.Draw (mainTarget, Vector2.Zero, Color.White);  
			spriteBatch.End ();  
		}

		public void AddLayer(Layer layer)
		{
			m_layersByName.Add (layer.Name, layer);
			m_orderedLayers = new List<Layer> (m_layersByName.Values);
			m_orderedLayers.OrderBy (la => la.ZOrder);
		}

		public Layer GetLayer(string name)
		{
			return m_layersByName [name];
		}

		public void RemoveLayer(string name)
		{
			m_layersByName.Remove (name);
			m_orderedLayers.RemoveAll (la => la.Name == name);
		}

		public void ActivateScene()
		{
			State = SceneState.Activated;

			m_layersByName.Clear ();
			m_orderedLayers.Clear ();

			LoadLayers ();

			var iter = new List<Layer> (m_orderedLayers);
			foreach (var la in iter) {
				la.Activate ();
			}
			OnActivated ();
		}

		public void DeactivateScene()
		{
			State = SceneState.Deactivated;

			var iter = new List<Layer> (m_orderedLayers);
			foreach (var la in iter) {
				la.Deactivate ();
			}
			OnDeactivated ();
		}

		protected virtual void LoadLayers()
		{
		}

		protected virtual void OnActivated()
		{
		}

		protected virtual void OnDeactivated()
		{
		}

		protected virtual void OnInitialized ()
		{
		}
	}
}

