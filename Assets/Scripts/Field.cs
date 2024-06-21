using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum FieldType
{
    Example,
    Game,
    Prefabs
}

public class Field : MonoBehaviour
{
    public event Action<Field> OnGridChangeEvent;

    [SerializeField] private FieldType type;
    [SerializeField] private int id;
    [SerializeField] private int referensFieldId;

    private Grid grid;

    private GameExample gameExample;

    public FieldType Type => type;
    public int Id => id;
    public int ReferensFieldId => referensFieldId;

    [Inject]
    private void Construct(GameExample gameExample)
    {
        this.gameExample = gameExample;
    }

    public void CreateGrid()
    {
        grid = new Grid(gameExample.GridWight, gameExample.GridHight, gameExample.GridCellSize, transform.position);
    }

    public void CreateGridState()
    {
        grid.CreateStates();
    }

    public void ClearGrid()
    {
        grid.Clear();
    }

    public void RemoveCube(CellObject cellObject)
    {
        grid.RemoveCellObject(cellObject);
    }

    public bool CanPlaceObject(Vector2Int cell)
    {
        return grid.CanPlaceObject(cell.x, cell.y);
    }

    public bool CanPlaceObject(Vector3 worldPos)
    {
        grid.GetXY(worldPos, out var x, out var y);
        return grid.CanPlaceObject(x, y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return grid.GetWorldPosition(x, y);
    }

    public void PlaceObject(Vector3 worldPos, CellObject cellObject)
    {
        grid.GetXY(worldPos, out var x, out var y);
        PlaceObject(x, y, cellObject, true);
    }

    public void PlaceObject(int x, int y, CellObject cellObject, bool canReplace = false)
    {
        grid.SetCellObject(x, y, cellObject);

        cellObject.transform.SetParent(transform);
        cellObject.SetInteractable(canReplace);
        cellObject.transform.SetPositionAndRotation(grid.GetWorldPosition(x, y), Quaternion.identity);
        OnGridChangeEvent?.Invoke(this);
    }

    public Dictionary<(int, int), bool> GetCellsStates()
    {
        return grid.GetCellsStates();
    }
}
