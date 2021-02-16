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
using System.Collections;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace HexGameProject
{
    /// <summary>
    /// Interaction logic for Game1vs1.xaml
    /// </summary>
    public partial class Game1vs1 : Window
    {
        string colorPlayer;//which player need to play
        string[,] bord;
        bool[,] isChekedBord;
        string winner;
        Stack<Button> undo = new Stack<Button>();
        Queue<Button> winWayRed = new Queue<Button>();
        Queue<Button> winWayBlue = new Queue<Button>();

        public Game1vs1(string colorPlayer)
        {
            InitializeComponent();
            this.colorPlayer = colorPlayer;
            bord = new string[11, 11];
            isChekedBord = new bool[11, 11];
            resetIsCheckedBord();
            //this.DataContext = this;
            //CurrentPlayerColor = Brushes.Green;
        }

        //public Brush CurrentPlayerColor
        //{
        //    get; set;
        //}

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
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           
            int row = Grid.GetRow(sender as Button);
            int column = Grid.GetColumn(sender as Button);
            bord[row, column] = this.colorPlayer;
            paintingButton(sender as Button);
            winner = thereIsWinner(sender as Button);
            (sender as Button).IsEnabled = false;
            if (winner == "red" || winner == "blue")
            { 
                if (winner == "red")
                {                 
                    foreach (Button b in winWayRed)
                    {
                        b.Background = Brushes.Green;
                        await Task.Delay(500);   
                    }

                    while (winWayRed.Count>0)
                    {
                        winWayRed.Dequeue();
                    }
                }
                else
                {
                    foreach (Button b in winWayBlue)
                    {
                        b.Background = Brushes.Green;
                        await Task.Delay(500);
                    }

                    while (winWayBlue.Count > 0)
                    {
                        winWayBlue.Dequeue();
                    }
                }
                this.Hide();
                WinnerMessage winnerMessage = new WinnerMessage(winner);
                winnerMessage.ShowDialog();
                this.ShowDialog();
            }
            changingTurn();
            undo.Push(sender as Button);
        }


        private void changingTurn()
        {
            if (this.colorPlayer == "red")
            {
                this.colorPlayer = "blue";
            }
            else
            {
                this.colorPlayer = "red";
            }
        }
        private void paintingButton(Button b)
        {

            if (this.colorPlayer == "red")
            {
                b.Background = Brushes.Red;
            }
            else
            {
                b.Background = Brushes.Blue;
            }
        }

        private string thereIsWinner(Button button)
        {
            resetIsCheckedBord();
            bool redWinnerUp = false, redWinnerDown = false, blueWinnerLeft = false, blueWinnerRight = false;
            if (this.colorPlayer=="red")
            {
                redWinnerUp = redCheckingWinnerUp(Grid.GetRow(button), Grid.GetColumn(button));
                isChekedBord[Grid.GetRow(button), Grid.GetColumn(button)] = false;
                redWinnerDown = redCheckingWinnerDown(Grid.GetRow(button), Grid.GetColumn(button));
                if(redWinnerDown&&redWinnerUp)
                {
                    return "red";
                }
                else
                {
                    while(winWayRed.Count>0)
                    {
                        winWayRed.Dequeue();
                    }
                }
            }
            else
            {
                blueWinnerLeft = blueCheckingWinnerLeft(Grid.GetRow(button), Grid.GetColumn(button));
                isChekedBord[Grid.GetRow(button), Grid.GetColumn(button)] = false;
                blueWinnerRight = blueCheckingWinnerRight(Grid.GetRow(button), Grid.GetColumn(button));
                if(blueWinnerLeft&&blueWinnerRight)
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
        private bool redCheckingWinnerDown(int i, int j)
        {
            int plusJ = j + 1;
            int minusJ = j - 1;
            bool plusJchecking = false;
            bool minusJchecking = false;
            bool plusJhorchecking = false;
            bool minusJhorChecking = false;
            if (i == 11)
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
                plusJchecking = redCheckingWinnerDown(i + 1, j + 1);
                if (bord[i, j + 1] == "red" && isChekedBord[i, j + 1] == false)
                {
                    plusJhorchecking = redCheckingWinnerDown(i, j + 1);
                }
            }
            if (minusJ >= 0 && minusJ <= 10)
            {
                minusJchecking = redCheckingWinnerDown(i + 1, j - 1);
                if (bord[i, j - 1] == "red" && isChekedBord[i, j - 1] == false)
                {
                    minusJhorChecking = redCheckingWinnerDown(i, j - 1);
                }
            }
           bool ret= plusJchecking || redCheckingWinnerDown(i + 1, j) || minusJchecking || minusJhorChecking || plusJhorchecking;
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
        public bool redCheckingWinnerUp(int i, int j)
        {
            int plusJ = j + 1;
            int minusJ = j - 1;
            bool plusJchecking = false;
            bool minusJchecking = false;
            bool plusJhorChecking = false;
            bool minusJhorChecking = false;

            if (i == -1)
            {
                return true;
            }
            if(isChekedBord[i,j]==true)
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
                plusJchecking = redCheckingWinnerUp(i - 1, j + 1);
                if (bord[i, j + 1] == "red" && isChekedBord[i, j + 1] == false)
                {
                    plusJhorChecking = redCheckingWinnerUp(i, j + 1);
                }
            }
            if (minusJ >= 0 && minusJ <= 10)
            {
                minusJchecking = redCheckingWinnerUp(i - 1, j - 1);
                if (bord[i, j - 1] == "red" && isChekedBord[i, j - 1] == false)
                {
                    minusJhorChecking = redCheckingWinnerUp(i, j - 1);
                }
            }
            bool ret= plusJchecking || redCheckingWinnerUp(i - 1, j) || minusJchecking || minusJhorChecking || plusJhorChecking;
            if (ret)
            {
                winWayRed.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        private bool blueCheckingWinnerLeft(int i, int j)
        {
            int plusI = i + 1;
            int minusI = i - 1;
            bool plusIchecking = false;
            bool minusIchecking = false;
            bool minusJverChecking = false;
            bool plusJverChecking = false;
            if (j == 11)
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
                plusIchecking = blueCheckingWinnerLeft(i + 1, j + 1);
                if (bord[i + 1, j] == "blue" && isChekedBord[i + 1, j] == false)
                {
                    plusJverChecking = blueCheckingWinnerLeft(i + 1, j);
                }
            }
            if (minusI >= 0 && minusI <= 10)
            {
                minusIchecking = blueCheckingWinnerLeft(i - 1, j + 1);
                if (bord[i - 1, j] == "blue" && isChekedBord[i - 1, j] == false)
                {
                    minusJverChecking = blueCheckingWinnerLeft(i - 1, j);
                }
            }
            bool ret= plusIchecking || blueCheckingWinnerLeft(i, j + 1) || minusIchecking || minusJverChecking || plusJverChecking;
            if(ret)
            {
                winWayBlue.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        public bool blueCheckingWinnerRight(int i, int j)
        {
            int plusI = i + 1;
            int minusI = i - 1;
            bool plusIcheking = false;
            bool minusIcheking = false;
            bool minusJverChecking = false;
            bool plusJverChecking = false;
            if (j == -1)
            {
                return true;
            }
            if(isChekedBord[i,j]==true)
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
                plusIcheking = blueCheckingWinnerRight(i + 1, j - 1);
                if (bord[i + 1, j] == "blue" && isChekedBord[i + 1, j] == false)
                {
                    plusJverChecking = blueCheckingWinnerRight(i + 1, j);
                }
            }
            if (minusI >= 0 && minusI <= 10)
            {
                minusIcheking = blueCheckingWinnerRight(i - 1, j - 1);
                if (bord[i - 1, j] == "blue" && isChekedBord[i - 1, j] == false)
                {
                    minusJverChecking = blueCheckingWinnerRight(i - 1, j);
                }
            }
            bool ret= plusIcheking || blueCheckingWinnerRight(i, j - 1) || minusIcheking || plusJverChecking || minusJverChecking;
            if (ret)
            {
                winWayBlue.Enqueue(findButtonByIndex(i, j));
            }
            return ret;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (undo.Count != 0)
            {
                Button b = undo.Pop();
                bord[Grid.GetRow(b), Grid.GetColumn(b)] = null;
                b.Background = Brushes.White;
                b.IsEnabled = true;
                if (colorPlayer == "red")
                {
                    colorPlayer = "blue";
                }
                else
                {
                    colorPlayer = "red";
                }
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (colorPlayer == "red")
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

                if (b.Name != "newGame_btn" && b.Name != "undo_btn"&&b.Name!= "Instructions_btn")
                { 
                    b.IsEnabled = true;
                    b.Background = Brushes.White;
                    bord[Grid.GetRow(b), Grid.GetColumn(b)] = null;
                }
            }
        }
        private void reserUndoStack()
        {
            while(undo.Count!=0)
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
            this.colorPlayer = "red";
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
