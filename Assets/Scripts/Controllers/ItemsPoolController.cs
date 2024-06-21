using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ItemsPoolController : MonoBehaviour
{
    [SerializeField] private CellObject prefab;

    private List<GameObject> pool = new();

    public void ResetAll()
    {
        pool.ForEach(x => x.SetActive(false));
    }

    public GameObject GetPooledObject()
    {
        var item = pool.FirstOrDefault(x => !x.activeSelf);

        if (item == null)
            return CreateObject();

        return item;
    }

    private GameObject CreateObject()
    {
        var item = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
        item.SetActive(false);
        pool.Add(item);
        return item;
    }
}
