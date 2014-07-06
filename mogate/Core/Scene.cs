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
		private Effect m_effect;
		private Dictionary<string, Layer> m_layersByName = new Dictionary<string, Layer>();
		private List<Layer> m_orderedLayers = new List<Layer> ();
		private int m_spent;
		private float m_fade;
		private float m_duration;

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

			using (var reader = new BinaryReader(File.Open("Content/Shaders/fade.xnb", FileMode.Open))) {
				m_effect = new Effect(Game.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
			}
		}

		public override void Update (GameTime gameTime)
		{
			if (State != SceneState.Deactivated) {
				if (State == SceneState.Activating) {
					m_spent += gameTime.ElapsedGameTime.Milliseconds;
					if (m_spent < m_duration)
						m_fade = m_spent / m_duration;
					else
						State = SceneState.Activated;
				} else {
					var iter = new List<Layer> (m_orderedLayers);
					foreach (var la in iter) {
						la.Update (gameTime);
					}
				}
			}
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			if (State != SceneState.Deactivated) {
				m_spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend); 
				if (State == SceneState.Activating) {
					m_effect.Parameters ["ColorAmount"].SetValue (m_fade);
					m_effect.CurrentTechnique.Passes [0].Apply ();
				} else {
					var iter = new List<Layer> (m_orderedLayers);
					foreach (var la in iter) {
						la.Draw (m_spriteBatch, gameTime);
					}
				}
				m_spriteBatch.End ();
			}
			base.Draw (gameTime);
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
			var la = m_layersByName [name];
			m_layersByName.Remove (name);
			m_orderedLayers.Remove (la);
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
				la.OnActivated ();
			}
		}

		public void DeactivateScene()
		{
			State = SceneState.Deactivated;

			var iter = new List<Layer> (m_orderedLayers);
			foreach (var la in iter) {
				la.OnDeactivated ();
			}
		}

		protected virtual void LoadLayers()
		{
		}
	}
}

