using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Data.SqlClient;

namespace SW_Membership_Assignment2_UI
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class App : Application
    {
        
        
        public static SqlConnection scon;
       
        protected override void OnStartup(StartupEventArgs e)
        {
            
            string connectionString = "server = 210.118.69.165; uid = sa; pwd = tnwls; database = secmem_ver3; ";

            scon = new SqlConnection(connectionString);
            try
            {
                scon.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("APP입니다.");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("디비 접속합니다.!");
            
            Console.Write(scon.State);

        }


    }
}
