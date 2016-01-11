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
using System.Net.Sockets;       // SOCKET 연결
using System.Net;

using System.Threading;
using System.IO; //파일 전송

using WordTextExt.Office;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// education_board_write.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class education_board_write : UserControl
    {
        FileTransferServer ft = null;
        string filpath; //파일경로
        // db 연결 하기
        SqlConnection scon;

        TextExtHelper txtExtHelper = null;//텍스트 추출 핼퍼클래스 객체 선언
        

        public education_board_write()
        {
            InitializeComponent();
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용
            ft = Main.ft;

            txtExtHelper = new TextExtHelper();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void fileBtn_Click(object sender, RoutedEventArgs e)
        {
            // File 오픈 다이얼로그
            // 경로 추출하기

            Microsoft.Win32.OpenFileDialog dig = new Microsoft.Win32.OpenFileDialog();
            //dig.DefaultExt = ".doc";
            Nullable<bool> result = dig.ShowDialog();

            // 파일 이름 선택하고 텍스트박스로 보여주기
            if (result == true)
            {
                // open document
                filpath = dig.FileName;
                lbFilePath.Content = filpath.ToString();
            }

            
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            
            if (filpath != "")
            {
                FileInfo send_file = new FileInfo(filpath);

                txtExtHelper.OpenFile(filpath);
                IOfficeFile ioffice = txtExtHelper.get_file();

                txtExtHelper.ShowContentsFunc(ioffice);

                IWordFile wordfile = txtExtHelper.getwordFile();
                string curDoc = wordfile.ParagraphText; //텍스트 추출~ 
                
                curDoc = curDoc.Replace("\r\n","");

                int firstIndex = curDoc.IndexOf("및 동기") + "및 동기".Length; //첫번째 인덱스 구하기
                int lastIndex = curDoc.LastIndexOf("개발 환경  및");
                if (lastIndex == -1)
                {
                    lastIndex = curDoc.LastIndexOf("개발 환경및");
                }

                if (lastIndex == -1)
                {
                    lastIndex = curDoc.LastIndexOf("개발 환경 및");
                }

                string descString = curDoc.Substring(firstIndex, lastIndex - firstIndex);
                descString = descString.Replace("'", "");
                Console.WriteLine(descString.Length);

                txtExtHelper.CloseFile();

                //파일 정보 메시지 작성하기, 파일이름, 파일크기
                string msg = "CTOC_FILE_TRANS_INFO\a" + send_file.Name + "\a" + send_file.Length.ToString();
                //파일전송에 필요한 정보, 파일이름, 파일크기


                Console.WriteLine("디비열엄");
                string name = titleTxt.Text.ToString();

                string type = comboBox1.Text;
                string des = descrip.Text.ToString();

                string sDate = start.Text.ToString();
                string eDate = end.Text.ToString();

                string field = comboBox2.Text;
                string plat = comboBox3.Text;
                string is_used = "대기중";
                int status = 1;
                int startIdx = 0;
                int endIdx = 1;
                //string path = lbFilePath.Content.ToString();
                string path = send_file.Name;
                int idx = 1;

                // file 이름

                string sendData = "CTOC_SEND_SQL_AND_File_Trans\a" + name + "\a" + type + "\a" + des + "\a" +
                    sDate + "\a" + eDate + "\a" + status + "\a" + startIdx + "\a" + endIdx + "\a" +
                        plat + "\a" + is_used + "\a" + path + "\a" + descString + "\a"+ send_file.Name + "\a" + send_file.Length.ToString() ;
                
                this.ft.Send(sendData);
                this.ft.file_info = send_file;
                Thread.Sleep(300);

                education_board ebo = new education_board();
                Main.pageNavigation.PushBack(this);
                Main.pageNavigation.preClear();
                Main.pageFade.ShowPage(ebo);
            }
            else
            {
                MessageBox.Show("파일을 선택해주세요");
            }
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
          
            MessageBox.Show(lbFilePath.Content.ToString());
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            education_board ebo = new education_board();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(ebo);
        }
    }
}

