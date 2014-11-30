using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace mogate
{
	public enum SceneState
	{
		Deactivated,
		Activating,
		Activated
	};

	public class Scene : DrawableGameComponent
	{
		private SpriteBatch m_spriteBatch;
		#if !__IOS__
		private Effect m_effect;
		private float m_fade;
		#endif
		private Dictionary<string, Layer> m_layersByName = new Dictionary<string, Layer>();
		private List<Layer> m_orderedLayers = new List<Layer> ();
		private int m_spent;
		private float m_duration;
		private Matrix m_worldToScreenMtx;

		private RenderTarget2D m_mainTarget;

		public string Name { get; private set; }
		public SceneState State { get; private set; }

		public Scene (Game game, string name) : base(game)
		{
			Name = name;
			State = SceneState.Deactivated;
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);

			#if !__IOS__
			using (var reader = new BinaryReader(File.Open("Content/Shaders/fade.xnb", FileMode.Open))) {
				m_effect = new Effect(Game.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
			}
			#endif

			int backBufWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int backBufHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

			float horScaling = (float)backBufWidth / Globals.VIEWPORT_WIDTH;
			float verScaling = (float)backBufHeight / Globals.VIEWPORT_HEIGHT;
			var screenScalingFactor = new Vector3(horScaling, verScaling, 1);
			m_worldToScreenMtx = Matrix.CreateScale (screenScalingFactor);

			m_mainTarget = new RenderTarget2D (Game.GraphicsDevice, backBufWidth, backBufHeight);

			OnInitialized ();
		}

		public override void Update (GameTime gameTime)
		{
			if (State != SceneState.Deactivated) {
				if (State == SceneState.Activating) {
					m_spent += gameTime.ElapsedGameTime.Milliseconds;
					if (m_spent < m_duration) {
						#if !__IOS__
						m_fade = m_spent / m_duration;
						#endif
					} else {
						State = SceneState.Activated;
					}
				} else {
					foreach (var la in m_orderedLayers) {
						if (State == SceneState.Activated)
							la.Update (gameTime);
					}
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
				if (State == SceneState.Activating) {
					#if !__IOS__
					m_effect.Parameters ["ColorAmount"].SetValue (m_fade);
					m_effect.CurrentTechnique.Passes [0].Apply ();
					#endif
				}
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
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, worldToScreen);  
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

		public void ActivateScene(float duration)
		{
			State = SceneState.Activating;
			m_spent = 0;
			m_duration = duration;

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

