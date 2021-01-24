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

namespace HexGameProject
{
    /// <summary>
    /// Interaction logic for WinnerMessage.xaml
    /// </summary>
    public partial class WinnerMessage : Window
    {
        string winner;
        public WinnerMessage(string winner)
        {
            InitializeComponent();
            this.winner = winner;
            if(this.winner=="red")
            {
                winnerName_lbl.Content = "Red Player";
            }
            else
            {
                winnerName_lbl.Content = "Blue Player";
            }
        }
    }
}
