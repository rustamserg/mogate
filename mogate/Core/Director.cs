using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace mogate
{
	public interface IDirector
	{
		void RegisterScene (Scene scene);
		void ActivateScene (string name);
		Scene GetScene(string name);
		Scene GetActiveScene();
	}

	public class Director : GameComponent, IDirector
	{
		private Dictionary<string, Scene> m_scenes = new Dictionary<string, Scene>();
		private string m_activeScene;

		public Director (Game game) : base(game)
		{
		}
		 
		public void RegisterScene (Scene scene)
		{
			m_scenes.Add (scene.Name, scene);
		}

		public void ActivateScene (string name)
		{
			var sc = m_scenes [name];
			m_activeScene = name;

			//Game.Components.Clear (); // TODO: uncomment when all moved to scene-based logic
			sc.ComposeScene ();
			Game.Components.Add (sc);
		}

		public Scene GetActiveScene()
		{
			return GetScene (m_activeScene);
		}

		public Scene GetScene(string name)
		{
			return m_scenes [name];
		}
	}
}

