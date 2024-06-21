using Photon.Pun;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ItemsPoolController pool;
    [SerializeField] private Transform playerStartPosition;
    [Header("Network")]
    [SerializeField] private CellObject cellObjectPrefab;
    [SerializeField] private PlayerController playerPrefab;
    [Header("Fields")]
    [SerializeField] private Field[] fields;

    private PlayerController player;
    private GameExample gameExample;
    private FieldServise fieldServise;

    [Inject]
    private void Construct(GameExample gameExample)
    {
        this.gameExample = gameExample;
    }

    public void InitGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            fieldServise = new FieldServise(gameExample, pool);
            foreach (Field field in fields)
                fieldServise.AddField(field);

            fieldServise.OnGameComplitedEvent += GameComplited;

            var playerGo = PhotonNetwork.Instantiate(playerPrefab.name, playerStartPosition.position, Quaternion.identity);
            player = playerGo.GetComponent<PlayerController>();
            player.Construct(fieldServise);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            player.transform.SetPositionAndRotation(playerStartPosition.position, Quaternion.identity);
            player.SyncPosition();
            fieldServise.SpawnCubes();
        }
    }

    private void GameComplited()
    {
        StartGame();
    }
}
