using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading;


namespace Minesweeper.Android
{
	[Activity (Label = "Minesweeper.Android.Android", MainLauncher = true)]
	//[Activity (Label = "Sudoku", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : AndroidActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			int width = ConvertPixelsToDp (Resources.DisplayMetrics.WidthPixels);
			int height = ConvertPixelsToDp (Resources.DisplayMetrics.HeightPixels);

			GameGrid gameGrid = new GameGrid (MineFieldFactory.MineField, width, height);

			Xamarin.Forms.Forms.Init (this, bundle);

			MineFieldFactory.Height = height;
			MineFieldFactory.Width = width;

			SetPage (MineFieldFactory.GameGrid);
		}

		private int ConvertPixelsToDp(float pixels)
		{
			return (int)((pixels) / Resources.DisplayMetrics.Density);
		}
	}
}

