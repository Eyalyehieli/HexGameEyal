using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace HexGameProject
{

    /// <summary>
    /// Interaction logic for Game1vsCom.xaml
    /// </summary>
    public partial class Game1vsCom : Window
    {
        string Player;//which player need to play
        string[,] bord;
        bool[,] isChekedBord;
        string winner;
        Stack<Button> undo = new Stack<Button>();
        Queue<Button> winWayRed = new Queue<Button>();
        Queue<Button> winWayBlue = new Queue<Button>();
        public Game1vsCom(string player)
        {
            InitializeComponent();
            comTurn_lbl.Visibility = Visibility.Hidden;
            this.Player = player;
            bord = new string[11, 11];
            isChekedBord = new bool[11, 11];
            resetIsCheckedBord();
        }

        private void resetIsCheckedBord()
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    this.isChekedBord[i, j] = false;
                }
            }
        }

        private Button FindbestButton(int i,int j)
        {
            foreach (Button b in FindVisualChildren<Button>(this))
            { 
                if(Grid.GetRow(b)==i&&Grid.GetColumn(b)==j)
                {
                    return b;
                }
            }
            return null;
        }


        private async void bestMove()
        {
            int depth = 3;
            int bestScore = -Int32.MaxValue;
            int[] move = new int[2];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (bord[i, j] == null)
                    {
                        bord[i, j] = "blue";//aiPlayer
                        int score = miniMax(bord, depth, false, i, j,Int32.MinValue,Int32.MaxValue);
                        bord[i, j] = null;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            move[0] = i;
                            move[1] = j;
                        }
                    }
                }
            }
            Button bestButton = FindbestButton(move[0], move[1]);
            if (bestButton != null)
            {
                paintingButton(bestButton);
            }
            bestButton.IsEnabled = false;
            bord[move[0], move[1]] = "blue";
            winner = thereIsWinner(bestButton,11,-1);
            if (/*winner == "red" ||*/ winner == "blue")
            {
                //if (winner == "red")
                //{
                //    foreach (Button b in winWayRed)
                //    {
                //        b.Background = Brushes.Green;
                //        await Task.Delay(500);
                //    }

                //    while (winWayRed.Count > 0)
                //    {
                //        winWayRed.Dequeue();
                //    }
                //}
                //else
                //{
                foreach (Button b in winWayBlue)
                {
                    b.Background = Brushes.Green;
                    await Task.Delay(500);
                }

                    while (winWayBlue.Count > 0)
                    {
                        winWayBlue.Dequeue();
                        
                    }
                //}
                this.Hide();
                WinnerMessage winnerMessage = new WinnerMessage(winner);
                winnerMessage.ShowDialog();
                this.ShowDialog();
            }
            changingTurn();
            undo.Push(bestButton);
        }

        public int staticEvaluateBoard(string [,] board)
        {
            int value = 10;
            return value;
        }



        private int miniMax(string[,]board,int depth,bool isMaxmizing,int r,int c,int alpha,int beta)
        {
            //int evaluateScore = evaluate(board);
            //if (evaluateScore == 10)
            //{
            //    return evaluateScore;
            //}
            //if (evaluateScore == -10)
            //{
            //return evaluateScore;
            //}
            string result = thereIsWinner(r, c, 11, -1);
            if (result != null)
            {
                return result == "blue" ? 100 : -100; //blue=aiPlayer
            }
            if(depth==0)
            {
                return staticEvaluateBoard(board);
            }
            if (isMaxmizing)
            {
                int bestScore = -Int32.MaxValue;
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        if (bord[i, j] == null)
                        {
                            bord[i, j] = "blue";//aiPlayer
                            int score = miniMax(bord, depth-1, false,i,j,alpha,beta);
                            bord[i, j] = null;
                            bestScore = Math.Max(score, bestScore);
                            alpha = Math.Max(alpha, bestScore);
                        }
                        if (beta <= alpha) { break; }
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = Int32.MaxValue;
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        if (bord[i, j] == null)
                        {
                            bord[i, j] = "red";//humanPlayer
                            int score = miniMax(board, depth-1, true,i,j,alpha,beta);
                            bord[i, j] = null;
                            bestScore = Math.Min(score, bestScore);
                            beta = Math.Min(beta, bestScore);
                        }
                        if (beta <= alpha) { break; }
                    }
                }
                return bestScore;
            }
        }
        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            int row = Grid.GetRow(sender as Button);
            int column = Grid.GetColumn(sender as Button);
            bord[row, column] = this.Player;
            paintingButton(sender as Button);
            winner = thereIsWinner(sender as Button,11,-1);
            (sender as Button).IsEnabled = false;
            if (winner == "red") /*|| winner == "blue")*/
            {
                foreach (Button b in winWayRed)
                {
                    b.Background = Brushes.Green;
                    await Task.Delay(500);
                  
                }
                while (winWayRed.Count > 0)
                {
                    winWayRed.Dequeue();
                }
                //else
                //{
                //    foreach (Button b in winWayBlue)
                //    {
                //        b.Background = Brushes.Green;
                //    }
                //    while (winWayBlue.Count > 0)
                //    {
                //        winWayBlue.Dequeue();
                //    }
                //}
                this.Hide();
                WinnerMessage winnerMessage = new WinnerMessage(winner);
                winnerMessage.ShowDialog();
                this.ShowDialog();
            }

           
            changingTurn();
            undo.Push(sender as Button);
            comTurn_lbl.Visibility = Visibility.Visible;
            await Task.Delay(300);
            bestMove();
            comTurn_lbl.Visibility = Visibility.Hidden;
            await Task.Delay(300);
        }
        private void changingTurn()
        {
            if (this.Player == "red")
            {
                this.Player = "blue";
            }
            else
            {
                this.Player = "red";
            }
        }
        private void paintingButton(Button b)
        {

            if (this.Player == "red")
            {
                b.Background = Brushes.Red;
            }
            else
            {
                b.Background = Brushes.Blue;
            }
        }


        private string thereIsWinner(Button button,int maxDepth,int minDepth)
        {
            resetIsCheckedBord();
            bool redWinnerUp = false, redWinnerDown = false, blueWinnerLeft = false, blueWinnerRight = false;
            if (this.Player == "red")
            {
                redWinnerUp = redCheckingWinnerUp(Grid.GetRow(button), Grid.GetColumn(button),-1);
                isChekedBord[Grid.GetRow(button), Grid.GetColumn(button)] = false;
                redWinnerDown = redCheckingWinnerDown(Grid.GetRow(button), Grid.GetColumn(button),11);
                if (redWinnerDown && redWinnerUp)
                {
                    return "red";
                }
                else
                {
                    while (winWayRed.Count > 0)
                    {
                        winWayRed.Dequeue();
                    }
                }
            }
            else
            {
                blueWinnerLeft = blueCheckingWinnerLeft(Grid.GetRow(button), Grid.GetColumn(button),11);
                isChekedBord[Grid.GetRow(button), Grid.GetColumn(button)] = false;
                blueWinnerRight = blueCheckingWinnerRight(Grid.GetRow(button), Grid.GetColumn(button),-1);
                if (blueWinnerLeft && blueWinnerRight)
                {
                    return "blue";
                }
                else
                {
                    while (winWayBlue.Count > 0)
                    {
                        winWayBlue.Dequeue();
                    }
                }
            }
            return "";
        }

        private string thereIsWinner(int i,int j,int maxDepth,int minDepth)
        {
            resetIsCheckedBord();
            bool redWinnerUp = false, redWinnerDown = false, blueWinnerLeft = false, blueWinnerRight = false;
            if (this.Player == "red")
            {
                redWinnerUp = redCheckingWinnerUp(i,j,-1);
                isChekedBord[i,j] = false;
                redWinnerDown = redCheckingWinnerDown(i,j,11);
                if (redWinnerDown && redWinnerUp)
                {
                    while (winWayRed.Count > 0)
                    {
                        winWayRed.Dequeue();
                    }
                    return "red";
                }
                while (winWayRed.Count > 0)
                {
                    winWayRed.Dequeue();
                }

            }
            else
            {
                blueWinnerLeft = blueCheckingWinnerLeft(i,j,11);
                isChekedBord[i,j] = false;
                blueWinnerRight = blueCheckingWinnerRight(i,j,-1);
                if (blueWinnerLeft && blueWinnerRight)
                {
                    while (winWayBlue.Count > 0)
                    {
                        winWayBlue.Dequeue();
                    }
                    return "blue";
                }

                while (winWayBlue.Count > 0)
                {
                    winWayBlue.Dequeue();
                }
               
            }
            return null;
        }
        private bool redCheckingWinnerDown(int i, int j,int depth)
        {
            int plusJ = j + 1;
            int minusJ = j - 1;
            bool plusJchecking = false;
            bool minusJchecking = false;
            bool plusJhorchecking = false;
            bool minusJhorChecking = false;
            if (i == depth)
            {
                return true;
            }
            if (isChekedBord[i, j] == true)
            {
                return false;
            }
            isChekedBord[i, j] = true;
            if (bord[i, j] == null || bord[i, j] == "blue")
            {
                return false;
            }
            if (plusJ >= 0 && plusJ <= 10)
            {
                plusJchecking = redCheckingWinnerDown(i + 1, j + 1,depth);
                if (bord[i, j + 1] == "red" && isChekedBord[i, j + 1] == false)
                {
                    plusJhorchecking = redCheckingWinnerDown(i, j + 1,depth);
                }
            }
            if (minusJ >= 0 && minusJ <= 10)
            {
                minusJchecking = redCheckingWinnerDown(i + 1, j - 1,depth);
                if (bord[i, j - 1] == "red" && isChekedBord[i, j - 1] == false)
                {
                    minusJhorChecking = redCheckingWinnerDown(i, j - 1,depth);
                }
            }
            bool ret= plusJchecking || redCheckingWinnerDown(i + 1, j,depth) || minusJchecking || minusJhorChecking || plusJhorchecking;
            if(ret)
            {
                winWayRed.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }
        private Button findButtonByIndex(int i, int j)
        {
            foreach (Button b in FindVisualChildren<Button>(this))
            {
                if (Grid.GetRow(b) == i && Grid.GetColumn(b) == j)
                {
                    return b;
                }
            }
            return null;
        }
        public bool redCheckingWinnerUp(int i, int j,int depth)
        {
            int plusJ = j + 1;
            int minusJ = j - 1;
            bool plusJchecking = false;
            bool minusJchecking = false;
            bool plusJhorChecking = false;
            bool minusJhorChecking = false;

            if (i == depth)//-1
            {
                return true;
            }
            if (isChekedBord[i, j] == true)
            {
                return false;
            }
            isChekedBord[i, j] = true;
            if (bord[i, j] == null || bord[i, j] == "blue")
            {
                return false;
            }
            if (plusJ >= 0 && plusJ <= 10)
            {
                plusJchecking = redCheckingWinnerUp(i - 1, j + 1,depth);
                if (bord[i, j + 1] == "red" && isChekedBord[i, j + 1] == false)
                {
                    plusJhorChecking = redCheckingWinnerUp(i, j + 1,depth);
                }
            }
            if (minusJ >= 0 && minusJ <= 10)
            {
                minusJchecking = redCheckingWinnerUp(i - 1, j - 1,depth);
                if (bord[i, j - 1] == "red" && isChekedBord[i, j - 1] == false)
                {
                    minusJhorChecking = redCheckingWinnerUp(i, j - 1,depth);
                }
            }
            bool ret= plusJchecking || redCheckingWinnerUp(i - 1, j,depth) || minusJchecking || minusJhorChecking || plusJhorChecking;
            if (ret)
            {
                winWayRed.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        private bool blueCheckingWinnerLeft(int i, int j,int depth)
        {
            int plusI = i + 1;
            int minusI = i - 1;
            bool plusIchecking = false;
            bool minusIchecking = false;
            bool minusJverChecking = false;
            bool plusJverChecking = false;
            if (j == depth)
            {
                return true;
            }
            if (isChekedBord[i, j] == true)
            {
                return false;
            }
            isChekedBord[i, j] = true;
            if (bord[i, j] == null || bord[i, j] == "red")
            {
                return false;
            }
            if (plusI >= 0 && plusI <= 10)
            {
                plusIchecking = blueCheckingWinnerLeft(i + 1, j + 1,depth);
                if (bord[i + 1, j] == "blue" && isChekedBord[i + 1, j] == false)
                {
                    plusJverChecking = blueCheckingWinnerLeft(i + 1, j,depth);
                }
            }
            if (minusI >= 0 && minusI <= 10)
            {
                minusIchecking = blueCheckingWinnerLeft(i - 1, j + 1,depth);
                if (bord[i - 1, j] == "blue" && isChekedBord[i - 1, j] == false)
                {
                    minusJverChecking = blueCheckingWinnerLeft(i - 1, j,depth);
                }
            }
            bool ret= plusIchecking || blueCheckingWinnerLeft(i, j + 1,depth) || minusIchecking || minusJverChecking || plusJverChecking;
            if (ret)
            {
                winWayBlue.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        public bool blueCheckingWinnerRight(int i, int j,int depth)
        {
            int plusI = i + 1;
            int minusI = i - 1;
            bool plusIcheking = false;
            bool minusIcheking = false;
            bool minusJverChecking = false;
            bool plusJverChecking = false;
            if (j == depth)//-1
            {
                return true;
            }
            if (isChekedBord[i, j] == true)
            {
                return false;
            }
            isChekedBord[i, j] = true;
            if (bord[i, j] == null || bord[i, j] == "red")
            {
                return false;
            }
            if (plusI >= 0 && plusI <= 10)
            {
                plusIcheking = blueCheckingWinnerRight(i + 1, j - 1,depth);
                if (bord[i + 1, j] == "blue" && isChekedBord[i + 1, j] == false)
                {
                    plusJverChecking = blueCheckingWinnerRight(i + 1, j,depth);
                }
            }
            if (minusI >= 0 && minusI <= 10)
            {
                minusIcheking = blueCheckingWinnerRight(i - 1, j - 1,depth);
                if (bord[i - 1, j] == "blue" && isChekedBord[i - 1, j] == false)
                {
                    minusJverChecking = blueCheckingWinnerRight(i - 1, j,depth);
                }
            }
            bool ret= plusIcheking || blueCheckingWinnerRight(i, j - 1,depth) || minusIcheking || plusJverChecking || minusJverChecking;
            if (ret)
            {
                winWayBlue.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (undo.Count != 0)
            {
                Button b = undo.Pop();
                bord[Grid.GetRow(b), Grid.GetColumn(b)] = null;
                b.Background = Brushes.White;
                b.IsEnabled = true;
                if (Player == "red")
                {
                    Player = "blue";
                    await Task.Delay(500);
                    bestMove();
                }
                else
                {
                    Player = "red";
                }
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Player == "red")
            {
                (sender as Button).Background = Brushes.Red;
            }
            else
            {
                (sender as Button).Background = Brushes.Blue;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            //Button b = sender as Button;
            //if(bord[Grid.GetRow(b),Grid.GetColumn(b)]=="red")
            //        {
            //    b.Background = Brushes.Red;
            //}
            //else if(bord[Grid.GetRow(b), Grid.GetColumn(b)] == "blue")
            //{
            //    b.Background = Brushes.Blue;
            //}
            //else
            //{
            //    b.Background = Brushes.White;
            //}
            if ((sender as Button).IsEnabled == true)
            {
                (sender as Button).Background = Brushes.White;
            }
            else if (winner == "red" || winner == "blue")
            {
                return;
            }
        }

        private void resetGrid()
        {
            foreach (Button b in FindVisualChildren<Button>(this))
            {

                if (b.Name != "newGame_btn" && b.Name != "undo_btn" && b.Name != "Instructions_btn")
                {
                    b.IsEnabled = true;
                    b.Background = Brushes.White;
                    bord[Grid.GetRow(b), Grid.GetColumn(b)] = null;
                }
            }
        }
        private void reserUndoStack()
        {
            while (undo.Count != 0)
            {
                undo.Pop();
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Player = "red";
            resetIsCheckedBord();
            resetGrid();
            reserUndoStack();

        }

        private void Instructions_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The game is played on a hexagonal paved board, in each turn, one of the players paints an empty square in red or blue, alternately.\nEach player's goal is to tie the sides of the board marked with his color, in a continuous chain of hexagons of that color.\nThe first player to succeed in the mission, thus preventing the second player from completing his own chain, wins.\nYou can advance from any cell forward, backward, sideways and diagonally, meaning eight directions to advance to victory.\nGood luck !!");
        }
    }
}
