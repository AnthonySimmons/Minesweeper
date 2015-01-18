using System;
using Xamarin.Forms;

namespace Minesweeper
{
	public class App
	{
		public static Page GetMainPage ()
		{	
			return new GameGrid (null, 0, 0);

		}
	}
}

