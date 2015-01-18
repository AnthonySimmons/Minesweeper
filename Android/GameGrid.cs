using System;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Timer = System.Timers.Timer;

namespace Minesweeper
{
	public class GameGrid : ContentPage
	{
		private MineField mineField;
		int btnSize = 30;
		int padding = 5;
		int screenWidth;
		int screenHeight;

		public bool IsVertical
		{
			get{ return screenHeight >= screenWidth; }
		}

		bool flagMines;

		Color [] NumMinesColors = new Color[8] {Color.Blue, Color.Blue, Color.Green, Color.Yellow,
			Color.FromRgb(255, 153, 0), Color.Red, Color.Maroon, Color.Maroon};

		Color ColorCellRevealed = Color.FromRgb (140, 150, 200);

		Color ColorCellHidden = Color.FromRgb(0, 0, 150);

		List<Button> MineButtons = new List<Button> ();

		Timer timer = new Timer();

		Label timerLabel = new Label();

		List<Image> MineImages = new List<Image>();

		List<Image> FlagImages = new List<Image>();

		Image FlagImage = new Image();


		Grid grid;

		public GameGrid (MineField nMineField, int width, int height)
		{
			screenWidth = width;
			screenHeight = height;

			mineField = nMineField;

			btnSize = Math.Min (width, height) / mineField.nSize - 5;

			CreateLayout ();

			this.BackgroundColor = Color.FromRgb(0, 64, 128);

			timer = new Timer ();
			timer.Interval = 1000;
			timer.Elapsed += OnTimerElapsed;
		}

		public void Dispose()
		{
			if (timer != null) 
			{
				timer.Elapsed -= OnTimerElapsed;
				timer.Stop ();
				timer = null;
			}
			RemoveButtonsFromGrid (grid);

			MineButtons = new List<Button> ();
			FlagImages = new List<Image> ();
			MineImages = new List<Image> ();
		}

		private void NewGame()
		{
			try
			{
				Dispose ();

				timer = new Timer ();
				timer.Interval = 1000;
				timer.Elapsed += OnTimerElapsed;

				mineField.InitMineField (mineField.nSize);

				if (grid == null) 
				{
					CreateLayout ();
				}
				else 
				{
					AddButtons(grid);
				}
				mineField.TimeCount = 0;
			}
			catch (Exception e) 
			{
				DisplayAlert ("Minesweeper", e.Message, "Ok", "");
			}
		}

		private void CreateLayout()
		{
			StackLayout layout = new StackLayout ();
			grid = new Grid ();
			grid.Padding = new Thickness (padding, padding);
			grid.HorizontalOptions = LayoutOptions.CenterAndExpand;
		 	grid.VerticalOptions = LayoutOptions.CenterAndExpand;
			grid.ColumnSpacing = 1.0;
			grid.RowSpacing = 1.0;

			CreateBackBlackBox (grid);
			AddButtons (grid);
			CreateGameControls (grid);
			CreateTitle (grid);
			layout.Children.Add (grid);

			this.Content = layout;

			if (mineField.GameStarted) 
			{
				timer.Start ();
			}
		}

		private void CreateBackBlackBox(Grid grid)
		{
			BoxView blackBox = new BoxView ();
			blackBox.HeightRequest = btnSize * mineField.nSize;
			blackBox.WidthRequest = btnSize * mineField.nSize;
			blackBox.Color = Color.Black;

			if (IsVertical) 
			{
				grid.Children.Add (blackBox, 0, 10, 5, 15);
			}
			else 
			{
				grid.Children.Add (blackBox, 2, 12, 0, 10);
			}
		}

