using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TMP_Dropdown dropdown;
    public GameObject startButton;
    public GameObject connectButton;
    public GameObject disconnectButton;
    public GameObject tragetPoint;
    public TMP_Text textField;
    public GameObject inputFieldObj;
    private TMP_InputField inputFieldPort;

    private bool connected = false;

    private int posX;
    private int posY;

    void Start()
    {
        Application.targetFrameRate = 60;
        inputFieldPort = inputFieldObj.GetComponent<TMP_InputField>();
        inputFieldPort.text = SerialReader.sr.getPort();
        enableButton(connectButton);
        disableButton(startButton);
        disableButton(disconnectButton);
    }

    // Update is called once per frame
    void Update()
    {
        if (connected)
        {
            parseLine();        
            tragetPoint.transform.position = new Vector3((200f+posX*4) *0.265f,
            posY*4 *0.265f,
            tragetPoint.transform.position.z); // Stays
        }
    }

    public void getSelectedOptions() {
        switch (dropdown.value)
        {
            case 0: GameManager.gm.startRace(15); break;
            case 1: GameManager.gm.startRace(30); break;
            case 2: GameManager.gm.startEndless(); break;
        }
    }

    public void QuitGame() {
        Application.Quit(); 
    }

    public void connectSerialReader()
    {
        SerialReader.sr.setPort(inputFieldPort.text);
        SerialReader.sr.ConnectPort();
        connected = true;
        disableButton(connectButton);
        enableButton(startButton);
        enableButton(disconnectButton);
    }

    public void disconnectSerialReader()
    {
        SerialReader.sr.DisconnectPort();
        connected = false;
        enableButton(connectButton);
        disableButton(startButton);
        disableButton(disconnectButton);
    }

    private void disableButton(GameObject button)
    {
        Button b = button.GetComponent<Button>();
        setButtonState(b, false);
    }

    private void enableButton(GameObject button)
    {
        Button b = button.GetComponent<Button>();
        setButtonState(b, true);
    }

    private void setButtonState(Button b, bool state)
    {
        b.interactable = state;
    }

    public void setTextField(string text)
    {
        textField.text = text;
    }


    private void parseLine()
    {
        string line = SerialReader.sr.getLatestLine();
        if (line.Length != 6) 
        {
            return;
        }


        try {
        int i = int.Parse(line.Substring(1,2));
        if (line[0] == 'B'){
            i = i * -1;
        }
        posY = i;

        i = int.Parse(line.Substring(4,2));
        if (line[3] == 'L'){
            i = i * -1;
        }
        posX = i;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log(line);
        }

    }
}
