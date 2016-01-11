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
using System.Windows.Shapes;
using WpfPageTransitions;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// board.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class board : UserControl
    {

        Main main = null;
        public board()
        {
            InitializeComponent();
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {

            education_board ebo_page = new education_board();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();

            Main.pageFade.ShowPage(ebo_page);
        }


    }
}
