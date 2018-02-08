#region Using Statements
using System;

#endregion

namespace mogate
{
    static class Program
	{
		private static GameMogate game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			game = new GameMogate ();
			game.Run ();
		}
	}
}