		private void CreateGameControls(Grid grid)
		{
			Grid subGrid = new Grid ();
			subGrid.ColumnSpacing = 1.0;

			Picker diffPicker = new Picker ();

			diffPicker.Items.Add ("Easy");
			diffPicker.Items.Add ("Medium");
			diffPicker.Items.Add ("Hard");
			diffPicker.SelectedIndexChanged += DifficultyIndexChanged;
			diffPicker.SelectedIndex = diffPicker.Items.IndexOf (mineField.difficulty.ToString());
			diffPicker.Title = diffPicker.Items [diffPicker.SelectedIndex].ToString ();
			diffPicker.HeightRequest = btnSize;
			diffPicker.WidthRequest = btnSize * 2;

			Button newGame = new Button ();
			newGame.Text = "New Game";
			newGame.Clicked += NewGameClicked;
			newGame.HeightRequest = btnSize;
			newGame.WidthRequest = btnSize * 2;

			Label flagLabel = new Label ();
			flagLabel.Text = "Flag:";
			flagLabel.XAlign = TextAlignment.End;
			flagLabel.Font = Font.BoldSystemFontOfSize (18);
			flagLabel.HeightRequest = btnSize;
			flagLabel.WidthRequest = btnSize * 2;
			flagLabel.XAlign = TextAlignment.Center;

			Switch flagSwitch = new Switch ();
			flagSwitch.HeightRequest = btnSize;
			flagSwitch.WidthRequest = btnSize * 2;
			flagSwitch.IsToggled = flagMines;
			flagSwitch.Toggled += FlagSwitchToggled;


			BoxView box = new BoxView ();
			box.HeightRequest = btnSize;
			box.WidthRequest = btnSize;


			Button titleBox = new Button ();
			titleBox.HeightRequest = btnSize;
			titleBox.WidthRequest = btnSize;
			titleBox.IsVisible = false;

			timerLabel = new Label ();
			timerLabel.HeightRequest = btnSize;
			timerLabel.WidthRequest = btnSize * 3;
			timerLabel.Font = Font.BoldSystemFontOfSize (btnSize - 5);

			if (IsVertical) 
			{
				int left = 0, right = 1, top = 2, bottom = 3;

				subGrid.Children.Add (newGame, left, right, top, bottom);
				subGrid.Children.Add (diffPicker, left+1, right+2, top, bottom);

				subGrid.Children.Add (flagLabel, left+3, right+3, top, bottom);
				subGrid.Children.Add (flagSwitch, left+4, right+4, top, bottom);
				subGrid.Children.Add (titleBox, left, right, top-1, bottom-1);
				subGrid.Children.Add (timerLabel, left+3, right+3, top + 1, bottom + 1);

				grid.Children.Add (box, 0, 9, top-1, bottom-1);
				grid.Children.Add (subGrid, 0, 9, 1, 2);
			}
			else 
			{
				int left = 1, right = 2, top = 3, bottom = 4;
				subGrid.Children.Add (newGame, left, right, top, bottom);
				subGrid.Children.Add (diffPicker, left, right, top+1, bottom+1);

				subGrid.Children.Add (timerLabel, left, right, top+2, bottom+2);

				subGrid.Children.Add (flagLabel, left, right, top+3, bottom+3);
				subGrid.Children.Add (flagSwitch, left, right, top+4, bottom+4);
				subGrid.Children.Add (titleBox, left+1, right+1, top, bottom);

				grid.Children.Add (box, left, right);
				grid.Children.Add (subGrid, 0, 1, 1, 8);
			}


		}

		private void CreateTitle(Grid grid)
		{
			Label title = new Label ();

			Font font = Font.BoldSystemFontOfSize (btnSize);

			title.Font = font;
			title.XAlign = TextAlignment.Center;
			title.YAlign = TextAlignment.Center;

			title.Text = "Minesweeper";
			title.TextColor = Color.White;
			title.HeightRequest = btnSize;
			title.Font = Font.BoldSystemFontOfSize (24);
			title.XAlign = TextAlignment.Center;

			if(IsVertical)
			{
				grid.Children.Add (title, 0, 9, 0, 1);
			}
			else
			{
				grid.Children.Add (title, 0, 1, 0, 1);
			}
		}


		private void AddButtons(Grid grid)
		{
			MineButtons = new List<Button> ();

			for (int i = 0; i < mineField.nSize; i++)
			{
				for (int j = 0; j < mineField.nSize; j++)
				{
					Button mButton = new Button();
					mButton.WidthRequest = btnSize;
					mButton.HeightRequest = btnSize;
					//mButton.Location = new Point(i * btnSize + border, j * btnSize + border);
					mButton.Font = Font.SystemFontOfSize (btnSize / 2);
					mButton.BorderRadius = 2;
					mButton.IsEnabled = !(mineField.Cells [i, j].IsRevealed);

					if (!mButton.IsEnabled) 
					{
						mButton.BackgroundColor = ColorCellRevealed;
					}
					else 
					{
						mButton.BackgroundColor = ColorCellHidden;
					}

					mButton.Text = mineField.Cells [i, j].NumMinesString;
					mButton.Clicked += CellClicked;

					if (mButton.Text == "!" || mButton.Text == "X") 
					{
						mButton.TextColor = Color.Red;
					}
					else
					{
						mButton.TextColor = NumMinesColors [mineField.Cells [i, j].NumMines];
					}
					mineField.Cells [i, j].button = mButton;
					mButton.ClassId = "btn_" + i.ToString() + "_" + j.ToString();

					MineButtons.Add (mButton);

					if (IsVertical) 
					{
						if (mButton.Text == "X") 
						{
							AddMineImage (grid, mButton.ClassId, i, j + 5);
						} 
						else if(mButton.Text == "!")
						{
							AddFlagImage (grid, mButton.ClassId, i, j + 5);
						}
						else 
						{
							grid.Children.Add (mButton, i, j + 5);
						}
					}
					else 
					{
						if (mButton.Text == "X")
						{
							AddMineImage (grid, mButton.ClassId, i + 2, j);
						} 
						else if(mButton.Text == "!")
						{
							AddFlagImage (grid, mButton.ClassId, i + 2, j);
						}
						else
						{
							grid.Children.Add (mButton, i + 2, j);
						}
					}
				}
			}
		}


