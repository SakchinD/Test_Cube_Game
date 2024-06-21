using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CellObject Object { get; private set; }

    public GridCell(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetObject(CellObject cellObject)
    {
        Object = cellObject;
    }

    public void RemoveObject()
    {
        Object = null;
    }
}

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private List<GridCell> gridCells = new();
    private Dictionary<(int, int), bool> stateDictionary;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                gridCells.Add( new GridCell(x, y));
    }

    public void Clear()
    {
        gridCells.ForEach(c =>
        {
            c.RemoveObject();
            UpdateStates((c.X, c.Y), c.Object != null);
        });
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 pos = worldPosition - originPosition;
        float percentageX = Mathf.Clamp01(pos.x / width);
        float percentageZ = Mathf.Clamp01(pos.z / height);
        x = Mathf.Clamp(Mathf.RoundToInt(percentageX * width), 0, width - 1);
        y = Mathf.Clamp(Mathf.RoundToInt(percentageZ * height), 0, height - 1);
    }

    public void SetCellObject(int x, int y, CellObject value)
    {
        var cell = GetCell(x, y);
        if (cell != null)
        {
            cell.SetObject(value);
            UpdateStates((cell.X, cell.Y), cell.Object != null);
        }
    }

    public void CreateStates()
    {
        stateDictionary = new Dictionary<(int, int), bool>();
        gridCells.ForEach(cell =>
        {
            var key = (cell.X, cell.Y);
            stateDictionary[key] = cell.Object != null;
        });
    }

    public Dictionary<(int,int), bool> GetCellsStates()
    {
        return stateDictionary;
    }

    public bool CanPlaceObject(int x, int y)
    {
        var cell = GetCell(x, y);
        return cell != null && !cell.Object;
    }

    public void RemoveCellObject(CellObject cellObject)
    {
        var cell = gridCells.Find(x => x.Object == cellObject);
        cell.RemoveObject();
        UpdateStates((cell.X, cell.Y), cell.Object != null);
    }

    private GridCell GetCell(int x, int y)
    {
        return gridCells.Find(c => c.X == x && c.Y == y);
    }

    private void UpdateStates((int,int) key, bool value)
    {
        if (stateDictionary == null) return;

        stateDictionary[key] = value;
    }
}
