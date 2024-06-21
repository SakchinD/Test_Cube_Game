using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float sphereCastRadius = 0.1f;

    private byte eventUpdateCode = 9;
    private Vector3 worldMin;
    private Vector3 worldMax;

    private FieldServise fieldServise;
    private CellObject cube;

    public void Construct(FieldServise fieldServise)
    {
        this.fieldServise = fieldServise;
    }

    public void SyncPosition()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventUpdateCode, new object[] { transform.position, transform.rotation }, options, sendOptions);
    }

    private void Start()
    {
        Camera mainCamera = Camera.main;

        var cameraPosition = mainCamera.transform.position;

        var scale = transform.localScale.x;
        var halfWidth = (mainCamera.aspect * mainCamera.orthographicSize) - scale;
        var halfHeight = mainCamera.orthographicSize - scale;

        worldMin = new Vector3(-halfWidth, transform.position.y, -halfHeight) + cameraPosition;
        worldMax = new Vector3(halfWidth, transform.position.y, halfHeight) + cameraPosition;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("RPC_ButtonInput", RpcTarget.MasterClient, parameters: true);
        }

        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input != Vector3.zero)
            photonView.RPC("RPC_HandleMoveRequest", RpcTarget.MasterClient, input);
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    private void RPC_HandleMoveRequest(Vector3 input)
    {
        Vector3 movementDirection = new Vector3(input.x, 0, input.z).normalized;

        if (movementDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }

        transform.position += input * speed * Time.deltaTime;

        transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, worldMin.x, worldMax.x),
                    transform.position.y,
                    Mathf.Clamp(transform.position.z, worldMin.z, worldMax.z));

        photonView.RPC("RPC_UpdatePosition", RpcTarget.AllBuffered, transform.position);
        photonView.RPC("RPC_UpdateRotation", RpcTarget.AllBuffered, transform.rotation);
    }

    [PunRPC]
    private void RPC_UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [PunRPC]
    private void RPC_UpdateRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

    [PunRPC]
    private void RPC_ButtonInput(bool message)
    {
        var hits = Physics.SphereCastAll(target.position, sphereCastRadius, Vector3.down, layerMask);

        if (cube)
        {
            var fieldHit = hits.FirstOrDefault(x => x.collider.CompareTag("Field"));
            if (fieldHit.collider != null)
            {
                if(fieldServise.PlaceCube(fieldHit.transform.GetInstanceID(), 
                    target.position, cube))
                {
                    cube = null;
                }
                return;
            }

            cube.transform.SetParent(null);
            cube = null;
            return;
        }

        var cubeHit = hits.FirstOrDefault(x => x.collider.CompareTag("Cube"));
        if (cubeHit.collider != null)
        {
            cube = cubeHit.transform.GetComponent<CellObject>();
            if (cube.IsInteractable)
            {
                cube.transform.position = new Vector3(target.position.x, cube.transform.position.y, target.position.z);
                cube.transform.SetParent(target);

                var fieldHit = hits.FirstOrDefault(x => x.collider.CompareTag("Field"));
                if (fieldHit.collider != null)
                    fieldServise.RemoveCube(fieldHit.transform.GetInstanceID(), cube);
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == eventUpdateCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            transform.position = (Vector3)data[0];
            transform.rotation = (Quaternion)data[1];

        }
    }
}
