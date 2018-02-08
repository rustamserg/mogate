using Elizabeth;
using Microsoft.Xna.Framework;

namespace mogate
{
    public class MenuBackgroundLayer : Layer
	{
		private int m_tileSize;
		private int m_tilesHeight;
		private int m_tilesWidth;

		public MenuBackgroundLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			m_tileSize = sprites.GetSprite ("back_01").Rect.Width;
			m_tilesWidth = Globals.VIEWPORT_WIDTH / m_tileSize + 1;
			m_tilesHeight = Globals.VIEWPORT_HEIGHT / m_tileSize + 1;

			for (int x = 0; x < m_tilesWidth; x++) {
				for (int y = 0; y < m_tilesHeight; y++) {
					var tileEnt = CreateEntity (string.Format("{0}_{1}", x, y));
					tileEnt.Register (new Sprite (sprites.GetSprite ("back_01"), false));
					tileEnt.Register (new Drawable (new Vector2 (x * m_tileSize, y * m_tileSize)));
				}
			}
			var controller = CreateEntity ();
			controller.Register (new Execute ());
			controller.Get<Execute> ().Add (new Loop (new ActionEntity (controller, (_) => {
				Update (controller);
			}), 50));
		}

		void Update(Entity controller)
		{
			for (int x = 0; x < m_tilesWidth; x++) {
				for (int y = 0; y < m_tilesHeight; y++) {
					var tileEnt = GetEntityByTag (string.Format("{0}_{1}", x, y));
					tileEnt.Get<Drawable> ().DrawPos.X--;
					tileEnt.Get<Drawable> ().DrawPos.Y--;
					if (tileEnt.Get<Drawable> ().DrawPos.X % m_tileSize == 0) {
						tileEnt.Get<Drawable> ().DrawPos.X = x * m_tileSize;
						tileEnt.Get<Drawable> ().DrawPos.Y = y * m_tileSize;
					}
				}
			}
		}
	}
}

