using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgramming.UDP
{
    class UdpServerConsole
    {
        // 수정한 코드
        private static string ip; // 전역 변수로 변경
        private static int port;  // 전역 변수로 변경
        private static string message; // 전역 변수로 변경

        static void Main(string[] args)
        {
            Console.Write("메시지를 전송할 IP 주소를 입력하세요: ");
            ip = Console.ReadLine();
            if (!IPAddress.TryParse(ip, out _))
            {
                Console.WriteLine("유효하지 않은 IP 주소입니다. 프로그램을 종료합니다.");
                return;
            }

            Console.Write("포트 번호를 입력하세요: ");
            if (!int.TryParse(Console.ReadLine(), out port) || port <= 0 || port > 65535)
            {
                Console.WriteLine("유효하지 않은 포트 번호입니다. 프로그램을 종료합니다.");
                return;
            }
            // 수정한 코드
            message = new string('a', 1024 - 100);
            // 멀티 스레드로 실행
            Thread socketThread = new Thread(new ThreadStart(SendMessage));
            socketThread.IsBackground = true;
            socketThread.Start();

            while (true)
            {
                Console.Write("전송할 메시지를 입력하세요 (종료하려면 'exit' 입력): ");
                string message_input = Console.ReadLine();
                if (string.Equals(message_input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("프로그램을 종료합니다.");
                    break;
                }

                //SendMessage(ip, port, message);
            }
        }

        private static void SendMessage()
        {
            int sequenceNum = 0;
            UdpClient client = new UdpClient();
            try
            {
                // using (UdpClient client = new UdpClient()) 구문의 의미
                // UdpClient 객체를 생성하고, 사용이 끝난 후 해당 객체의 리소스를 자동으로 해제

                //using (UdpClient client = new UdpClient()){}
                    while (true) {
                        
                            sequenceNum++;
                            byte[] bytes = Encoding.UTF8.GetBytes(sequenceNum+message);
                            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            client.Send(bytes, bytes.Length, endPoint);

                            WriteLog(sequenceNum+message.Substring(0,10));
                            Thread.Sleep(50);
                        
                    }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"오류 발생: {ex.Message}");
                Console.WriteLine($"메시지 전송 중 오류 발생: {ex.Message}");
            }
        }

        private static void WriteLog(string message)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"[{date}] 전송된 메시지: {message}");
        }
    }
}
