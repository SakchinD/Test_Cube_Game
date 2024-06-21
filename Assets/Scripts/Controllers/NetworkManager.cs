using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private StartUI startUI;
    [SerializeField] private GameManager gameManager;

    private bool isConnected;

    public override void OnConnectedToMaster()
    {
        startUI.SetMessageText("connected");
        isConnected = true;
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            gameManager.InitGame();

        startUI.SetTypeText(PhotonNetwork.IsMasterClient ? "Host" : "Client");
        startUI.SetActiveView(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!newPlayer.IsMasterClient)
            gameManager.StartGame();
    }

    public override void OnLeftRoom()
    {
        startUI.SetActiveView(true);
        startUI.SetTypeText(string.Empty);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void CreateGame()
    {
        if (!isConnected)
        {
            Connecting();
            return;
        }

        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2});
    }

    public void JoinGame()
    {
        if (!isConnected)
        {
            Connecting();
            return; 
        }

        PhotonNetwork.JoinRandomRoom();
    }

    private void Start()
    {
        startUI.Subscribe(CreateGame, JoinGame);
        startUI.SetTypeText(string.Empty);
        Connecting();
    }

    private void Connecting()
    {
        if (PhotonNetwork.IsConnected)
            return;

        isConnected = false;
        startUI.SetMessageText("connecting to server...");

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }
}
