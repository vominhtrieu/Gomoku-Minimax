using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //const int nRows = 15, nCols = 15;
        const int nRows = 17, nCols = 13;
        GameLogic gameLogic;
        Computer computer;
        Button[,] buttonBoard;
        Stack<int> listMoveX;
        Stack<int> listMoveY;
        Uri xResourceUri;
        Uri oResourceUri;
        void InitializeButton()
        {
            Application.Current.MainWindow.Height = nRows * 30 + 20;
            Application.Current.MainWindow.Width = nCols * 30;
            gameLogic = new GameLogic(nRows, nCols, 2);
            computer = new Computer(gameLogic, nRows, nCols);
            buttonBoard = new Button[nRows, nCols];
            listMoveX = new Stack<int>();
            listMoveY = new Stack<int>();
            xResourceUri = new Uri("Resources/Cross.png", UriKind.Relative);
            oResourceUri = new Uri("Resources/Circle.png", UriKind.Relative);

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
        }

        public void ApplyMove(int r, int c)
        {
            if(listMoveY.Count > 0)
                buttonBoard[listMoveY.Peek(), listMoveX.Peek()].Background = Brushes.White;
            listMoveY.Push(r);
            listMoveX.Push(c);

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
            int[] pos = computer.NextMove(buttonBoard);
            ApplyMove(pos[0], pos[1]);
            if (gameLogic.CheckWin(pos[0], pos[1]))
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
            computer = new Computer(gameLogic, nRows, nCols);
            listMoveX = new Stack<int>();
            listMoveY = new Stack<int>();
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

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (listMoveX.Count <= 1)
                return;
            mainGrid.IsEnabled = true;
            int x = listMoveX.Pop();
            int y = listMoveY.Pop();
            gameLogic.RemoveMove(y, x);
            buttonBoard[y, x].Background = Brushes.White;
            buttonBoard[y, x].Content = "";

            x = listMoveX.Pop();
            y = listMoveY.Pop();
            gameLogic.RemoveMove(y, x);
            buttonBoard[y, x].Background = Brushes.White;
            buttonBoard[y, x].Content = "";
            buttonBoard[listMoveY.Peek(), listMoveX.Peek()].Background = Brushes.Yellow;
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeButton();
        }
    }
}
