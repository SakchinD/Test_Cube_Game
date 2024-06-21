using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnDestroy()
    {
        quitButton.onClick.RemoveListener(Quit);
    }

    private void Quit()
    {
        PhotonNetwork.LeaveRoom(false);
    }
}
