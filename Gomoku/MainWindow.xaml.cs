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
        const int nRows = 15, nCols = 15;
        ImageBrush XSource, OSource;
        GameLogic gameLogic;
        Computer computer;
        Button[,] buttonBoard;
        Stack<int> listMoveX;
        Stack<int> listMoveY;

        void InitializeButton()
        {
            gameLogic = new GameLogic(nRows, nCols);
            computer = new Computer(gameLogic, nRows, nCols);
            buttonBoard = new Button[nRows, nCols];
            listMoveX = new Stack<int>();
            listMoveY = new Stack<int>();

            Uri xResourceUri = new Uri("Resources/Cross.png", UriKind.Relative);
            Uri oResourceUri = new Uri("Resources/Circle.png", UriKind.Relative);
            StreamResourceInfo streamInfoX = Application.GetResourceStream(xResourceUri);
            StreamResourceInfo streamInfoO = Application.GetResourceStream(oResourceUri);

            BitmapFrame temp1 = BitmapFrame.Create(streamInfoX.Stream);
            BitmapFrame temp2 = BitmapFrame.Create(streamInfoO.Stream);
            XSource = new ImageBrush();
            OSource = new ImageBrush();
            XSource.ImageSource = temp1;
            OSource.ImageSource = temp2;

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
            ComputerNextMove();
        }

        public void ApplyMove(int r, int c)
        {
            listMoveY.Push(r);
            listMoveX.Push(c);
            if (gameLogic.isXMove)
            {
                buttonBoard[r, c].Background = XSource;
            }
            else
            {
                buttonBoard[r, c].Background = OSource;
            }

            gameLogic.NextMove(r, c);

            if (gameLogic.CheckWin(r, c))
            {
                MessageBox.Show(Application.Current.MainWindow, "Player won", "End Game");
                mainGrid.IsEnabled = false;
            }
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
        }

        private void ComputerNextMove()
        {
            if (!mainGrid.IsEnabled)
                return;
            int[] pos = computer.NextMove(buttonBoard);
            gameLogic.NextMove(pos[0], pos[1]);

            listMoveY.Push(pos[0]);
            listMoveX.Push(pos[1]);

            buttonBoard[pos[0], pos[1]].Background = gameLogic.isXMove ? OSource : XSource;
            
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
                }
            }

            gameLogic = new GameLogic(nRows, nCols);
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

            x = listMoveX.Pop();
            y = listMoveY.Pop();
            gameLogic.RemoveMove(y, x);
            buttonBoard[y, x].Background = Brushes.White;
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeButton();
        }
    }
}
