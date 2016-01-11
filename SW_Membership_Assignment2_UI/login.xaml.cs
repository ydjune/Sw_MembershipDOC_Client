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

using WpfPageTransitions;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class login : UserControl
    {
        // db 연결 하기
        
        private FileTransferServer ft = null;
        SqlConnection scon;

        Main main;
        public login()
        {
            InitializeComponent();
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용
           // ft = ;
            PageNavigation.CurrentPage = this;

            scon = App.scon;
           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            join input = new join(); //회원가입 페이지

            

            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();

            Main.pageFade.ShowPage(input);
           
            //NavigationService.Navigate(new Uri("join.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // 회원정보 DB 비교한 후 로그인 하기
            // 비교 쿼리문 넣기
            // 연동한 후 종료 시키지 말기

            

            String userId = userid.Text;
            String userPwd = userpwd.Text;
            String strSql = string.Format("select * from dbo.secmem_member_ssm_project where id='{0}' and pwd='{1}'", userId, userPwd);
            SqlCommand scom = new SqlCommand(strSql, scon);
            scom.Connection = scon;
             
            // 쿼리의 결과를 받아온다 : password 결과 값 받아옴
            SqlDataReader reader = scom.ExecuteReader();
            if (reader.HasRows != false)
            {
                Console.WriteLine("information 있음");

                board newBoard = new board();

                Main.pageNavigation.PushBack(this);
                Main.pageNavigation.preClear();
                Main.pageNavigation.backClear();
                //페이지 네이게이트를 위한 스택 쌓기
                Main.pageFade.ShowPage(newBoard);

            }
            else
            {
                Console.WriteLine("information 없음");
                MessageBox.Show("입력하신 정보가 없습니다.");
            }
            reader.Close();


           
        }



    }
}
