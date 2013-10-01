using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public enum MonsterState
	{
		Idle,
		Chasing,
		Attacking,
		Dead
	};

	public class MonsterEntity : Entity
	{
		public MonsterState EState;

		public MonsterEntity(int x, int y, Texture2D monster)
		{
			EState = MonsterState.Idle;
			Register (new Position (x, y));
			Register (new Health (100));
			Register (new Execute ());
			Register (new Drawable (monster,
			                        new Rectangle (0, 0, 32, 32),
			                        new Point (x * 32, y * 32)));
		}
	};

	public interface IMonsters
	{
		IEnumerable<MonsterEntity> GetMonsters();
	};

	public class Monsters : GameComponent, IMonsters
	{
		List<MonsterEntity> m_monsters = new List<MonsterEntity>();
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);


		public Monsters (Game game) : base(game)
		{
		}

		public override void Update(GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
	
			if (gameState.State == EState.ItemsCreated) {
				Init();
				gameState.State = EState.MonstersCreated;
			}

			if (gameState.State == EState.LevelStarted) {
				foreach (var pt in m_monsters) {
					UpdateMonster (pt);
				}
				UpdateActions ();
			}

			base.Update(gameTime);
		}

		public IEnumerable<MonsterEntity> GetMonsters()
		{
			return m_monsters;
		}
		
		void Init()
		{
			var map = (IMapGrid)Game.Services.GetService(typeof(IMapGrid));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			m_monsters.Clear ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							m_monsters.Add (new MonsterEntity (x, y, sprites.GetSprite("monster")));
						}
					}
				}
			}
		}

		void UpdateMonster (MonsterEntity data)
		{
			if (data.EState == MonsterState.Idle)
				UpdateIdleState (data);
		}

		void UpdateIdleState(MonsterEntity monster)
		{
			Point newPos = TryChasePlayer(monster);

			if (monster.Get<Position>().MapPos != newPos) {
				MonsterMove (monster, newPos);
			} else {
				MonsterIdle (monster);
			}
		}

		Point TryChasePlayer(MonsterEntity monster)
		{
			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			var map = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));

			Point newPos = monster.Get<Position>().MapPos;
			Point curPos = monster.Get<Position>().MapPos;

			if (MapGenerator.Dist (curPos, hero.Player.Get<Position> ().MapPos) < 6) {
				if (hero.Player.Get<Position> ().MapPos.X < monster.Get<Position> ().MapPos.X)
					newPos.X--;
				else if (hero.Player.Get<Position> ().MapPos.X > monster.Get<Position> ().MapPos.X)
					newPos.X++;
				else if (hero.Player.Get<Position> ().MapPos.Y < monster.Get<Position> ().MapPos.Y)
					newPos.Y--;
				else if (hero.Player.Get<Position> ().MapPos.Y > monster.Get<Position> ().MapPos.Y)
					newPos.Y++;

				if (m_monsters.Any (x => x.Get<Position> ().MapPos == newPos))
					newPos = curPos;
				else if (map.GetID (newPos.X, newPos.Y) != MapGridTypes.ID.Tunnel)
					newPos = curPos;
			}
			return newPos;
		}

		void MonsterMove(MonsterEntity monster, Point newPos)
		{
			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			Point curPos = monster.Get<Position>().MapPos;

			if (newPos == hero.Player.Get<Position> ().MapPos) {
				var seq = new Sequence ();
				seq.Add (new MoveTo (monster, new Point(newPos.X*32, newPos.Y*32), 2));
				seq.Add (new AttackEntity (hero.Player, 10));
				seq.Add (new MoveTo (monster, new Point(curPos.X*32, curPos.Y*32), 2));
				seq.Add (new ActionEntity(monster, (_) => {
					monster.EState = MonsterState.Idle;
				}));
				monster.Get<Execute> ().Start (seq);
				monster.EState = MonsterState.Attacking;
			} else {
				var seq = new Sequence ();
				seq.Add (new MoveTo (monster, new Point(newPos.X*32, newPos.Y*32), 4));
				seq.Add (new ActionEntity (monster, (_) => {
					monster.Get<Position> ().MapPos = newPos;
				}));
				seq.Add (new ActionEntity(monster, (_) => {
					monster.EState = MonsterState.Idle; }));
				monster.Get<Execute> ().Start (seq);
				monster.EState = MonsterState.Chasing;
			}
		}

		void MonsterIdle(MonsterEntity monster)
		{
			var map = (IMapGrid)Game.Services.GetService (typeof(IMapGrid));
			Point curPos = monster.Get<Position>().MapPos;

			if (m_monsters.Any (x => x.Get<Position> ().MapPos == curPos && x != monster)) {
				foreach (var cell in map.GetBCross(curPos.X, curPos.Y)) {
					if (cell.Type == MapGridTypes.ID.Tunnel) {
						var seq = new Sequence ();
						seq.Add (new MoveTo (monster, new Point(cell.Pos.X*32, cell.Pos.Y*32), 2));
						seq.Add (new ActionEntity (monster, (_) => {
							monster.Get<Position> ().MapPos = cell.Pos;
						}));
						seq.Add (new ActionEntity(monster, (_) => {
							monster.EState = MonsterState.Idle;
						}));
						monster.Get<Execute> ().Start (seq);
						monster.EState = MonsterState.Chasing;
						break;
					}
				}
			}
		}

		void UpdateActions ()
		{
			foreach (var me in m_monsters) {
				me.Get<Execute> ().Update ();
			}
			m_monsters.RemoveAll (x => x.Get<Health> ().HP == 0);
		}
	}
}

