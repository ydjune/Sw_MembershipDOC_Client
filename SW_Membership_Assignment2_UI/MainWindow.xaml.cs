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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Net;
using System.Net.Sockets;
using WpfPageTransitions;
using System.Threading;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Main : Window
    {

        public static FileTransferServer ft = null;// 파일전송 클래스 객체 변수
            

        public static Socket client = null;
        public static education_board education = null;
        public static RelationProject relationPage = null;
        public static AssignmentPage assignPage = null;

        public static PageNavigation pageNavigation;
      

        public static PageTransition pageFade = null;
        public Main()
        {
            InitializeComponent();
			LoadDatas();
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용
            ft = new FileTransferServer();
            pageNavigation = PageNavi;
            if (!ft.Connect("210.118.69.166"))
            {
                MessageBox.Show("서버 아이피 주소가 틀리거나 작동중이지 않습니다.");
            }
            login loginPage = new login();
            pageFade = pageTransitionControl;
            
            
            pageFade.ShowPage(loginPage);
			
			
			this.MouseLeftButtonDown+=new System.Windows.Input.MouseButtonEventHandler(Main_MouseLeftButtonDown);
    
        }
		
		void LoadDatas()
		{
			Thread.Sleep(3000);	
		}

        public static void getEducationBoard(education_board handle) //게시판 페이지에 접근하기 위한 핸들 받아오기
        {
            education = handle;
        }

        public static void setRelationProject(RelationProject handle) //연관과제 페이지에 접근하기 위한 핸들 받아오기
        {
            relationPage = handle;
        }

        public static void setAssignPage(AssignmentPage handle) //assignment 페이지에 접근하기 위한 핸들 받아오기
        {
            assignPage = handle;
        }

        public void pageNavigate(UserControl newPage)
        {
            pageTransitionControl.ShowPage(newPage);
        }

        private void Main_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //MessageBox.Show(e.GetPosition(this).ToString());
			this.DragMove();
            
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e) //클라이언트를 종료할때 서버 접속과 디비서버의 접속이 종료된다.
        {
            try
            {
                if (ft.Disconnect() != 0)
                {
                    ft.Disconnect();	// 파일 서버 접속 해제
                }

                ft.ServerStop();  // 파일 서버 작동 중지
                App.scon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }	

            Close();
        }
    }
}
