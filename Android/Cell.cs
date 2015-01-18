using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Minesweeper
{
	public class Cell
	{
		public delegate void UpdateButtonEvent(object button, object cell);

		public Button button;

		public bool IsMine;

		public int Row;

		public int Column;

		public int NumMines;

		public bool IsFlagged;

		private bool _isRevealed;

		public bool IsRevealed
		{
			get { return _isRevealed; }
			set
			{
				_isRevealed = value; 
			}
		}


		public string NumMinesString
		{
			get
			{
				string mineString = String.Empty;
				if(IsRevealed)
				{
					if (NumMines > 0) 
					{
						mineString = NumMines.ToString (); 
					}
					if (IsMine) 
					{
						mineString = "X";
					}
				}
				if (IsFlagged) 
				{
					mineString = "!";
				}
				return mineString;
			}
			set
			{ 
				if (!string.IsNullOrEmpty (value)) 
				{
					int num;
					if (Int32.TryParse (value, out num)) 
					{
						NumMines = num;
					}
				}
			}
		}

		public bool IsEnabled
		{
			get{ return !IsRevealed; }
		}
		 
	}
}
