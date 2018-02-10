using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mogate
{
    public static class MapGenerator
	{
		public struct Params
		{
			public int RoomMaxSize;
			public int RoomMinSize;
			public readonly int RoomsNumber;
			public int TunnelsCurveWeight;
			public bool RemoveDeadEnd;

			public Params(IMapGrid map)
			{
				RoomMaxSize = 7;
				RoomMinSize = 5;
				RoomsNumber = map.Height*map.Width/RoomMaxSize*RoomMinSize;
				TunnelsCurveWeight = 10;
				RemoveDeadEnd = false;
			}
		}

		enum Direction
		{
			Up,
			Right,
			Down,
			Left
		}

		static Params m_params;

		public static void Generate (IMapGrid map, Params param)
		{
			m_params = param;
			map.Init();

			// round block
			for (int x = 0; x < map.Width; x++)
				for (int y = 0; y < map.Height; y++)
					if (x == 0 || y == 0 || x == (map.Width - 1) || y == (map.Height - 1))
						map.SetID (x, y, MapGridTypes.ID.Blocked);
					else
						map.SetID (x, y, MapGridTypes.ID.Empty);

			// create rooms
			PlaceRooms(map);

			// create corridors
			PlaceCorridors(map);

			// place stairs
			PlaceStairs(map);

			// remove dead ends
			if (m_params.RemoveDeadEnd)
				RemoveDeadEnds(map);

			// final cleanup
			Cleanup(map);
		}

		static void Cleanup (IMapGrid map)
		{
			// cleanup empty
			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID(x, y) == MapGridTypes.ID.Empty)
						map.SetID (x, y, MapGridTypes.ID.Blocked);
					if (map.GetID(x, y) == MapGridTypes.ID.TunnelEnd)
						map.SetID (x, y, MapGridTypes.ID.Tunnel);
				}
			}

			// cleanup tunnels
			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID(x, y) == MapGridTypes.ID.Tunnel) {
						if (!map.GetBBox(x, y).Any(z => z.Type != MapGridTypes.ID.Blocked))
							map.SetID (x, y, MapGridTypes.ID.Blocked);
					}
				}
			}
		}

		static void PlaceCorridors (IMapGrid map)
		{
			for (int i = 0; i < map.Width/2; i++) {
				for (int j = 0; j < map.Height/2; j++) {
					int x = i * 2 + 1;
					int y = j * 2 + 1;

					if (map.GetID(x, y) != MapGridTypes.ID.Empty)
						continue;

					OpenCorridor (map, i, j, Utils.RandomEnumValue<Direction> ());
				}
			}

			// fixup tunnels end
			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID(x, y) == MapGridTypes.ID.TunnelEnd) {
						if (map.GetBBox(x, y).Any(z => z.Type == MapGridTypes.ID.Door))
							map.SetID (x, y, MapGridTypes.ID.Tunnel);
					}
				}
			}
		}

		static void OpenCorridor (IMapGrid map, int i, int j, Direction last_dir)
		{
			var all_dirs = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
			var dirs = new List<Direction>();

			all_dirs.Shuffle();
			if (Utils.Rand.Next (100) < m_params.TunnelsCurveWeight) dirs.Add(last_dir);
			dirs.AddRange(all_dirs.Where(x => !dirs.Contains(x)));

			foreach (var dir in dirs)
			{
				int xs = i*2 + 1;
				int ys = j*2 + 1;
				int xe = xs;
				int ye = ys;
				int xm = xs;
				int ym = ys;

				if (dir == Direction.Right) { xe = (i + 1)*2 + 1; xm = (i + 1)*2; } // right
				if (dir == Direction.Down) { ye = (j + 1)*2 + 1; ym = (j + 1)*2; } // down
				if (dir == Direction.Left) { xe = (i - 1)*2 + 1; xm = i*2; } // left
				if (dir == Direction.Up) { ye = (j - 1)*2 + 1; ym = j*2; } // up

				if ((map.GetID (xs, ys) == MapGridTypes.ID.Empty
				     || map.GetID (xs, ys) == MapGridTypes.ID.TunnelEnd
				     || map.GetID (xs, ys) == MapGridTypes.ID.Tunnel) 
				    && map.GetID (xm, ym) == MapGridTypes.ID.Empty
				    && (map.GetID (xe, ye) == MapGridTypes.ID.Empty || map.GetID (xe, ye) == MapGridTypes.ID.Door))
				{
					if (map.GetID (xs, ys) == MapGridTypes.ID.Empty)
						map.SetID (xs, ys, MapGridTypes.ID.TunnelEnd);
					else
						map.SetID (xs, ys, MapGridTypes.ID.Tunnel);

					if (map.GetID (xe, ye) == MapGridTypes.ID.Empty) {
						map.SetID (xm, ym, MapGridTypes.ID.Tunnel);
						map.SetID (xe, ye, MapGridTypes.ID.TunnelEnd);
					}
					else
						map.SetID (xm, ym, MapGridTypes.ID.TunnelEnd);

					if (dir == Direction.Right) i++;
					if (dir == Direction.Down) j++;
					if (dir == Direction.Left) i--;
					if (dir == Direction.Up) j--;

					OpenCorridor(map, i, j, dir);
				}
			}
		}

		static List<MapGridTypes.Room> GetRooms (IMapGrid map)
		{
			int roomId = 0;
			List<MapGridTypes.Room> sills = new List<MapGridTypes.Room> ();

			for (int i = 0; i < map.Width/2; i++) {
				for (int j = 0; j < map.Height/2; j++) {
					MapGridTypes.Room r = new MapGridTypes.Room(new Point(i*2 + 1, j*2 + 1), ++roomId);
					r.Width = Math.Max (m_params.RoomMinSize, Utils.Rand.Next (m_params.RoomMaxSize/2)*2 + 1);
					r.Height = Math.Max (m_params.RoomMinSize, Utils.Rand.Next (m_params.RoomMaxSize/2)*2 + 1);

					bool isFree = true;
					for (int x = r.Pos.X - 3; x < (r.Pos.X + r.Width + 3) && isFree; x++)
						for (int y = r.Pos.Y - 3; y <  (r.Pos.Y + r.Height + 3) && isFree; y++)
							isFree = (map.GetID(x, y) == MapGridTypes.ID.Empty);

					if (isFree) {
						for (int x = r.Pos.X - 1; x < (r.Pos.X + r.Width + 1); x++)
							for (int y = r.Pos.Y - 1; y <  (r.Pos.Y + r.Height + 1); y++)
								map.SetID (x, y, MapGridTypes.ID.Sill);

						sills.Add(r);
					}
				}
			}

			for (int x = 0; x < map.Width; x++)
				for (int y = 0; y < map.Height; y++)
					if (map.GetID(x, y) == MapGridTypes.ID.Sill)
						map.SetID (x, y, MapGridTypes.ID.Empty); 

			return sills;
		}

		static void PlaceRooms (IMapGrid map)
		{
			for (int i = 0; i < m_params.RoomsNumber; i++) {
				List<MapGridTypes.Room> sills = GetRooms(map);
				sills.Shuffle();

				if (sills.Count == 0)
					continue;

				MapGridTypes.Room r = sills.First();

				// alloc room
				for (int x = r.Pos.X - 1; x < (r.Pos.X + r.Width + 1); x++)
					for (int y = r.Pos.Y - 1; y <  (r.Pos.Y + r.Height + 1); y++)
						if (x == (r.Pos.X - 1) || x == (r.Pos.X + r.Width) || 
						    y == (r.Pos.Y - 1) || y == (r.Pos.Y + r.Height))
							map.SetID (x, y, MapGridTypes.ID.Blocked);
						else 
							map.SetID (x, y, MapGridTypes.ID.Room);

				// open room
				int doors = 2;
				while (doors > 0) {
					Direction dir = Utils.RandomEnumValue<Direction>();
					MapGridTypes.Door door = null;
					if (dir == Direction.Right) { // right 
						int y = r.Pos.Y + Utils.Rand.Next(r.Height/2)*2;
						if (map.GetID (r.Pos.X + r.Width + 1, y) == MapGridTypes.ID.Empty
						    && map.GetID (r.Pos.X + r.Width + 2, y) == MapGridTypes.ID.Empty
							&& map.GetID (r.Pos.X + r.Width, y - 1) != MapGridTypes.ID.Door
						    && map.GetID (r.Pos.X + r.Width, y + 1) != MapGridTypes.ID.Door) { 
							door = new MapGridTypes.Door (new Point(r.Pos.X + r.Width, y));
						}
					}
					if (dir == Direction.Down) { // down 
						int x = r.Pos.X + Utils.Rand.Next(r.Width/2)*2;
						if (map.GetID (x, r.Pos.Y + r.Height + 1) == MapGridTypes.ID.Empty
						    && map.GetID (x, r.Pos.Y + r.Height + 2) == MapGridTypes.ID.Empty
						    && map.GetID (x - 1, r.Pos.Y + r.Height) != MapGridTypes.ID.Door
						    && map.GetID (x + 1, r.Pos.Y + r.Height) != MapGridTypes.ID.Door) {
							door = new MapGridTypes.Door (new Point (x, r.Pos.Y + r.Height));
						}
					}
					if (dir == Direction.Left) { // left 
						int y = r.Pos.Y + Utils.Rand.Next(r.Height/2)*2;
						if (map.GetID (r.Pos.X - 1, y) == MapGridTypes.ID.Empty
						    && map.GetID (r.Pos.X - 2, y) == MapGridTypes.ID.Empty
						    && map.GetID (r.Pos.X, y - 1) != MapGridTypes.ID.Door
						    && map.GetID (r.Pos.X, y + 1) != MapGridTypes.ID.Door) {
							door = new MapGridTypes.Door (new Point (r.Pos.X, y));
						}
					}
					if (dir == Direction.Up) { // up 
						int x = r.Pos.X + Utils.Rand.Next(r.Width/2)*2;
						if (map.GetID (x, r.Pos.Y - 1) == MapGridTypes.ID.Empty
						    && map.GetID (x, r.Pos.Y - 2) == MapGridTypes.ID.Empty
						    && map.GetID (x - 1, r.Pos.Y) != MapGridTypes.ID.Door
						    && map.GetID (x + 1, r.Pos.Y) != MapGridTypes.ID.Door) {
							door = new MapGridTypes.Door (new Point (x, r.Pos.Y));
						}
					}
					if (door != null) {
						r.Doors.Add(door);
						map.SetID (door.Pos.X, door.Pos.Y, MapGridTypes.ID.Door);
						doors--;
					}
				}
				map.AddRoom(r);
			}
		}

		static void PlaceStairs (IMapGrid map)
		{
			List<Point> ends = new List<Point> ();

			for (int x = 0; x < map.Width; x++)
				for (int y = 0; y < map.Height; y++)
					if (map.GetID(x, y) == MapGridTypes.ID.TunnelEnd)
						ends.Add (new Point () { X = x, Y = y });

			ends.Shuffle ();

			map.StairUp = ends [0];
			map.StairDown = map.StairUp;
			foreach (var st in ends) {
				if (Utils.Dist(st, map.StairUp) > Utils.Dist(map.StairDown, map.StairUp))
					map.StairDown = st;
			}
			map.SetID (map.StairUp.X, map.StairUp.Y,  MapGridTypes.ID.StairUp);
			map.SetID (map.StairDown.X, map.StairDown.Y, MapGridTypes.ID.StairDown);
		}

		static void RemoveDeadEnds (IMapGrid map)
		{
			for (int i = 0; i < map.Width/2; i++) {
				for (int j = 0; j < map.Height/2; j++) {
					int x = i * 2 + 1;
					int y = j * 2 + 1;

					if (map.GetID(x, y) != MapGridTypes.ID.TunnelEnd)
						continue;

					CleanupTunnel (map, x, y);
				}
			}
		}

		static void CleanupTunnel (IMapGrid map, int x, int y)
		{
			if (map.GetID(x, y) != MapGridTypes.ID.Tunnel && map.GetID(x, y) != MapGridTypes.ID.TunnelEnd)
				return;

			int tuncells = map.GetBCross(x, y).Count(e => e.Type == MapGridTypes.ID.Tunnel);
			int empty = map.GetBCross(x, y).Count (e => e.Type == MapGridTypes.ID.Empty || e.Type == MapGridTypes.ID.Blocked);

			if (tuncells != 1 || empty != 3)
				return;

			var all_dirs = Enum.GetValues (typeof(Direction)).Cast<Direction> ().ToList ();
			foreach (var dir in all_dirs) {
				map.SetID (x, y, MapGridTypes.ID.Empty);
				if (dir == Direction.Up) CleanupTunnel (map, x, y - 1);
				if (dir == Direction.Right) CleanupTunnel (map, x + 1, y);
				if (dir == Direction.Down) CleanupTunnel (map, x, y + 1);
				if (dir == Direction.Left) CleanupTunnel (map, x - 1, y);
			}
		}
	}
}

