using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private Thread udpThread;
    private Thread tcpThread;
    private TcpClient tcpClient;

    // msb move x y lb rb 1~7
    // 0110 0000 0000 0000 0000 0000 0000 0000
    private const int moveMask = 0x60000000;
    private const int moveBit = 29;
    // 0001 1111 1111 1000 0000 0000 0000 0000
    private const int xMask = 0x1FF80000;
    private const int xBit = 19;
    // 0000 0000 0000 0111 1111 1110 0000 0000
    private const int yMask = 0x0007FE00;
    private const int yBit = 9;

    // 0000 0000 0000 0000 0000 0001 0000 0000
    private const int leftButtonMask = 0x00000100;
    private const int leftButtonBit = 8;

    // 0000 0000 0000 0000 0000 0000 1000 0000
    private const int rightbuttonMask = 0x00000080;
    private const int rightButtonBit = 7;

    // 0000 0000 0000 0000 0000 0000 0100 0000
    private const int button1Mask = 0x00000040;
    private const int button1Bit = 6;

    // 0000 0000 0000 0000 0000 0000 0010 0000
    private const int button2Mask = 0x00000020;
    private const int button2Bit = 5;

    void Start()
    {
        // UDP 브로드캐스트 수신
        udpThread = new Thread(() =>
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEP);
                string receivedString = Encoding.UTF8.GetString(receivedBytes);
                Debug.Log("Received UDP broadcast from " + remoteEP + ": " + receivedString);

                // TCP 연결 요청
                tcpClient = new TcpClient();
                tcpClient.Connect(remoteEP.Address, 23456); // 라즈베리파이의 IP 주소와 포트 번호

                // TCP로 데이터 수신
                tcpThread = new Thread(() =>
                {
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buffer = new byte[4];
                    while (true)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 4)
                        {
                            int data = BitConverter.ToInt32(buffer, 0);

                            int move = (moveMask & data) >> moveBit;
                            int x = (xMask & data) >> xBit;
                            int y = (yMask & data) >> yBit;
                            int leftButton = (leftButtonMask & data) >> leftButtonBit;
                            int rightButton = (rightbuttonMask & data) >> rightButtonBit;
                            int btn1 = (button1Mask & data) >> button1Bit;
                            int btn2 = (button2Mask & data) >> button2Bit;

                            Debug.Log($"move: {move}, x: {x}, y: {y}, leftButton: {leftButton}, rightButton: {rightButton}\nbutton1: {btn1}, button2: {btn2}");
                        }
                        else
                        {
                            break;
                        }
                    }
                });
                tcpThread.Start();
            }
        });
        udpThread.Start();
    }

    void OnDestroy()
    {
        // 스레드 종료
        if (udpThread != null)
        {
            udpThread.Abort();
            udpThread = null;
        }
        if (tcpThread != null)
        {
            tcpThread.Abort();
            tcpThread = null;
        }

        // TCP 연결 종료
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
    }
}