		private void AddFlagImage(Grid grid, string id, int row, int col)
		{
			Image FlagImage = FlagImages.FirstOrDefault (m => m.ClassId == id);

			if (FlagImage == null)
			{
				FlagImage = new Image{ Aspect = Aspect.AspectFit };
				FlagImage.Source = ImageSource.FromFile ("Flag.png");
				FlagImage.HeightRequest = btnSize;
				FlagImage.WidthRequest = btnSize;
				FlagImage.ClassId = id;


				TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer ();
				tapGestureRecognizer.TappedCallback += (s, e) => {
					Button btn = MineButtons.FirstOrDefault(m => m.ClassId == id);
					if(btn != null)
					{
						if(grid.Children.Contains(FlagImage))
						{
							grid.Children.Remove(FlagImage);
						}
						if(!grid.Children.Contains(btn))
						{
							grid.Children.Add(btn);
						}
					}

				};

				FlagImage.GestureRecognizers.Add (tapGestureRecognizer);

				FlagImages.Add (FlagImage);

			}
			grid.Children.Add (FlagImage, row, col);
		}

		private void AddMineImage(Grid grid, string id, int row, int col)
		{
			Image MineImage = MineImages.FirstOrDefault (m => m.ClassId == id);
			if (MineImage == null) 
			{
				MineImage = new Image { Aspect = Aspect.AspectFit };
				MineImage.Source = ImageSource.FromFile ("Mine.png");
				MineImage.HeightRequest = btnSize;
				MineImage.WidthRequest = btnSize;
				MineImage.ClassId = id;

				MineImages.Add (MineImage);
			}
			grid.Children.Add (MineImage, row, col);
		}

		private Cell GetCellFromId(string id)
		{
			int row, col;
			return GetCellFromId (id, out row, out col);
		}

		private Cell GetCellFromId(string id, out int row, out int col)
		{
			//btn_0_0
			string[] strArr = id.Split('_');
			int i, j;
			row = 0;
			col = 0;
			Cell cell = null;
			if (strArr.Length > 2)
			{
				bool success;
				success = Int32.TryParse(strArr[1], out i);
				success &= Int32.TryParse(strArr[2], out j);
				if (success)
				{
					row = i;
					col = j;
					cell = mineField.Cells[row, col];
				}
			}
			return cell;
		}


		private void RevealMineImages()
		{
			if (MineImages != null) 
			{
				foreach (Image img in MineImages) 
				{
					img.SetValue (Image.IsVisibleProperty, true);
				}
			}
		}

		private void RemoveButtonsFromGrid(Grid grid)
		{
			if (MineButtons != null && grid != null) 
			{
				foreach (Button btn in MineButtons) 
				{
					if (grid.Children.Contains (btn)) 
					{
						grid.Children.Remove (btn);
					}
				}
			}

		}

