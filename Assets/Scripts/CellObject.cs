using UnityEngine;

public class CellObject : MonoBehaviour
{
    public bool IsInteractable {  get; private set; }

    public void SetInteractable(bool value)
    {
        IsInteractable = value;
    }
}
