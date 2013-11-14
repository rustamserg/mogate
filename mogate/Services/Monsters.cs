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

	public interface IMonsters
	{
		IEnumerable<Entity> GetMonsters();
	};

	public class Monsters : GameComponent, IMonsters
	{
		List<Entity> m_monsters = new List<Entity>();
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

			if (gameState.State == EState.GameStarted) {
				foreach (var pt in m_monsters) {
					UpdateMonster (pt);
				}
				UpdateActions (gameTime);
			}

			base.Update(gameTime);
		}

		public IEnumerable<Entity> GetMonsters()
		{
			return m_monsters;
		}
		
		void Init()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			m_monsters.Clear ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							var me = new Entity ();
							me.Register (new State<MonsterState> (MonsterState.Idle));
							me.Register (new Position (x, y));
							me.Register (new Health (100));
							me.Register (new Attack (20));
							me.Register (new Attackable ((attacker) => OnAttacked(me, attacker)));
							me.Register (new Execute ());
							me.Register (new Drawable (sprites.GetSprite("monster"), "idle", new Vector2 (x * 32, y * 32)));
							m_monsters.Add (me);
						}
					}
				}
			}
		}

		void UpdateMonster (Entity monster)
		{
			if (monster.Get<State<MonsterState>>().EState == MonsterState.Idle)
				UpdateIdleState (monster);
		}

		void UpdateIdleState(Entity monster)
		{
			Point newPos = TryChasePlayer(monster);

			if (monster.Get<Position>().MapPos != newPos) {
				MonsterMove (monster, newPos);
			} else {
				MonsterIdle (monster);
			}
		}

		Point TryChasePlayer(Entity monster)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);

			var hero = (IHero)Game.Services.GetService (typeof(IHero));

			Point newPos = monster.Get<Position>().MapPos;
			Point curPos = monster.Get<Position>().MapPos;

			if (Utils.Dist (curPos, hero.Player.Get<Position> ().MapPos) < 6) {
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

		void MonsterMove(Entity monster, Point newPos)
		{
			var hero = (IHero)Game.Services.GetService (typeof(IHero));
			Point curPos = monster.Get<Position>().MapPos;

			if (newPos == hero.Player.Get<Position> ().MapPos) {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*32, newPos.Y*32), 500));
				seq.Add (new AttackEntity (monster, hero.Player));
				seq.Add (new MoveSpriteTo (monster, new Vector2(curPos.X*32, curPos.Y*32), 500));
				seq.Add (new ActionEntity(monster, (_) => {
					TryChangeState(monster, MonsterState.Idle);
				}));
				monster.Get<Execute> ().Add (seq);
				monster.Get<State<MonsterState>>().EState = MonsterState.Attacking;
			} else {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*32, newPos.Y*32), 600));
				seq.Add (new ActionEntity (monster, (_) => {
					monster.Get<Position> ().MapPos = newPos;
				}));
				seq.Add (new ActionEntity(monster, (_) => {
					TryChangeState(monster, MonsterState.Idle);
				}));
				monster.Get<Execute> ().Add (seq);
				monster.Get<State<MonsterState>>().EState = MonsterState.Chasing;
			}
		}

		void MonsterIdle(Entity monster)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);
			Point curPos = monster.Get<Position>().MapPos;

			if (m_monsters.Any (x => x.Get<Position> ().MapPos == curPos && x != monster)) {
				var dirs = new List<MapGridTypes.Cell> (map.GetBCross (curPos.X, curPos.Y));
				dirs.Shuffle ();
				foreach (var cell in dirs) {
					if (cell.Type == MapGridTypes.ID.Tunnel) {
						var seq = new Sequence ();
						seq.Add (new MoveSpriteTo (monster, new Vector2(cell.Pos.X*32, cell.Pos.Y*32), 200));
						seq.Add (new ActionEntity (monster, (_) => {
							monster.Get<Position> ().MapPos = cell.Pos;
						}));
						seq.Add (new ActionEntity(monster, (_) => {
							TryChangeState(monster, MonsterState.Idle);
						}));
						monster.Get<Execute> ().Add (seq);
						monster.Get<State<MonsterState>>().EState = MonsterState.Chasing;
						break;
					}
				}
			}
		}

		void UpdateActions (GameTime gameTime)
		{
			foreach (var me in m_monsters) {
				me.Get<Execute> ().Update (gameTime);
			}
		}

		void TryChangeState(Entity monster, MonsterState newState)
		{
			if (monster.Get<State<MonsterState>> ().EState != MonsterState.Dead)
				monster.Get<State<MonsterState>> ().EState = newState;
		}

		void OnAttacked(Entity monster, Entity attacker)
		{
			if (monster.Get<Health> ().HP == 0)
				monster.Get<State<MonsterState>> ().EState = MonsterState.Dead;
		}
	}
}
