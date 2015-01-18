using System;
using System.IO;

namespace Minesweeper.Android
{
	public static class MineFieldFactory
	{
		private static MineField mineField;

		public static MineField MineField
		{
			get 
			{
				if (mineField == null) 
				{
					mineField = new MineField ();
				}
				return mineField;
			}
		}

		private static string AppDataPath = Environment.SpecialFolder.ApplicationData.ToString();

		private static string savedGameTimeFile;

		public static string SavedGameTimeFile
		{
			get
			{
				string dirname = Path.Combine (AppDataPath, "Minesweeper");
				savedGameTimeFile = Path.Combine (dirname, "GameTime.txt");
				if (!Directory.Exists (dirname)) 
				{
					Directory.CreateDirectory (dirname);
				}
				return savedGameTimeFile;
			}
		}


		public static int Width, Height;

		private static GameGrid gameGrid;

		public static GameGrid GameGrid
		{
			get
			{ 
				if (gameGrid != null) 
				{
					gameGrid.Dispose ();
					gameGrid = null;
					GC.Collect ();
				}


				gameGrid = new GameGrid (MineField, Width, Height);

				return gameGrid;
			}
		}

	}
}

