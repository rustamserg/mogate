using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace mogate
{
	[Register ("AppDelegate")]
	class Program : UIApplicationDelegate 
	{
		private GameMogate game;

		public override void FinishedLaunching (UIApplication app)
		{
			// Fun begins..
			game = new GameMogate();
			game.Run();
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
