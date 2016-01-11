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
using System.Data.SqlClient;    // MSSQL 연동

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// join.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class join : UserControl
    {
        SqlConnection scon;

        public join()
        {
            InitializeComponent();
            scon = App.scon;
            PageNavigation.CurrentPage = this;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            String Sname = name.Text.ToString();
            String Sid = id.Text.ToString();
            String Spwd = pwd.Text.ToString();

            // 변수 사용하려면 이렇게 적용하기
            String strSql = "INSERT INTO dbo.secmem_member_ssm_project VALUES (N'"+Sname+"', N'"+Sid+"', N'"+Spwd+"')";
            SqlCommand scom = new SqlCommand(strSql, scon);
            scom.Connection = scon;
            scom.ExecuteNonQuery();


            Main.pageNavigation.PushBack(this);

            login loginPage = new login();

            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(loginPage);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // 중복 체크 하기
        }


    }
}