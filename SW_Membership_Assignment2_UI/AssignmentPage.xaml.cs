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
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Data.SqlClient;
using WpfPageTransitions;
using ElementsCloud;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// AssignmentPage.xaml에 대한 상호 작용 논리
    /// </summary>효
    public partial class AssignmentPage : UserControl
    {

        FileTransferServer ft = null;

        public static PageTransition pt = null;

        delegate void SET_TECHTERM(Label lbTag, string msg);  // 과제 단어 태그 등록 델리게이트

        delegate void UPDATA_TAGCLOUD();  // 태그클라우드 단어 델리게이트

        delegate void CONTROL_PROGRESSBAR(string msg);  // 승인 프로그레스바 델리게이트



        SqlConnection scon;
        DataRowView data;

        string path = null;
        public DataRowView cpD = null;
        public AssignmentPage(DataRowView d)
        {
            InitializeComponent();
            data = d;
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용
            cpD = d; //연관과제에서 이벤트를 위한 내용 접근용 변수

            //#################연관과제 페이지페이드효과 ####################
            RelationProject rProject = new RelationProject();
            pt = relationPageFade;
            pt.ShowPage(rProject);//연관과제 보여주기 위한 페이지 전환
            // #################페이지페이드효과 ####################

            scon = App.scon;

            ft = Main.ft;
            Main.setAssignPage(this);
            

            if (d.Row[10].Equals("대기중"))
            {
                relationPageFade.IsEnabled = false;
                btnAssignmentAgree.IsEnabled = true;
            }
            else if (d.Row[10].Equals("진행중"))
            {
                relationPageFade.IsEnabled = true;
                btnAssignmentAgree.IsEnabled = false;
            }

           // 서버에 연결해서 경로 탐색 후 파일 이름 가져오기
           // SocketConnection();
            string fileName = System.IO.Path.GetFileNameWithoutExtension(@d.Row[11].ToString());
            string fileExtension = System.IO.Path.GetExtension(@d.Row[11].ToString());
            
            // 총 11개
            // 0            1           2           3               4               5               6               7                    8                   9               10        11     12
            // wi_id    ex_pr_name  ex_pr_type  ex_description  ex_start_date   ex_end_date     ex_pr_status    ex_start_file_idx   ex_end_file_idx     ex_platform     ex_is_used(진행OR대기중)  ex_path  ex_data (과제요약)
            name.Content = d.Row[1];
            type.Content = d.Row[2];
            description.Content = d.Row[3];
            status.Content = d.Row[6];

            tool.Content = d.Row[9];

            used.Content = d.Row[10];

            path = d.Row[11].ToString();
			
            
			
            btnDownload.Content = new TextBlock()
            {
                Text = fileName + fileExtension,
                TextWrapping = TextWrapping.Wrap
            };


            label1.Content = "과제게시판 > 과제 DB > " + d.Row[1];
            Main.relationPage.Change_BtnContent(Main.relationPage.tbCenterText, d.Row[1].ToString()); //중심버튼 이름 바꿔주기
            
            if (data.Row[10].Equals("진행중"))
            {
                cloud.Run();//태그 구름 동작 메소드
                TagCloud();
                del1.Visibility = Visibility.Visible;
                del2.Visibility = Visibility.Visible;
                del3.Visibility = Visibility.Visible;
                del4.Visibility = Visibility.Visible;
                del5.Visibility = Visibility.Visible;
            }
        }

        public void TagCloud()
        {
            List<string> top20 = new List<string>();


            if (data.Row[10].Equals("진행중"))
            {
                Point point = new Point(111, 111);
                
                String strSelect = "select top 20 technology_word from dbo.secmem_technology_db_ssm_project where technology_document =" + data.Row[0] + " and technology_flag !=" + 0 + " order by technology_TFIDF desc ";
                SqlCommand scomSelect = new SqlCommand(strSelect, scon);
                scomSelect.Connection = scon;
                //returnValue = (int)scomSelect.ExecuteScalar();
                SqlDataReader reader = scomSelect.ExecuteReader();
                while (reader.Read())
                {
                    top20.Add(reader.GetString(0).Trim());
                }
                reader.Close();
                tb1.Text = "1." + top20[0];
                tb2.Text = "2." + top20[1];
                tb3.Text = "3." + top20[2];
                tb4.Text = "4." + top20[3];
                tb5.Text = "5." + top20[4];

                tb6.Text = "6." + top20[5];
                tb7.Text = "7." + top20[6];
                tb8.Text = "8." + top20[7];
                tb9.Text = "9." + top20[8];
                tb10.Text = "10." + top20[9];

                tb11.Text = "11." + top20[10];
                tb12.Text = "12." + top20[11];
                tb13.Text = "13." + top20[12];
                tb14.Text = "14." + top20[13];
                tb15.Text = "15." + top20[14];

                tb16.Text = "16." + top20[15];
                tb17.Text = "17." + top20[16];
                tb18.Text = "18." + top20[17];
                tb19.Text = "19." + top20[18];
                tb20.Text = "20." + top20[19];

            }
        }


        public void submitTagCloud()
        {
            List<string> top20 = new List<string>();

            String strSelect = "select top 20 technology_word from dbo.secmem_technology_db_ssm_project where technology_document =" + data.Row[0] + " and technology_flag !=" + 0 + " order by technology_TFIDF desc ";
            SqlCommand scomSelect = new SqlCommand(strSelect, scon);
            scomSelect.Connection = scon;
            //returnValue = (int)scomSelect.ExecuteScalar();
            SqlDataReader reader = scomSelect.ExecuteReader();
            while (reader.Read())
            {
                top20.Add(reader.GetString(0).Trim());
            }
            reader.Close();
            tb1.Text = "1." + top20[0];
            tb2.Text = "2." + top20[1];
            tb3.Text = "3." + top20[2];
            tb4.Text = "4." + top20[3];
            tb5.Text = "5." + top20[4];

            tb6.Text = "6." + top20[5];
            tb7.Text = "7." + top20[6];
            tb8.Text = "8." + top20[7];
            tb9.Text = "9." + top20[8];
            tb10.Text = "10." + top20[9];

            tb11.Text = "11." + top20[10];
            tb12.Text = "12." + top20[11];
            tb13.Text = "13." + top20[12];
            tb14.Text = "14." + top20[13];
            tb15.Text = "15." + top20[14];

            tb16.Text = "16." + top20[15];
            tb17.Text = "17." + top20[16];
            tb18.Text = "18." + top20[17];
            tb19.Text = "19." + top20[18];
            tb20.Text = "20." + top20[19];

        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("CTOC_FILE_DOWN_LOAD\a" + path);//현재 다운로드 할 파일의 이름과 다운로드 요청코드를 보냄
        }

        private void btnAssignmentAgree_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("CTOC_TEXT_EXT_PROC\a" + path + "\a" + name.Content);
            /*education_board ebo = new education_board();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(ebo);*/
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            education_board ebo = new education_board();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(ebo);
        }

        public void SET_TECH_TERM(Label lbTag, string msg)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 SET_TECH_TERM 메서드를 다시 호출
                    SET_TECHTERM d = new SET_TECHTERM(SET_TECH_TERM);
                    Dispatcher.Invoke(d, new object[] { lbTag, msg });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    lbTag.Content = msg;
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }


        public void SET_UPDATE()
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 SET_TECH_TERM 메서드를 다시 호출
                    UPDATA_TAGCLOUD d = new UPDATA_TAGCLOUD(SET_UPDATE);
                    Dispatcher.Invoke(d, new object[] { });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    TagCloud();
                    
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }

        public void SET_PROG(string msg)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 SET_TECH_TERM 메서드를 다시 호출
                    CONTROL_PROGRESSBAR d = new CONTROL_PROGRESSBAR(SET_PROG);
                    Dispatcher.Invoke(d, new object[] {msg });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    float prog_value = 100 / 8;
                    if (msg.Equals("clear"))
                    {
                        submitProg.Value = 100;
                        btnAssignmentAgree.IsEnabled = false;//승인버튼 비활성화
                        relationPageFade.IsEnabled = true;//연관과제 버튼 활성화

                        ft.Send("CLIENT_TECHNOLOGY\a" + data.Row[0]);//기술 태그 요청

                        cloud.Run();//태그 구름 동작 메소드
                        submitTagCloud();//데이터 그리드에서 검사를 안하는 태그클라우드 실행 메소드
                        del1.Visibility = Visibility.Visible;
                        del2.Visibility = Visibility.Visible;
                        del3.Visibility = Visibility.Visible;
                        del4.Visibility = Visibility.Visible;
                        del5.Visibility = Visibility.Visible;//태그 삭제 버튼 5개의 활성화

                    }
                    submitProg.Value += prog_value;
                    Console.WriteLine(submitProg.Value);
                    lbProgState.Content = msg;
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }

        private void del1_Click(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show(data.Row[0].ToString() +"," + lbTag1.Content.ToString()) ;
          
            ft.Send("DEL_REQ\a" + data.Row[0] + "\a" + lbTag1.Content);

            //TagCloud();
        }

        private void del2_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("DEL_REQ\a" + data.Row[0] + "\a" + lbTag2.Content);
            //TagCloud();
        }

        private void del3_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("DEL_REQ\a" + data.Row[0] + "\a" + lbTag3.Content);
            //TagCloud();
        }

        private void del4_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("DEL_REQ\a" + data.Row[0] + "\a" + lbTag4.Content);
            //TagCloud();
        }

        private void del5_Click(object sender, RoutedEventArgs e)
        {
            ft.Send("DEL_REQ\a" + data.Row[0] + "\a" + lbTag5.Content);
            //TagCloud();
        }



    }
}