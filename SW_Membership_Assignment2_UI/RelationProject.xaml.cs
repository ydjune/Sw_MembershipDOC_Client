using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Data;
using SW_Membership_Assignment2_UI.Control;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// RelationProject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RelationProject : UserControl
    {
        delegate void SET_BTNCONTENT(TextBlock txText, string msg);  // 연관버튼 델리게이트
        delegate void STOP_LOADING(LoadingAnimation loading);  // 로딩 델리게이트;
       

        DataRowView d;
        DataTable dt;
        
        public RelationProject()
        {
            InitializeComponent();
            Main.setRelationProject(this);
            dt = new DataTable();
            
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {//버튼1

            //if btn.content == 문서1 이라면 안하고
            //else 실행

            
            if (!tb1Text.Text.Equals(""))
            {
                
                string qeury = "select * from dbo.secmem_board_extra_ssm_project where ex_pr_name =N'" + tb1Text.Text + "'";
               
                SqlDataAdapter adapter = new SqlDataAdapter(qeury, App.scon);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                adapter.Fill(dt);
                
                DataView view = new DataView(dt);
                foreach (DataRowView drv in view)
                {
                    d = drv;
                    break;
                }

                AssignmentPage newRPpage1 = new AssignmentPage(d);
                Main.pageNavigation.preClear();
                Main.pageNavigation.backClear();


                Main.ft.Send("CLIENT_TECHNOLOGY\a" + d.Row[0]);
                Main.pageFade.ShowPage(newRPpage1);
            }
            
        }

       
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (!tb2Text.Text.Equals(""))
            {

                string qeury = "select * from dbo.secmem_board_extra_ssm_project where ex_pr_name =N'" + tb2Text.Text + "'";

                SqlDataAdapter adapter = new SqlDataAdapter(qeury, App.scon);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                adapter.Fill(dt);

                DataView view = new DataView(dt);
                foreach (DataRowView drv in view)
                {
                    d = drv;
                    break;
                }

                AssignmentPage newRPpage2 = new AssignmentPage(d);
                Main.pageNavigation.preClear();
                Main.pageNavigation.backClear();

                Main.ft.Send("CLIENT_TECHNOLOGY\a" + d.Row[0]);
                Main.pageFade.ShowPage(newRPpage2);
            }
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (!tb3Text.Text.Equals(""))
            {

                string qeury = "select * from dbo.secmem_board_extra_ssm_project where ex_pr_name =N'" + tb3Text.Text + "'";

                SqlDataAdapter adapter = new SqlDataAdapter(qeury, App.scon);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                adapter.Fill(dt);

                DataView view = new DataView(dt);
                foreach (DataRowView drv in view)
                {
                    d = drv;
                    break;
                }

                AssignmentPage newRPpage3 = new AssignmentPage(d);
                Main.pageNavigation.preClear();
                Main.pageNavigation.backClear();

                Main.ft.Send("CLIENT_TECHNOLOGY\a" + d.Row[0]);
                Main.pageFade.ShowPage(newRPpage3);
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
          
                if (!tb4Text.Text.Equals(""))
                {

                    string qeury = "select * from dbo.secmem_board_extra_ssm_project where ex_pr_name =N'" + tb4Text.Text + "'";

                    SqlDataAdapter adapter = new SqlDataAdapter(qeury, App.scon);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    adapter.Fill(dt);

                    DataView view = new DataView(dt);
                    foreach (DataRowView drv in view)
                    {
                        d = drv;
                        break;
                    }

                    AssignmentPage newRPpage4 = new AssignmentPage(d);
                    Main.pageNavigation.preClear();
                    Main.pageNavigation.backClear();

                    Main.ft.Send("CLIENT_TECHNOLOGY\a" + d.Row[0]);
                    Main.pageFade.ShowPage(newRPpage4);
                }
        }
        


        public void Change_BtnContent(TextBlock txText, string msg)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 Change_BtnContent 메서드를 다시 호출
                    SET_BTNCONTENT d = new SET_BTNCONTENT(Change_BtnContent);
                    Dispatcher.Invoke(d, new object[] { txText, msg });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    txText.Text = msg;
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }

        public void STOP_LOAD(LoadingAnimation loading)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 STOP_LOAD 메서드를 다시 호출
                    STOP_LOADING d = new STOP_LOADING(STOP_LOAD);
                    Dispatcher.Invoke(d, new object[] { loading });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    loadingBar.Visibility = Visibility.Collapsed;
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }


        private void btnCenter_Click(object sender, RoutedEventArgs e)
        {
            
            Main.ft.Send("SEVER_RELATION\a" + Main.assignPage.cpD.Row[0]);
            loadingBar.Visibility = Visibility.Visible;
            
        }

        public void Stop()
        {
            loadingBar.Visibility = Visibility.Collapsed;
        }
    }
}