		private void UpdateButtons()
		{
			foreach (Button btn in MineButtons) 
			{
				int row, col;
				Cell cell = GetCellFromId (btn.ClassId, out row, out col);
				if(cell != null && btn != null)
				{
					if (grid != null) 
					{
						if (cell.NumMinesString == "!")
						{
							grid.Children.Remove (btn);

							if (IsVertical) {
								AddFlagImage (grid, btn.ClassId, row, col + 5);
							} else {
								AddFlagImage (grid, btn.ClassId, row + 2, col);
							}
						}
						else if (cell.NumMinesString == "X") 
						{
							grid.Children.Remove (btn);

							if (IsVertical) {
								AddMineImage (grid, btn.ClassId, row, col + 5);
							} else {
								AddMineImage (grid, btn.ClassId, row, col + 5);
							}
						}
						else if (!grid.Children.Contains (btn))  
						{
							Image flagImage = FlagImages.FirstOrDefault (m => m.ClassId == btn.ClassId);
							if (flagImage != null && grid.Children.Contains(flagImage)) 
							{
								grid.Children.Remove (flagImage);
							}

							Image mineImage = MineImages.FirstOrDefault (m => m.ClassId == btn.ClassId);
							if (mineImage != null && grid.Children.Contains (mineImage)) 
							{
								grid.Children.Remove (mineImage);
							}


							if(IsVertical)
							{
								grid.Children.Add (btn, row, col + 5);
							}
							else
							{
								grid.Children.Add (btn, row + 2, col);
							}
						}


					}

					if (btn.Text != cell.NumMinesString && cell.NumMinesString != "!" && cell.NumMinesString != "X") 
					{
						btn.SetValue (Button.TextProperty, cell.NumMinesString);
					}

					if (btn.IsEnabled != cell.IsEnabled) 
					{
						btn.SetValue (Button.IsEnabledProperty, cell.IsEnabled);
					}

					if (!btn.IsEnabled) 
					{
						btn.SetValue (Button.BackgroundColorProperty, ColorCellRevealed);
					}
					else 
					{
						btn.SetValue(Button.BackgroundColorProperty, ColorCellHidden);
					}


					/*if (btn.Font != Font.SystemFontOfSize (btnSize / 2)) 
					{
						btn.SetValue (Button.FontProperty, btnSize / 2);
					}*/
					if ((cell.NumMinesString == "!" || cell.NumMinesString == "X")
					   && btn.TextColor != Color.Red) 
					{
						btn.SetValue (Button.TextColorProperty, Color.Red);
					}
					else if (btn.TextColor != NumMinesColors [cell.NumMines] && !cell.IsFlagged) 
					{
						btn.SetValue (Button.TextColorProperty, NumMinesColors [cell.NumMines]);
					}

				}
			}
		}

		private void NewGameClicked(object sender, EventArgs e)
		{
			NewGame ();
		}

		private void DifficultyIndexChanged(object sender, EventArgs e)
		{
			if(mineField != null)
			{
				Picker diffPicker = (Picker)sender;

				Enum.TryParse (diffPicker.Items [diffPicker.SelectedIndex].ToString (),
					out mineField.difficulty);

			}
		}

		private void RemoveButtonClicks()
		{
			foreach (Button btn in MineButtons) 
			{
				btn.Clicked -= CellClicked;
			}
		}

		private void AddButtonClicks()
		{
			foreach (Button btn in MineButtons) 
			{
				btn.Clicked += CellClicked;
			}
		}

		private void UpdateButtonEvent(object button, object cell)
		{
			Button btn = (Button)button;
			Cell mCell = (Cell)cell;

			btn.SetValue (Button.TextProperty, mCell.NumMinesString);
			btn.SetValue (Button.IsEnabledProperty, mCell.IsEnabled);
		}

		private void FlagSwitchToggled(object sender, EventArgs e)
		{
			flagMines = !flagMines;
		}

		private void CellClicked(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			Cell cell = GetCellFromId (btn.ClassId);
			if (cell != null) 
			{
				List<Cell> revealedCells = new List<Cell>();

				if (!mineField.GameStarted) 
				{
					mineField.GameStarted = true;
				}

				if (flagMines)
				{
					cell.IsFlagged = !cell.IsFlagged;
				}
				else if (cell.IsMine && !cell.IsFlagged)
				{
					mineField.GameOver = true;
					mineField.GameStarted = false;

					RemoveButtonClicks ();
					DisplayAlert ("Minesweeper", "Game Over!", "Ok",  "");
					revealedCells = mineField.GetMines ();
					mineField.TimeCount = 0;
					timer.Stop ();
				}
				else
				{
					cell.IsRevealed = true;
					revealedCells = mineField.RevealEmpties(cell.Row, cell.Column);
				}

				if (mineField.IsComplete ()) 
				{
					mineField.GameStarted = false;
					DisplayAlert ("Minesweeper", String.Format("Completed In {0}", mineField.TimeString()), "Ok", "");
					mineField.TimeCount = 0;
					timer.Stop ();
				}

				if (!timer.Enabled && !mineField.GameOver && mineField.GameStarted) 
				{
					timer.Start ();
				}

				timer.Elapsed -= OnTimerElapsed; 
				UpdateButtons ();
				timer.Elapsed += OnTimerElapsed;

				/*
				if (!mineField.GameOver && !flagMines) 
				{
					timer.Elapsed -= OnTimerElapsed; 
					UpdateButtons ();
					timer.Elapsed += OnTimerElapsed;
				}
				else 
				{
					CreateLayout ();
				}
				*/
			}

		}


		private void OnTimerElapsed(object sender, EventArgs e)
		{
			if (timerLabel != null) 
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
					mineField.TimeCount++;
					timerLabel.SetValue (Label.TextProperty, mineField.TimeString ());
				});
			}
		}

	}
}

