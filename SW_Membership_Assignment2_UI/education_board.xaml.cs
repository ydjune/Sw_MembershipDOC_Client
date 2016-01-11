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


using System.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Threading;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// education_board.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class education_board : UserControl
    {
        bool serchtFlag = false;
       
        // db 연결 하기
        SqlConnection scon;
        
        public static FileTransferServer ft = null;

        delegate void SetEXT_Text_Callback(string msg);  // Add_MSG 메서드 델리게이트
        delegate void SET_DATAGRID(DataView msg);  // 그리드뷰 메서드 델리게이트

        private DataGridRow previousRow;
//###################################### 페이징 커스텀콘트롤 변수들 ##########################################################
        private int paging_PageIndex = 0; //페이지 초기 인덱스
        private int paging_NoOfRecPerPage = 10; //페이지 최대 개수
        private int pagin_Max_RowCount = 0; //최대 페이지 수
        private enum PagingMode { Next = 2, Previous = 3, First = 1, Last = 4 }; //페이징 변수들
        DataTable dataTable; //현재 우리 데이터가 담기는 Datatable
//#############################################################################################################################

        education_board ebo = null;

        public education_board()
        {
            InitializeComponent();
            PageNavigation.CurrentPage = this; // 페이지 네비게이트 현재페이지 저장용

            ft = Main.ft;
            Main.getEducationBoard(this);
            ebo = this;

            scon = App.scon;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
            // 크기 자동 조절                                                                                                                              

            DataTable dataTable = getDBtable();
            pagin_Max_RowCount = dataTable.Rows.Count;
            //최대페이지 계산
            if(pagin_Max_RowCount%paging_NoOfRecPerPage == 0)
                lbMaxRowCount.Content = pagin_Max_RowCount/paging_NoOfRecPerPage ;
            else
                lbMaxRowCount.Content = pagin_Max_RowCount / paging_NoOfRecPerPage +1;
            firstSetTable();

            gridAssignment1.Columns[6].Visibility = Visibility.Collapsed;

        }

        public DataTable getDBtable()
        {
            
            String strSql = "select  * from dbo.secmem_board_extra_ssm_project order by wr_id desc";
            SqlCommand scom = new SqlCommand(strSql, scon);
            //scon.Open();
            scom.Connection = scon;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
            // database adapter
            SqlDataAdapter adapter = new SqlDataAdapter(strSql, scon);                                                                                                                                                


            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);


            dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }


        private void firstSetTable()
        {
            if (dataTable.Rows.Count > (paging_PageIndex * paging_NoOfRecPerPage))
            {
                DataTable tmpTable = new DataTable();
                tmpTable = dataTable.Clone();

                if (dataTable.Rows.Count >= ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage))
                {
                    for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage); i++)
                    {
                       

                        tmpTable.ImportRow(dataTable.Rows[i]);
                       
                    }
                }
                else
                {
                    for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < dataTable.Rows.Count; i++)
                    {
                        
                        tmpTable.ImportRow(dataTable.Rows[i]);
                        
                    }
                }

                paging_PageIndex += 1;
                gridAssignment1.ItemsSource = tmpTable.DefaultView;
                tmpTable.Dispose();
            }
        }

        private void CustomPaging(int mode)
        {
            int totalRecords = dataTable.Rows.Count;
            int pageSize = paging_NoOfRecPerPage;
            int currentPageIndex = paging_PageIndex;

            if (dataTable.Rows.Count <= paging_NoOfRecPerPage)
            {
                return;
            }

            switch (mode)
            {
                case (int)PagingMode.Next:
                    if (dataTable.Rows.Count > (paging_PageIndex * paging_NoOfRecPerPage))
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = dataTable.Clone();

                        if (dataTable.Rows.Count >= ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage))
                        {
                            for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage); i++)
                            {
                                
                                tmpTable.ImportRow(dataTable.Rows[i]);
                            }
                        }
                        else
                        {
                            for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < dataTable.Rows.Count; i++)
                            {
                                
                                tmpTable.ImportRow(dataTable.Rows[i]);
                            }
                        }

                        paging_PageIndex += 1;

                        gridAssignment1.ItemsSource = tmpTable.DefaultView;
                        tmpTable.Dispose();
                    }
                    break;
                case (int)PagingMode.Previous:
                    if (paging_PageIndex > 1)
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = dataTable.Clone();

                        paging_PageIndex -= 1;

                        for (int i = ((paging_PageIndex * paging_NoOfRecPerPage) - paging_NoOfRecPerPage); i < (paging_PageIndex * paging_NoOfRecPerPage); i++)
                        {
                        
                            tmpTable.ImportRow(dataTable.Rows[i]);
                        }

                        gridAssignment1.ItemsSource = tmpTable.DefaultView;
                        tmpTable.Dispose();
                    }
                    break;
                case (int)PagingMode.First:
                    paging_PageIndex = 2;
                    CustomPaging((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    paging_PageIndex = (dataTable.Rows.Count / paging_NoOfRecPerPage);
                    CustomPaging((int)PagingMode.Next);
                    break;
            }

            DisplayPagingInfo();
        }
        private void DisplayPagingInfo()
        {
            string pagingInfo = "Displaying " + (((paging_PageIndex - 1) * paging_NoOfRecPerPage) + 1) + " to " + paging_PageIndex * paging_NoOfRecPerPage;

            if (dataTable.Rows.Count < (paging_PageIndex * paging_NoOfRecPerPage))
            {
                pagingInfo = "Displaying " + (((paging_PageIndex - 1) * paging_NoOfRecPerPage) + 1) + " to " + dataTable.Rows.Count;
            }
            //lblPagingInfo.Content = pagingInfo;

            lblPageNumber.Content = paging_PageIndex;
        }


        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // 과제 등록 게시판으로 가기
            education_board_write edu_bo_wr = new education_board_write();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(edu_bo_wr);
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("더블클릭됨");
            
            // 한개의 ROW 값만 가져오기
            DataRowView d = gridAssignment1.CurrentCell.Item as System.Data.DataRowView;
 

            // 과제 등록 게시판으로 가기
            AssignmentPage assignPage = new AssignmentPage(d); 
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(assignPage);

            if(d.Row[10].Equals("진행중"))//승인후에만 태그가 뜨도록
             Main.ft.Send("CLIENT_TECHNOLOGY\a"+ d.Row[0]);

            // NavigationService.Navigate(new AssignmentPage(d));  이거 처리해줘야ㅇ되
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (tbQueryMsg.Text != "")
            {
                gridAssignment1.Columns[6].Visibility = Visibility.Visible;
                // 메세지값 얻어와서 select 프로토콜정의해서, 프로토콜+텍스트메시지값 
                //CTOC_QUESTION_MSG
                string protocol = "CTOC_QUESTION_MSG";
                string searchSQL = protocol + "\a" + tbQueryMsg.Text;
                ft.Send(searchSQL);
            }
            else
            {
                lbSerchResult.Content = "검색어를 입력해주세요!";
            }
            //gridAssignment1.Columns[6].Visibility = Visibility.Visible;
        }

        public void excuteupdate(DataTable dt)
        {
            gridAssignment1.ItemsSource = dt.DefaultView;
        }

        public void Add_MSG(string msg)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 Add_MSG 메서드를 다시 호출
                    SetEXT_Text_Callback d = new SetEXT_Text_Callback(Add_MSG);
                     Dispatcher.Invoke(d, new object[] { msg });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    this.lbSerchResult.Content = msg;  // 채팅 문자열 출력
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }

        public void Input_Grid(DataView msg)
        {
            try
            {
                if (!Dispatcher.CheckAccess()) // 컨트롤 요청이 들어올 경우
                {   // 델리게이트를 이용해 Input_Grid 메서드를 다시 호출
                    SET_DATAGRID d = new SET_DATAGRID(Input_Grid);
                    Dispatcher.Invoke(d, new object[] { msg });
                }
                else
                {   // 컨트롤 접근이 가능할 경우, 다음 구문 처리
                    this.gridAssignment1.ItemsSource = msg;  // 채팅 문자열 출력
                }
            }
            catch { } // 멀티 스레드 환경에서 뜻하지 않은 예외가 발생할 경우 처리
        }


        private void gridAssignment1_MouseMove(object sender, MouseEventArgs e) //마우스 움직일 때 Row색상 변경 이벤트
        {
            //마우스 무브이벤트
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            // iteratively traverse the visual tree
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null)
                return;

            if (dep is DataGridRow)
            {  
                DataGridRow row = dep as DataGridRow;
                if (row.IsMouseOver && previousRow != row)
                {
                    row.Background = new SolidColorBrush(Colors.Gray);
                    
                    if (previousRow != null)
                    {
                        previousRow.Background = new SolidColorBrush(Colors.White);
                    }
                }
                previousRow = row;

               
            }
             
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e) //요약팝업 띄어주는 이벤트
        {
            //데이터 버튼 그리드 이벤트

            DataRowView d2 = gridAssignment1.CurrentItem as System.Data.DataRowView;
            popInformation.IsOpen = true;
            tbMessage.Text = d2.Row[12].ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //팝업윈도우에서 닫기버튼 이벤트
        {
            popInformation.IsOpen = false;
        }

        private void Button1_Click(object sender, RoutedEventArgs e) //팝업윈도우에서 닫기버튼 이벤트
        {
            popInformation1.IsOpen = false;
        }

//############################페이지 커스텀 콘트롤 버튼이벤트 #####################################
        private void btnBackFirst_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.First);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Previous);
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Next);
        }

        private void btnPreLast_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Last);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            gridAssignment1.Columns[6].Visibility = Visibility.Collapsed;
            education_board ebo = new education_board();
            Main.pageNavigation.PushBack(this);
            Main.pageNavigation.preClear();
            Main.pageFade.ShowPage(ebo);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DataRowView d3 = gridAssignment1.CurrentItem as System.Data.DataRowView;

            List<string> top20 = new List<string>();

            String strSelect = "select count(*) from dbo.secmem_technology_db_ssm_project where technology_document =" + d3.Row[0];
            SqlCommand scomSelect = new SqlCommand(strSelect, scon);
            scomSelect.Connection = scon;
            //returnValue = (int)scomSelect.ExecuteScalar();
            SqlDataReader reader = scomSelect.ExecuteReader();
            int termNumber=0;
            while (reader.Read())
            {
                termNumber = reader.GetInt32(0) ;
            }
            reader.Close();
            double percent = (double)d3.Row[14] / (int)d3.Row[15];
            double hap = (double)d3.Row[14];
            popInformation1.IsOpen = true;
            tbMessage1.Text = termNumber.ToString() + "개 중 " + d3.Row[15] + "개 일치\n" + "일치 단어들의 TF-IDF 평균:" + percent.ToString("##.###") + "\n" + "검색어 TF-IDF 합 : " + hap.ToString("##.###") ;
            //tbMessage.Text = termNumber.ToString() + "개, " + "13번:" + d3.Row[13] + ", 14번:" + d3.Row[14] + ", 15번:" + d3.Row[15];//13 : 총합 , 14 : 검색어의 합 , 15 검색어 일치 개수
        }

//############################페이지 커스텀 콘트롤 버튼이벤트 #####################################
       

    }
}
