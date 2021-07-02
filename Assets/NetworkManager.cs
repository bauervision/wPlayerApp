using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDPCore;
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject wifiPlayerPrefab;

    public GameObject myPlayer;

    public Dictionary<string, WifiPlayer> networkPlayers = new Dictionary<string, WifiPlayer>();

    UDPComponent udpClient;

    public int serverPort = 3310;
    public bool serverFound;
    bool waitingSearch;
    public bool serverAlreadyStarted;

    void Start()
    {
        instance = this;
        udpClient = gameObject.GetComponent<UDPComponent>();
        ConnectToServer();
        udpClient.On("PONG", OnPong);
        udpClient.On("JOIN_SUCCESS", OnJoinSuccess);
    }

    public void ConnectToServer()
    {
        if (udpClient.GetServerIP() != string.Empty)
        {
            int randomPort = UnityEngine.Random.Range(3001, 3310);
            udpClient.connect(udpClient.GetServerIP(), serverPort, randomPort);
            UIManager.instance.ServerText.text = "--- Connecting ---";
            UIManager.instance.ClientText.text = "------";
        }
    }

    public void OnPong(UDPEvent data)
    {
        print("NetworkManager Received " + data.pack[1]);
        serverFound = true;
    }

    public void OnJoinSuccess(UDPEvent data)
    {
        print("NetworkManager Join Success: " + data.pack[1]);
        UIManager.instance.ClientText.text = "--- Joined ---";
        UIManager.instance.ClientText.color = Color.green;
    }


    public void EmitJoinServer()
    {
        if (!serverFound)
        {
            NetworkServer.instance.CreateServer();
            udpClient.EmitToServer("JOIN", generateID());
            UIManager.instance.ClientText.text = "--- Joining ---";
        }
    }

    //it generates a random id for the local player
    public string generateID()
    {
        string id = Guid.NewGuid().ToString("N");

        //reduces the size of the id
        id = id.Remove(id.Length - 15);

        return id;
    }

    // Update is called once per frame
    void Update()
    {
        if (udpClient.noNetwork)
        {
            Debug.Log("No Wifi present, please connect to Wifi");
            return;
        }


        if (!serverFound)
            StartCoroutine("PingPong");


    }

    private IEnumerator PingPong()
    {

        if (waitingSearch)
        {
            yield break;
        }

        waitingSearch = true;

        //sends a ping to server
        udpClient.EmitToServer("PING", "sending ping");
        UIManager.instance.ClientText.text = "--- Pinging ---";

        yield return new WaitForSeconds(3);

        waitingSearch = false;

        if (!serverFound)
            EmitJoinServer();

    }

}
