
using System;
using System.Windows.Forms;
using System.Threading;       // 네임스페이스 추가
using System.IO;              // 네임스페이스 추가
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Windows.Threading;


namespace SW_Membership_Assignment2_UI
{
    public class FileTransferServer
    {
        string BASICPATH = "C:\\SSM_Assignment2\\"; //서버 기본 저장 경로


        private Socket server = null;         // 서버쪽 소켓 
        private Socket client = null;          // 서버 접속용 소켓 
        private Thread th = null;             // 상대방 정보 수신 Receive 메서드 스레드
        private string client_ip = null;        // 접속한 아이피 주소
        public FileInfo file_info = null;       // 파일 정보
        private const int BUFFER = 4096;   // 네트워크에서 한번에 전송할 바이트 배열의 크기


        /// 파일 서버 프로그램 시작
        public void ServerStart()
        {
            
            try
            {   // 7500 번 포트를 이용해 파일 서버 실행 			
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 7500);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(ipep);  // 소켓 바인딩
                server.Listen(10);  // 클라이언트 접속 대기

                while (true)
                {
                    this.client = server.Accept();   // 클라이언트가 접속하면 

                    IPEndPoint ip = (IPEndPoint)this.client.RemoteEndPoint;   // 상대방 아이피 알아내기

                    this.client_ip = ip.Address.ToString();  // 접속한 상대방 아이피 주소 저장

                    th = new Thread(new ThreadStart(Receive)); // Receive 메서드 스레드 생성
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// 서버 프로그램 중지
        public void ServerStop()
        {
            try
            {
                if (client != null)
                {
                    if (client.Connected)  // 파일 전송 클라이언트와 연결되어 있다면
                    {
                        client.Close();   // 클라이언트 접속 끊음							if(th.IsAlive)
                        th.Abort();  // 스레드 종료
                    }
                } server.Close();   // 파일 서버 소켓 닫기
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// 파일 서버와 연결 시도
        ///ip 연결할 서버 아이피 주소
        public bool Connect(string ip)
        {
            try
            {   // 접속할 파일 서버 아이피 주소와 포트번호 설정
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), 7500);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(ipep);  // 파일 서버에 접속   
                Console.WriteLine(ip + ": 서버에 접속 성공");
                this.client_ip = ip;   // 파일 서버 아이피 주소 기록

                th = new Thread(new ThreadStart(Receive));  // 파일 서버가 보내는 데이터 수신
                th.Start();

                return true;   // 파일 서버 접속에 성공하면 true 값 반환
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;   // 파일 서버 접속에 실패하면 false 값 반환
            }
        }

        /// 파일 서버 연결 종료...
        public int Disconnect()
        {
            try
            {
                if (client != null)  // 파일 서버에 접속되어 있다면
                {
                    if (client.Connected) client.Close();  // 접속 끊기		
                    if (th.IsAlive) th.Abort();     // 스레드 종료
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            return 1;
        }

        /// 접속된 상대방으로부터 데이터 수신
        public void Receive()
        {
            try
            {         // 상대방과 연결되어 있다면 
                while (client != null && client.Connected)
                {        // 상대방이 보낸 데이터 읽어오기


                    
                    byte[] data = this.ReceiveData();

                    byte[] stx = new byte[1];
                    stx[0] = 0x02;

                    if (data[0] == stx[0])
                    {
                        byte[] result = new byte[data.Length - 1];

                        for (int i = 0; i < data.Length-1; i++)
                        {
                            result[i] = data[i + 1];
                        }
                        
                        DataSet dt = new DataSet();
                        dt = DecompressDataSet(result);
                        DataTable dtable = dt.Tables[0];
                        Main.education.Input_Grid(null);
                        Main.education.Input_Grid(dtable.DefaultView);

                        Main.education.Add_MSG(".");
                        Thread.Sleep(80);
                        Main.education.Add_MSG("..");
                        Thread.Sleep(80);
                        Main.education.Add_MSG("...");
                        Thread.Sleep(80);
                        Main.education.Add_MSG("....");
                        Thread.Sleep(100);
                        Main.education.Add_MSG("OK :D");
                        continue;
                    }
                    string msg = Encoding.Default.GetString(data);
                    string[] token = msg.Split('\a');
                    Console.WriteLine("토큰사이즈 : "+token.Length);

                    switch (token[0])
                    {
                        case "CTOC_FILE_TRANS_INFO":  // 전송할 파일 정보
                            FileInfo(token[1], Convert.ToInt64(token[2].Trim()));
                            break;

                        case "CTOC_FILE_TRANS_YES":	 // 파일 전송 수락
                            long current_size = Convert.ToInt64(token[1].Trim());
                            this.SendFileData(this.file_info, current_size);
                            break;

                        case "CTOC_FILE_TRANS_NO":  // 파일 전송 거부
                            Console.WriteLine("상대방이 파일 전송을 거부했습니다.");
                            break;
                        case "CTOC_FILE_DOWN_LOAD": //파일전송을 요청

                            FileInfo send_file = new FileInfo(token[1]);//token[1] : BASICPATH + filename;
                            this.Send("CTOC_FILE_TRANS_INFO" + "\a" + send_file.Name + "\a" + send_file.Length.ToString());
                            file_info = send_file;

                            break;
                        case "CLIENT_RELATION":
                            
                             Console.WriteLine("여기 들어왔숑");
                             Console.WriteLine(token[1]);
                             Console.WriteLine(token[2]);
                             Console.WriteLine(token[3]);
                             Console.WriteLine(token[4]);
                             
                             Main.relationPage.Change_BtnContent(Main.relationPage.tb1Text, token[1]);
                             Main.relationPage.Change_BtnContent(Main.relationPage.tb2Text, token[2]);
                             Main.relationPage.Change_BtnContent(Main.relationPage.tb3Text, token[3]);
                             Main.relationPage.Change_BtnContent(Main.relationPage.tb4Text, token[4]);
                             Main.relationPage.STOP_LOAD(Main.relationPage.loadingBar);
                            break;

                        case "CLIENT_WAITING":
                            Main.assignPage.SET_PROG("clear");
                            break;
                        case "SERVER_QUESTION_MSG":
                            Main.education.Input_Grid(null);
                            Main.education.Add_MSG(".");
                            Thread.Sleep(80);
                            Main.education.Add_MSG("..");
                            Thread.Sleep(80);
                            Main.education.Add_MSG("...");
                            Thread.Sleep(80);
                            Main.education.Add_MSG("....");
                            Thread.Sleep(100);
                            Main.education.Add_MSG("검색결과 없음");
                            break;
                        case "SERVER_TECHNOLOGY":
                            Console.WriteLine(token[1]);
                             Console.WriteLine(token[2]);
                             Console.WriteLine(token[3]);
                             Console.WriteLine(token[4]);
                            Main.assignPage.SET_TECH_TERM(Main.assignPage.lbTag1, token[1]);
                            Main.assignPage.SET_TECH_TERM(Main.assignPage.lbTag2, token[2]);
                            Main.assignPage.SET_TECH_TERM(Main.assignPage.lbTag3, token[3]);
                            Main.assignPage.SET_TECH_TERM(Main.assignPage.lbTag4, token[4]);
                            Main.assignPage.SET_TECH_TERM(Main.assignPage.lbTag5, token[5]);
                            Main.assignPage.SET_UPDATE();//태그클라우드 업데이트
                            break;
                        case "CLIENT_WAIT"://단어삭제 관련 프로토콜
                            Send("CLIENT_TECHNOLOGY\a"+token[1]);
                            Console.WriteLine("받았음");
                            break;
                        case "Running1":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);

                            break;
                        case "Running2":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);

                            break;
                        case "Running3":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);

                            break;
                        case "Running4":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);

                            break;
                        case "Running5":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);
                            break;
                        case "Running6":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);
                            break;
                        case "Running7":
                            Main.assignPage.SET_PROG("현재상태 : " + token[1]);
                            break;


                            
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public DataSet DecompressDataSet(byte[] bytDs)
        {
            DataSet outDs = new DataSet();
            MemoryStream inMs = new MemoryStream(bytDs);
            inMs.Seek(0, 0); //스트림으로 가져오기

            //1. 압축객체 생성- 압축 풀기
            DeflateStream zipStream = new DeflateStream(inMs,
            CompressionMode.Decompress, true);
            byte[] outByt = ReadFullStream(zipStream);
            zipStream.Flush();
            zipStream.Close();
            MemoryStream outMs = new MemoryStream(outByt);
            outMs.Seek(0, 0); //2. 스트림으로 다시변환
            outDs.RemotingFormat = SerializationFormat.Binary;

            //3. 데이터셋으로 Deserialize
            BinaryFormatter bf = new BinaryFormatter();
            outDs = (DataSet)bf.Deserialize(outMs, null);
            return outDs;
        }

        public byte[] ReadFullStream(Stream stream)
        {
            //스트림을 Byte 배열로 변환
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        /// 상대방이 보낸 데이터를 바이너리 형태로 읽어오기
        private byte[] ReceiveData()
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기   
                byte[] data_size = new byte[8];
                recv_data = this.client.Receive(data_size, 0, 8, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;
                byte[] data = new byte[size];
                // 서버에서 전송한 실제 데이터 수신
                while (total < size)
                {
                    recv_data = this.client.Receive(data, total, left_data, SocketFlags.None);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// 문자열 메시지 전송
        /// msg 전송할 문자열
        public void Send(string msg)
        {
            byte[] data = Encoding.Default.GetBytes(msg);
            this.SendData(data);
        }

        /// 상대방에게 바이너리 형태로 데이터 보내기
        private void SendData(byte[] data)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                // 전송할 실제 데이터의 크기 전달
                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = this.client.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = this.client.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// 파일 데이터 전송
        ///file 상대방에게 보낼 파일 정보
        /// currentl_size 상대방에게 보낼 파일 포인터 위치
        private void SendFileData(FileInfo file, long current_size)
        {
            long total_size = file.Length;  // 파일 크기
            long size = file.Length - current_size;  // 보낼 파일 위치 지정
            long count = size / BUFFER;             // 전송할 횟수
            long remain_byte = size % BUFFER;

            long index = 0;
            long prg_value = 0;
            long time = 0;

            FileStream fs = null;
            BinaryReader br = null;

            try
            {  // 전송할 실제 파일 데이터 크기 전달
                fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                if (current_size > 0)   // 파일이 상대방에게 있을 경우 파일 포인터 이동
                {
                    fs.Seek(current_size, SeekOrigin.Begin);
                    prg_value += current_size;
                }

                br = new BinaryReader(fs);   // 파일 읽어오기

                Byte[] data = new byte[BUFFER]; // 4096 단위로 읽어서 전송

                while (index < count)
                {
                    if (DateTime.Now.Ticks - time > 10E7)// 1초마다 프로그래스바 갱신
                    {
                        time = DateTime.Now.Ticks;
                    }
                    br.Read(data, 0, BUFFER);
                    client.Send(data, 0, BUFFER, SocketFlags.None);
                    index++;
                }

                if (remain_byte > 0)   // 남아있는 데이터가 있다면
                {
                    br.Read(data, 0, (int)remain_byte);
                    client.Send(data, 0, (int)remain_byte, SocketFlags.None);
                }

                Console.WriteLine("파일전송완료");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Close();
            }
        }

        /// 파일 데이터 수신
        //"fs" 수신할 파일 스트림
        //"total_size" 파일 전체 크기
        //"remain_size" 수신할 파일 크기
        private void ReceiveFileData(FileStream fs, long total_size, long remain_size)
        {
            long total = 0;
            long left_size = remain_size;
            int recv_size = 0;

            long prg_value = 0;
            long time = 0;

            BinaryWriter bw = null;

            Byte[] data = new byte[BUFFER];  // 4096 단위로 데이터 수신


            try
            {
                bw = new BinaryWriter(fs);

                if (total_size > remain_size)   // 이어받기 기능일 경우 해당 위치로 파일 포인터이동
                {
                    bw.Seek((int)(total_size - remain_size), SeekOrigin.Begin);
                    prg_value += total_size - remain_size;
                }

                while (total < remain_size)
                {
                    if (DateTime.Now.Ticks - time > 10E7) // 1초마다 프로그래스바 갱신
                    {
                        time = DateTime.Now.Ticks;
                    }

                    if (left_size > BUFFER)
                        recv_size = this.client.Receive(data, BUFFER, SocketFlags.None);
                    else
                        recv_size = this.client.Receive(data, (int)left_size, SocketFlags.None);

                    if (recv_size == 0) break;
                    bw.Write(data, 0, recv_size);
                    total += recv_size;
                    left_size -= recv_size;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (bw != null) bw.Close();
            }
        }

        // 상대방이 전송하려는 파일 이름과 크기 출력, 파일 수신기능 포함
        public void FileInfo(string filename, long filesize)
        {	// 파일이름/파일크기
            string message = this.client_ip + " 님이 보내는 파일명 : " + filename + " (" + filesize + " byte)을 다운 받으시겠습니까?";
            if (MessageBox.Show(message, "파일 전송 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {    // 파일 수신 확인				
                FileStream fs = new FileStream(@"C:\Download\" + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                this.Send("CTOC_FILE_TRANS_YES\a" + fs.Length.ToString());
                this.ReceiveFileData(fs, filesize, filesize - fs.Length); // 파일 수신 시작
                fs.Close();  // 파일 닫기

            }
            else
            {   // 파일 수신 거부

                this.Send("CTOC_FILE_TRANS_NO\a");
            }
        }
    }
}
