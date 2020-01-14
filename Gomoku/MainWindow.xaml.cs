using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Move = Gomoku.GameLogic.Move;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //const int nRows = 15, nCols = 15;
        const int nRows = 25, nCols = 25;
        GameLogic gameLogic;
        Computer computer;
        Button[,] buttonBoard;
        Stack<Move> undoMoves;
        Stack<Move> redoMoves;
        Uri xResourceUri;
        Uri oResourceUri;
        void InitializeButton()
        {
            Application.Current.MainWindow.Height = nRows * 30 + 20;
            Application.Current.MainWindow.Width = nCols * 30;
            gameLogic = new GameLogic(nRows, nCols, 2);
            int comLV = 1;
            if (File.Exists("setting.cfg"))
            {
                FileStream file = new FileStream("setting.cfg", FileMode.Open, FileAccess.Read);
                comLV = file.ReadByte();
                file.Close();
            }
            else
            {
                FileStream file = new FileStream("setting.cfg", FileMode.OpenOrCreate, FileAccess.Write);
                file.WriteByte(1);
                file.Close();
            }
            computer = new Computer(gameLogic, comLV);
            buttonBoard = new Button[nRows, nCols];
            undoMoves = new Stack<Move>();
            redoMoves = new Stack<Move>();
            xResourceUri = new Uri("Resources/Cross.png", UriKind.Relative);
            oResourceUri = new Uri("Resources/Circle.png", UriKind.Relative);
            UndoButton.IsEnabled = false;
            RedoButton.IsEnabled = false;

            GridLengthConverter gridLengthConverter = new GridLengthConverter();
            ThicknessConverter thicknessConverter = new ThicknessConverter();

            for (int i = 0; i < nRows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = (GridLength)gridLengthConverter.ConvertFrom("*");
                mainGrid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < nCols; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                mainGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    Button newButton = new Button();
                    newButton.Name = "Button" + i.ToString() + "_" + j.ToString();

                    newButton.BorderThickness = (Thickness)thicknessConverter.ConvertFrom(0.5);
                    newButton.Background = Brushes.White;
                    newButton.Click += NextMove;
                    newButton.Focusable = false;
                    newButton.Style = this.FindResource("SetButton") as Style;

                    Grid.SetRow(newButton, i);
                    Grid.SetColumn(newButton, j);
                    
                    mainGrid.Children.Add(newButton);
                    buttonBoard[i, j] = newButton;
                }
            }
        }
        
        public void NextMove(object sender, RoutedEventArgs e)
        {
            Button sendButton = (Button)sender;
            int r = Grid.GetRow(sendButton);
            int c = Grid.GetColumn(sendButton);
            if (!gameLogic.IsEmpty(r, c))
                return;
            ApplyMove(r, c);
            if (gameLogic.CheckWin(r, c))
            {
                MessageBox.Show(Application.Current.MainWindow, "Player won", "End Game");
                mainGrid.IsEnabled = false;
            }
            ComputerNextMove();
            UndoButton.IsEnabled = true;
            redoMoves.Clear();
        }

        public void ApplyMove(int r, int c)
        {
            if(undoMoves.Count > 0)
                buttonBoard[undoMoves.Peek().row, undoMoves.Peek().column].Background = Brushes.White;
            undoMoves.Push(new Move(r,c));

            Image Source = new Image();
            Source.Source = new BitmapImage(gameLogic.isXMove?xResourceUri:oResourceUri);
            buttonBoard[r, c].Background = Brushes.Yellow;
            buttonBoard[r, c].Content = Source;
            gameLogic.NextMove(r, c);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
        }

        private void ComputerNextMove()
        {
            if (!mainGrid.IsEnabled)
                return;
            Move move = computer.NextMove();
            ApplyMove(move.row, move.column);
            if (gameLogic.CheckWin(move.row, move.column))
            {
                MessageBox.Show(Application.Current.MainWindow, "Computer won", "End Game");
                mainGrid.IsEnabled = false;
            }
        }

        private void NewGame(bool isComputerTurn)
        {
            mainGrid.IsEnabled = true;
            foreach (object o in LogicalTreeHelper.GetChildren(mainGrid))
            {
                if (o is Button)
                {
                    Button btn = (Button)o;
                    btn.Background = Brushes.White;
                    btn.Content = "";
                }
            }

            gameLogic = new GameLogic(nRows, nCols, isComputerTurn?1:2);

            FileStream file = new FileStream("setting.cfg", FileMode.Open, FileAccess.Read);
            int comLV = file.ReadByte();
            file.Close();

            computer = new Computer(gameLogic, comLV);
            undoMoves = new Stack<Move>();
            redoMoves = new Stack<Move>();

            UndoButton.IsEnabled = false;
            RedoButton.IsEnabled = false;
            if (isComputerTurn)
                ApplyMove(nRows / 2, nCols / 2);
        }

        private void NewGameComputer(object sender, RoutedEventArgs e)
        {
            NewGame(true);
        }

        private void NewGamePlayer(object sender, RoutedEventArgs e)
        {
            NewGame(false);
        }

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Author: Vo Minh Trieu\nRelease: 1.0", "About");
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ChangeLV(int lv)
        {
            computer.ChangeLV(lv);
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (undoMoves.Count <= 1)
                return;
            mainGrid.IsEnabled = true;
            Move move = undoMoves.Pop();
            gameLogic.RemoveMove(move.row, move.column);
            buttonBoard[move.row, move.column].Background = Brushes.White;
            buttonBoard[move.row, move.column].Content = "";
            redoMoves.Push(move);

            move = undoMoves.Pop();
            gameLogic.RemoveMove(move.row, move.column);
            buttonBoard[move.row, move.column].Background = Brushes.White;
            redoMoves.Push(move);
            buttonBoard[move.row, move.column].Content = "";

            if(undoMoves.Count > 0)
                buttonBoard[undoMoves.Peek().row, undoMoves.Peek().column].Background = Brushes.Yellow;
            if(undoMoves.Count <= 1)
                UndoButton.IsEnabled = false;
            RedoButton.IsEnabled = true;
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            Move move = redoMoves.Pop();
            ApplyMove(move.row, move.column);
            if (redoMoves.Count > 0)
            {
                move = redoMoves.Pop();
                ApplyMove(move.row, move.column);
            }
            buttonBoard[move.row, move.column].Background = Brushes.Yellow;
            if (redoMoves.Count == 0)
                RedoButton.IsEnabled = false;
            UndoButton.IsEnabled = true;
        }

        private void OpenSetting(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow();
            window.Owner = this;
            window.Left = this.Left + this.Width / 2 - window.Width/2;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;
            window.Show();
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeButton();
        }
    }
}
