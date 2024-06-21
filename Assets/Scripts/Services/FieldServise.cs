using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FieldServise : IDisposable
{
    public event Action OnGameComplitedEvent;

    private List<Field> fields = new();

    private GameExample gameExample;
    private ItemsPoolController pool;

    public FieldServise(GameExample gameExample, ItemsPoolController pool)
    {
        this.gameExample = gameExample;
        this.pool = pool;
    }

    public void AddField(Field field)
    {
        fields.Add(field);
        field.CreateGrid();
        if (field.Type != FieldType.Prefabs)
        {
            field.CreateGridState();
            
            if (field.Type == FieldType.Game)
                field.OnGridChangeEvent += CheckFieldCompliting;
        }
    }

    public void Dispose()
    {
        fields.ForEach(x => x.OnGridChangeEvent -= CheckFieldCompliting);
        fields.Clear();
    }

    public bool PlaceCube(int fieldInstanceId, Vector3 worldPos, CellObject cube)
    {
        var field = fields.FirstOrDefault(x => x.transform.GetInstanceID() == fieldInstanceId);
        if (field)
        {
            if(!field.CanPlaceObject(worldPos))
                return false;

            field.PlaceObject(worldPos, cube);
            return true;
        }

        return false;
    }

    public void RemoveCube(int fieldInstanceId, CellObject CellObject)
    {
        var field = fields.FirstOrDefault(x => x.transform.GetInstanceID() == fieldInstanceId);
        if(field)
            field.RemoveCube(CellObject);
    }

    public void SpawnCubes()
    {
        pool.ResetAll();

        fields.ForEach(f => f.ClearGrid());

        SpawnCubesOnFields(FieldType.Example, true);
        SpawnCubesOnFields(FieldType.Prefabs, false);
    }

    private void SpawnCubesOnFields(FieldType type, bool isRandomPos)
    {
        foreach (Field field in fields)
        {
            if (field.Type == type)
                SpawnCubes(field, isRandomPos);   
        }
    }

    private void SpawnCubes(Field field, bool isRandomPos)
    {
        if (isRandomPos)
        {
            for (int i = 0; i < gameExample.CubeCount; i++)
            {
                var cellPosition = GenerateRandomPosition(field);
                var cellObjectGo = pool.GetPooledObject();
                cellObjectGo.SetActive(true);
                var cellObject = cellObjectGo.GetComponent<CellObject>();
                field.PlaceObject(cellPosition.x, cellPosition.y, cellObject);
            }
            return;
        }

        var count = 0;
        for (int y = 0; y < gameExample.GridHight; y++)
        {
            for (int x = 0; x < gameExample.GridWight; x++)
            {
                var cubeGo = pool.GetPooledObject();
                cubeGo.SetActive(true);
                var cube = cubeGo.GetComponent<CellObject>();
                cube.SetInteractable(true);
                cube.transform.SetPositionAndRotation(field.GetWorldPosition(x, y), Quaternion.identity);
                count++;

                if (count == gameExample.CubeCount)
                    return;
            }
        }
    }

    private Vector2Int GenerateRandomPosition(Field field)
    {
        var cellPosition = new Vector2Int(Random.Range(0, gameExample.GridWight), Random.Range(0, gameExample.GridHight));

        if (!field.CanPlaceObject(cellPosition))
            return GenerateRandomPosition(field);

        return cellPosition;
    }

    private void CheckFieldCompliting(Field field)
    {
        var referensField = fields.Find(x => x.Id == field.ReferensFieldId);
        if(referensField != null)
        {
            var complited = true;
            var referensCells = referensField.GetCellsStates();
            var gameCells = field.GetCellsStates();
            foreach (var cell in referensCells)
            {
                if (gameCells[cell.Key] != cell.Value)
                {
                    complited = false;
                    break;
                }
            }
            if (complited)
                OnGameComplitedEvent?.Invoke();
        }
    }
}
