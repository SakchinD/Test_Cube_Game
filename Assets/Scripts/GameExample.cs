using UnityEngine;

[CreateAssetMenu(menuName = (nameof(GameExample)))]
public class GameExample : ScriptableObject
{
    [SerializeField] private int cubeCount;
    [SerializeField] private int gridWight;
    [SerializeField] private int gridHight;
    [SerializeField] private int gridCellSize;

    public int CubeCount => cubeCount;
    public int GridWight => gridWight;
    public int GridHight => gridHight;
    public int GridCellSize => gridCellSize;
}
