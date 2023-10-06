using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO.Ports;
using System;

using System.Net.Sockets;


class BluetoothManager : MonoBehaviour
{
    public string deviceName = "RaspberryController";
    public int baudRate = 9600;
    private SerialPort serialPort;
    private byte[] _buffer = new byte[4];

    void Start()
    {
        serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.None);

        if (serialPort.IsOpen == false)
        {
            Debug.Log("Open!!!");
            serialPort.ReadTimeout = 1;
            serialPort.Open();
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            string data = serialPort.ReadLine();
            Debug.Log(data);


            //serialPort.Read(_buffer, 0, 4);

            //int data = BitConverter.ToInt32(_buffer, 0);
            //Debug.Log("Received data from Bluetooth device: " + data);
        }
    }

    public void SendData(string data)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(data);
            Debug.Log(data);
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
