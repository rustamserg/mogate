using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace mogate
{
	namespace MapGridTypes
	{	
		public enum ID
		{
			Blocked,
			Empty,
			Room,
			Sill,
			Door,
			TunnelEnd,
			Tunnel,
			StairUp,
			StairDown
		}

		public class Cell
		{
			public Point Pos;
			public ID Type;

			public Cell(Point p, ID t)
			{
				Pos = p;
				Type = t;
			}
		}

		public class Door : Cell
		{
			public Door(Point p) : base(p, ID.Door) {
			}
		}

		public class Room : Cell
		{
			public int Width;
			public int Height;
			public List<Door> Doors = new List<Door>();

			public Room(Point p) : base(p, ID.Room) {
			}
		}
	}

	public interface IMapGrid
	{
		int Width { get; set; }
		int Height { get; set; }
		Point StairUp { get; set; }
		Point StairDown { get; set; }

		void Init();
		MapGridTypes.ID GetID(int x, int y);
		void SetID(int x, int y, MapGridTypes.ID id);
		List<MapGridTypes.Cell> GetBBox (int x, int y);
		List<MapGridTypes.Cell> GetBCross (int x, int y);
		List<MapGridTypes.Cell> GetLine (Point from, Point to);
		Point ScreenToWorld (int x, int y);

		void AddRoom(MapGridTypes.Room room);
		List<MapGridTypes.Room> GetRooms();
		List<Point> GetTunnels();
	}

	public class MapGrid : IMapGrid
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public Point StairUp { get; set; }
		public Point StairDown { get; set; }

		MapGridTypes.Cell[,] m_map;
		List<MapGridTypes.Room> m_rooms;

		public MapGrid (int width, int heigth)
		{
			Width = width;
			Height = heigth;
			Init ();
		}

		public void Init ()
		{
			m_map = new MapGridTypes.Cell[Width, Height];
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					m_map [x, y] = new MapGridTypes.Cell (new Point (x, y), MapGridTypes.ID.Blocked);

			m_rooms = new List<mogate.MapGridTypes.Room>();
		}

		public MapGridTypes.ID GetID(int x, int y)
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				return MapGridTypes.ID.Blocked;

			return m_map[x, y].Type;
		}

		public void SetID(int x, int y, MapGridTypes.ID id)
		{
			if (x >= 0 && y >= 0 && x < Width && y < Height)
				m_map[x, y].Type = id;
		}

		public void AddRoom(MapGridTypes.Room room)
		{
			m_rooms.Add(room);
		}

		public List<MapGridTypes.Room> GetRooms ()
		{
			return new List<MapGridTypes.Room>(m_rooms);
		}

		public List<MapGridTypes.Cell> GetBBox (int x, int y)
		{
			List<MapGridTypes.Cell> box = new List<MapGridTypes.Cell>();
			box.Add (new MapGridTypes.Cell(new Point(x - 1, y - 1), GetID (x - 1, y - 1)));
			box.Add (new MapGridTypes.Cell(new Point(x, y - 1), GetID (x, y - 1)));
			box.Add (new MapGridTypes.Cell(new Point(x + 1, y - 1), GetID (x + 1, y - 1)));
			box.Add (new MapGridTypes.Cell(new Point(x - 1, y), GetID (x - 1, y)));
			box.Add (new MapGridTypes.Cell(new Point(x + 1, y), GetID (x + 1, y)));
			box.Add (new MapGridTypes.Cell(new Point(x - 1, y + 1), GetID (x - 1, y + 1)));
			box.Add (new MapGridTypes.Cell(new Point(x, y + 1), GetID (x, y + 1)));
			box.Add (new MapGridTypes.Cell(new Point(x + 1, y + 1), GetID (x + 1, y + 1)));

			return box;
		}

		public List<MapGridTypes.Cell> GetBCross (int x, int y)
		{
			List<MapGridTypes.Cell> box = new List<MapGridTypes.Cell>();
			box.Add (new MapGridTypes.Cell(new Point(x, y - 1), GetID (x, y - 1)));
			box.Add (new MapGridTypes.Cell(new Point(x - 1, y), GetID (x - 1, y)));
			box.Add (new MapGridTypes.Cell(new Point(x + 1, y), GetID (x + 1, y)));
			box.Add (new MapGridTypes.Cell(new Point(x, y + 1), GetID (x, y + 1)));

			return box;
		}

		public List<MapGridTypes.Cell> GetLine(Point from, Point to)
		{
			var ids = new List<MapGridTypes.Cell> ();

			if (from.X == to.X && from.Y < to.Y) {
				for (int pos = from.Y; pos <= to.Y; pos++) {
					ids.Add(new MapGridTypes.Cell (new Point (from.X, pos), GetID (from.X, pos)));
				}
			} else if (from.X == to.X && from.Y > to.Y) {
				for (int pos = from.Y; pos >= to.Y; pos--) {
					ids.Add(new MapGridTypes.Cell (new Point (from.X, pos), GetID (from.X, pos)));
				}
			} else if (from.Y == to.Y && from.X < to.X) {
				for (int pos = from.X; pos <= to.X; pos++) {
					ids.Add(new MapGridTypes.Cell (new Point (pos, from.Y), GetID (pos, from.Y)));
				}
			} else if (from.Y == to.Y && from.X > to.X) {
				for (int pos = from.X; pos >= to.X; pos--) {
					ids.Add(new MapGridTypes.Cell (new Point (pos, from.Y), GetID (pos, from.Y)));
				}
			}
			return ids;
		}

		public Point ScreenToWorld (int x, int y)
		{
			return new Point(x / Globals.CELL_WIDTH,  y / Globals.CELL_HEIGHT);
		}

		public List<Point> GetTunnels()
		{
			var tunnels = new List<Point> ();

			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					if (m_map [x, y].Type == MapGridTypes.ID.Tunnel) {
						tunnels.Add (m_map [x, y].Pos);
					}
				}
			}
			return tunnels;
		}
	}
}