using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mogate
{
	public class Scene : DrawableGameComponent
	{
		private SpriteBatch m_spriteBatch;
		private Dictionary<string, Layer> m_layersByName = new Dictionary<string, Layer>();
		private List<Layer> m_orderedLayers = new List<Layer> ();

		public string Name { get; private set; }
		public bool IsActive { get; private set; }

		public Scene (Game game, string name) : base(game)
		{
			Name = name;
		}

		protected override void LoadContent ()
		{
			m_spriteBatch = new SpriteBatch (Game.GraphicsDevice);
		}

		public override void Update (GameTime gameTime)
		{
			if (IsActive) {
				var iter = new List<Layer> (m_orderedLayers);
				foreach (var la in iter) {
					la.Update (gameTime);
				}
			}
			base.Update (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			if (IsActive) {
				m_spriteBatch.Begin ();

				var iter = new List<Layer> (m_orderedLayers);
				foreach (var la in iter) {
					la.Draw (m_spriteBatch, gameTime);
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

		public void ActivateScene()
		{
			IsActive = true;

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
			IsActive = false;

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

