using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialReader : MonoBehaviour
{
    SerialPort port;
    Thread readThread;
    bool running = false;
    public static SerialReader sr = null;

    string latestLine = "";
    string portNum = "COM3";

    void Awake()
    {
        if (sr == null){
            sr = this;
        }
    }

    void Start()
    {

    }

    public void ConnectPort()
    {
        port = new SerialPort(portNum, 9600);
        port.ReadTimeout = 100;

        port.Open();
        // Debug.Log("Connected");

        running = true;
        readThread = new Thread(ReadSerial);
        readThread.Start();
    }

    public void DisconnectPort()
    {
        running = false;

        if (readThread != null && readThread.IsAlive)
            readThread.Join();

        if (port != null && port.IsOpen)
            port.Close();
    }

    void ReadSerial()
    {
        while (running)
        {
            try
            {
                string line = port.ReadExisting();
                if (!string.IsNullOrEmpty(line))
                latestLine = latestLine + line;
                if (latestLine.Length > 15){
                    latestLine = latestLine.Substring(latestLine.Length-15);
                }
            }
            catch { }
        }
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        DisconnectPort();
    }

    public void setPort(string portText)
    {
        portNum = portText;
    }

    public string getPort()
    {
        return portNum;
    }

    public bool getConnected()
    {
        return running;
    }

    public string getLatestLine()
    {
        int i = latestLine.IndexOf(';');
        if (i == -1) {
            return "";
        }
        if (i == 6){
            return latestLine.Substring(0, 6);
        }
        else if ( latestLine.Length < i + 6){
            return "";
        }
        return latestLine.Substring(i+1, 6);
    }
}
