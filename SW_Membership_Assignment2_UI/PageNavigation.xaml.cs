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

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// PageNavigation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PageNavigation : UserControl
    {

        Stack<object> BackPage = new Stack<object>();
        Stack<object> PrePage = new Stack<object>();
        public static object CurrentPage = new object();
        


        public PageNavigation()
        {
            InitializeComponent();
        }

        public void PushBack(object page)
        {
            
            BackPage.Push(page);
            btnPre.IsEnabled = true;
        }


        public void PushPre(object page)
        {
            PrePage.Push(page);
            btnBack.IsEnabled = true;
        }

        public void backClear()
        {
            BackPage.Clear();
        }

        public void preClear()
        {
            PrePage.Clear();
        }



        private void btnBack_Click(object sender, RoutedEventArgs e)
        {//뒤로가기
            if (BackPage.Count != 0)
            {
                
                PrePage.Push(CurrentPage);
                CurrentPage = BackPage.Pop();
                Main.education.gridAssignment1.Columns[6].Visibility = Visibility.Collapsed;
                Main.pageFade.ShowPage((UserControl)CurrentPage);
            }



        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {//앞으로 가기
            
            if (PrePage.Count > 0)
            {
                
                BackPage.Push(CurrentPage);
               
                CurrentPage = PrePage.Pop();
                Main.pageFade.ShowPage((UserControl)CurrentPage);
            }
        }

       
        
    }
}
