using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

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

		public MonsterEntity(int x, int y)
		{
			EState = MonsterState.Idle;
			Register (new Position (x, y));
			Register (new Health (100));
			Register (new ActionQueue ());
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
			m_monsters.Clear ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							m_monsters.Add (new MonsterEntity (x, y));
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
				monster.Get<ActionQueue> ().Add (new MoveTo (monster, newPos, 1, (_) => {
					monster.Get<ActionQueue> ().Add (new MoveTo (monster, curPos, 1, (__) => {
						hero.Player.Get<ActionQueue> ().Add (new AttackEntity (hero.Player, 10));
						monster.EState = MonsterState.Idle;
					}));
				}));
				monster.EState = MonsterState.Attacking;
			} else {
				monster.Get<ActionQueue> ().Add (new MoveTo (monster, newPos, 2, (_) => {
					monster.EState = MonsterState.Idle; }));
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
						monster.Get<ActionQueue>().Add (new MoveTo (monster, cell.Pos, 2, (_) => {
							monster.EState = MonsterState.Idle;
						}));
						monster.EState = MonsterState.Chasing;
						break;
					}
				}
			}
		}

		void UpdateActions ()
		{
			foreach (var me in m_monsters) {
				me.Get<ActionQueue> ().Update ();
			}
			m_monsters.RemoveAll (x => x.Get<Health> ().HP == 0);
		}
	}
}

