using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPClient
{
    class UdpClientConsole
    {
        private static int port;
        private static UdpClient client;
        private static IPEndPoint endPoint;
        private static IAsyncResult asyncResult;

        static void Main(string[] args)
        {
            Console.Write("사용할 포트 번호를 입력하세요: ");
            if (!int.TryParse(Console.ReadLine(), out port) || port <= 0 || port > 65535)
            {
                Console.WriteLine("유효하지 않은 포트 번호입니다. 프로그램을 종료합니다.");
                return;
            }

            Console.WriteLine($"UDP 클라이언트가 포트 {port}에서 시작됩니다...");
            Thread socketThread = new Thread(new ThreadStart(Start));
            socketThread.IsBackground = true;
            socketThread.Start();

            Console.WriteLine("메시지 수신 대기 중입니다. 종료하려면 Ctrl+C를 누르세요.");
            while (true)
            {
                Thread.Sleep(1000); // 메인 스레드는 대기 상태 유지
            }
        }

        private static void Start()
        {
            if (client != null)
            {
                Debug.WriteLine("이미 UDP 소켓이 생성되어 있습니다.");
                return;
            }

            client = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Any, port);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(endPoint);

            StartListening();
        }

        private static void StartListening()
        {
            asyncResult = client.BeginReceive(Receive, null);
        }

        private static void Receive(IAsyncResult ar)
        {
            if (client == null) { return; }

            try
            {
                byte[] bytes = client.EndReceive(ar, ref endPoint);
                //20자까지만 나타냄
                string message = Encoding.UTF8.GetString(bytes, 0, 20);

                WriteLog(message);
                StartListening();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"수신 중 오류 발생: {ex.Message}");
            }
        }

        private static void WriteLog(string message)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"[{date}] {message}");
        }
    }
}
