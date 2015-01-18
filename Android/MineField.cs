using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper
{
	public enum Difficulty {Easy, Medium, Hard}

	public class MineField
	{
		public Difficulty difficulty;

		public int nSize = 10;

		public int numMines = 25;

		public double minePercent = 0.10;

		public int TimeCount = 0;

		public Cell[,] Cells;

		public bool GameOver;

		public DateTime startTime;

		public bool GameStarted;

		public MineField()
		{ 
			InitMineField (nSize);
		}

		public void InitMineField(int size)
		{
			GameOver = false;
			nSize = size;
			InitCells();
			SetNumMinesDifficulty ();
			DeployMines();
			CountNearMines();
			startTime = DateTime.Now;
		}

		public void SetNumMinesDifficulty()
		{
			switch(difficulty)
			{
				case Difficulty.Easy:
					minePercent = 0.10;
					break;
				case Difficulty.Medium:
					minePercent = 0.20;
					break;
				case Difficulty.Hard:
					minePercent = 0.30;
					break;
			}
			numMines = (int)((nSize * nSize) * minePercent);
		}

		public void InitCells()
		{
			Cells = new Cell[nSize, nSize];
			for (int i = 0; i < nSize; i++)
			{
				for (int j = 0; j < nSize; j++)
				{
					Cells [i, j] = new Cell();
					Cells [i, j].Row = i;
					Cells [i, j].Column = j;
					Cells [i, j].IsRevealed = false;
				}
			}
		}

		public void DeployMines()
		{
			Random rand = new Random(Guid.NewGuid().GetHashCode());
			int n = 0, i, j;
			while (n < numMines)
			{
				i = rand.Next(0, nSize);
				j = rand.Next(0, nSize);
				if (!Cells[i, j].IsMine)
				{
					Cells[i, j].IsMine = true;
					n++;
				}
			}
		}

		public void RevealAllCells()
		{
			for (int i = 0; i < nSize; i++) 
			{
				for (int j = 0; j < nSize; j++) 
				{
					Cells [i, j].IsRevealed = true;
				}
			}
		}

		public bool IsComplete()
		{
			bool complete = true;

			for (int i = 0; i < nSize; i++)
			{
				for (int j = 0; j < nSize; j++)
				{
					if (Cells [i, j].IsMine && !Cells [i, j].IsFlagged) 
					{
						complete = false;
						break;
					}
					if (!Cells [i, j].IsMine && !Cells [i, j].IsRevealed) 
					{
						complete = false;
						break;
					}
				}
				if (!complete) 
				{
					break;
				}
			}

			return complete;
		}


		public string TimeString()
		{
			int min = TimeCount / 60;
			int sec = TimeCount % 60;
			string timestr = min.ToString() + ":";

			if (sec <= 9) 
			{
				timestr += "0";
			}
			timestr += sec.ToString ();

			return timestr;
		}

		public List<Cell> GetMines()
		{
			List<Cell> mineCells = new List<Cell> ();

			for (int i = 0; i < nSize; i++) 
			{
				for (int j = 0; j < nSize; j++) 
				{
					if(Cells[i, j].IsMine)
					{
						Cells [i, j].IsRevealed = true;
						mineCells.Add (Cells [i, j]);
					}
				}
			}
			return mineCells;
		}

		public List<Cell> GetAllCells()
		{
			List<Cell> cells = new List<Cell> ();

			for (int i = 0; i < nSize; i++) 
			{
				for (int j = 0; j < nSize; j++) 
				{
					cells.Add (Cells [i, j]);
				}
			}

			return cells;
		}

		public List<Cell> RevealEmpties(int i, int j)
		{
			return RevealNearbyDFS(i, j);
		}


		private bool ShouldRevealCell(int i, int j)
		{
			bool reveal = false;

			if (i >= 0 && j >= 0 && i < nSize && j < nSize) 
			{
				if (!Cells[i, j].IsMine && !Cells [i, j].IsRevealed) 
				{
					reveal = true;
				}
			}

			return reveal;
		}

		public List<Cell> RevealNearbyDFS(int i, int j)
		{
			List<Cell> revealedCells = new List<Cell> ();
			if (Cells [i, j].NumMines == 0) {
				Stack<Cell> stack = new Stack<Cell> ();
				stack.Push (Cells [i, j]);


				while (stack.Count > 0) {
					Cell cell = stack.Pop ();
					if (!cell.IsMine) {
						revealedCells.Add (cell);
						cell.IsRevealed = true;

						if (cell.NumMines == 0) {

							if (ShouldRevealCell (cell.Row - 1, cell.Column)) {
								stack.Push (Cells [cell.Row - 1, cell.Column]);
							}
							if (ShouldRevealCell (cell.Row, cell.Column - 1)) {
								stack.Push (Cells [cell.Row, cell.Column - 1]);
							}
							if (ShouldRevealCell (cell.Row + 1, cell.Column)) {
								stack.Push (Cells [cell.Row + 1, cell.Column]);
							}
							if (ShouldRevealCell (cell.Row, cell.Column + 1)) {
								stack.Push (Cells [cell.Row, cell.Column + 1]);
							}
						}
					}
				}
			}
			return revealedCells;
		}


		public void CountNearMines()
		{
			for (int i = 0; i < nSize; i++)
			{
				for (int j = 0; j < nSize; j++)
				{
						if (i > 0)
						{
							if (Cells[i - 1, j].IsMine) { Cells[i, j].NumMines++; }
						}
						if (j > 0)
						{
							if (Cells[i, j - 1].IsMine) { Cells[i, j].NumMines++; }
						}
						if (i < nSize - 1)
						{
							if (Cells[i + 1, j].IsMine) { Cells[i, j].NumMines++; }
						}
						if (j < nSize - 1)
						{
							if (Cells[i, j + 1].IsMine) { Cells[i, j].NumMines++; }
						}
						if (i > 0 && j > 0)
						{
							if (Cells[i - 1, j - 1].IsMine) { Cells[i, j].NumMines++; }
						}
						if (i > 0 && j < nSize - 1)
						{
							if (Cells[i - 1, j + 1].IsMine) { Cells[i, j].NumMines++; }    
						}
						if (i < nSize - 1 && j > 0)
						{
							if (Cells[i + 1, j - 1].IsMine) { Cells[i, j].NumMines++; }
						}
						if (i < nSize - 1 && j < nSize - 1)
						{
							if (Cells[i + 1, j + 1].IsMine) { Cells[i, j].NumMines++;  }
						}

				}
			}
		}
	}
}
