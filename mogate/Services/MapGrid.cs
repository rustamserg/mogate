using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


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

		public class Door
		{
			public Point Pos;
		}

		public class Room
		{
			public Point Pos;
			public int Width;
			public int Height;
			public List<Door> Doors = new List<Door>();
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
		IEnumerable<MapGridTypes.ID> GetBBox (int x, int y);
		IEnumerable<MapGridTypes.ID> GetBCross (int x, int y);

		void AddRoom(MapGridTypes.Room room);
		IEnumerable<MapGridTypes.Room> GetRooms();
	}

	public class MapGrid : IMapGrid
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public Point StairUp { get; set; }
		public Point StairDown { get; set; }

		MapGridTypes.ID[,] m_map;
		List<MapGridTypes.Room> m_rooms;

		public MapGrid (int width, int heigth)
		{
			Width = width;
			Height = heigth;
			Init ();

		}

		public void Init ()
		{
			m_map = new MapGridTypes.ID[Width, Height];
			m_rooms = new List<mogate.MapGridTypes.Room>();
		}

		public MapGridTypes.ID GetID(int x, int y)
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				return MapGridTypes.ID.Blocked;

			return m_map[x, y];
		}

		public void SetID(int x, int y, MapGridTypes.ID id)
		{
			if (x >= 0 && y >= 0 && x < Width && y < Height)
				m_map[x, y] = id;
		}

		public void AddRoom(MapGridTypes.Room room)
		{
			m_rooms.Add(room);
		}

		public IEnumerable<MapGridTypes.Room> GetRooms ()
		{
			return new List<MapGridTypes.Room>(m_rooms);
		}

		public IEnumerable<MapGridTypes.ID> GetBBox (int x, int y)
		{
			List<MapGridTypes.ID> box = new List<MapGridTypes.ID>();
			box.Add (GetID (x - 1, y - 1));
			box.Add (GetID (x, y - 1));
			box.Add (GetID (x + 1, y - 1));
			box.Add (GetID (x - 1, y));
			box.Add (GetID (x + 1, y));
			box.Add (GetID (x - 1, y + 1));
			box.Add (GetID (x, y + 1));
			box.Add (GetID (x + 1, y + 1));

			return box;
		}

		public IEnumerable<MapGridTypes.ID> GetBCross (int x, int y)
		{
			List<MapGridTypes.ID> box = new List<MapGridTypes.ID>();
			box.Add (GetID (x, y - 1));
			box.Add (GetID (x - 1, y));
			box.Add (GetID (x + 1, y));
			box.Add (GetID (x, y + 1));

			return box;
		}
	}
}