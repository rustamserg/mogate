using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace mogate
{
	public class Scene : DrawableGameComponent
	{
		private SpriteBatch m_spriteBatch;
		private Effect m_effect;
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

			using (var reader = new BinaryReader(File.Open("Content/grayscale.mgfxo", FileMode.Open))) {
				m_effect = new Effect(Game.GraphicsDevice, reader.ReadBytes((int)reader.BaseStream.Length));
			}
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
				m_spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend); 
				m_effect.Parameters ["ColorAmount"].SetValue(0.6f);
				m_effect.CurrentTechnique.Passes [0].Apply ();
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

