using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private TileBehaviour[] tiles;
    [SerializeField] private Vector2Int size = new(25, 25);
    [SerializeField] private GameObject building;

    [SerializeField] private Color backgroundColor = new(1, 0, 0);
    [SerializeField] private Color roadColor = new(0, 1, 0);
    [SerializeField] private Color lineColor = new(0, 0, 1);

    private TileBehaviour[,] city;
    private List<GameObject> buildings = new();
    private GameObject buildingGroup;

    void Start()
    {
        buildingGroup = new("Buidlings");
        foreach (var item in tiles)
        {
            var mat = item.GetComponent<Renderer>().sharedMaterial;
            mat.SetColor("_BackgroundColor", backgroundColor);
            mat.SetColor("_RoadColor", roadColor);
            mat.SetColor("_LineColor", lineColor);
        }
        transform.localScale = new((int)transform.localScale.x, 1, (int)transform.localScale.x);
        city = FillMatrix();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearMatrix(city);
            foreach (GameObject building in buildings)
            {
                Destroy(building);
            }
            buildings.Clear();
            city = FillMatrix();
        }
    }


    private TileBehaviour[,] FillMatrix()
    {
        Vector2 distance = new(transform.localScale.x / size.x, transform.localScale.z / size.y);
        Vector2 localOffset = distance / 2;
        TileBehaviour[,] spawnedItems = new TileBehaviour[(int)size.x, (int)size.y];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector2 matrixToCenter = new(x - size.x / 2, y - size.y / 2);
                Vector3 spawnPosition = new
                    (matrixToCenter.x * distance.x + localOffset.x,
                    0f,
                    matrixToCenter.y * distance.y + localOffset.x);

                if (CheckIfMatrixEdge(new(x,y)))
                {
                    spawnedItems[x, y] = InstantiateTile(spawnPosition,0, 0); // Spawn 0WaysT ile on the Edge
                }
                else
                {
                    List<TileBehaviour> possibleTiles = ListOfPossibleTiles(spawnedItems, new(x, y), spawnPosition);
                    //migliorare la funzione random in modo che abbia dei pesi per ciascun tile
                    int randomTile = Random.Range(0, possibleTiles.Count);
                    possibleTiles[randomTile].gameObject.SetActive(true);
                    spawnedItems[x, y] = possibleTiles[randomTile];
                    if (possibleTiles[randomTile].Name == "0Ways")
                    {
                        
                        GameObject go = Instantiate(building, spawnPosition, Quaternion.identity, buildingGroup.transform);
                        float randomHeight = Random.Range(1, 5);
                        go.transform.localScale = new Vector3(1, randomHeight, 1);
                        go.transform.localScale *= size.x /2;
                        buildings.Add(go);
                    }
                    ClearList(possibleTiles, randomTile);
                }
            }
        }
        return spawnedItems;
    }

    private void ClearMatrix(TileBehaviour[,] matrix)
    {
        for (int y = 0; y < matrix.GetLength(0); y++)
        {
            for (int x = 0; x < matrix.GetLength(1); x++)
            {
                //Non avevo voglia di fare pooling, avrei dovuto fare un bel po' di refactoring
                Destroy(matrix[x, y].gameObject);
            }
        }
    }


    private bool CheckIfMatrixEdge(Vector2Int currentMatrixPosition)
    {
        return
            currentMatrixPosition.x == 0 ||
            currentMatrixPosition.x == size.x - 1 ||
            currentMatrixPosition.y == 0 ||
            currentMatrixPosition.y == size.y - 1;
    }

    private TileBehaviour InstantiateTile (Vector3 spawnPosition, int index, int direction)
    {
        TileBehaviour tb = Instantiate(tiles[index], spawnPosition, Quaternion.identity, transform);
        tb.transform.localScale /= size.x;
        tb.CurrentOrientation = tb.GetPossibleOrientations(direction);
        return tb;
    }

    private bool[] CalculateNeededOrientation(TileBehaviour[,] matrix, Vector2Int matrixIndex)
    {
        bool[] orientation = new bool[4];
        orientation[0] = true;
        orientation[1] = true;
        //Calculate the needed orientation of this specific tile
        orientation[2] = matrix[matrixIndex.x, matrixIndex.y - 1].CurrentOrientation[0];
        orientation[3] = matrix[matrixIndex.x - 1, matrixIndex.y].CurrentOrientation[1];

        return orientation;
    }

    private List<TileBehaviour> ListOfPossibleTiles(TileBehaviour[,] matrix, Vector2Int matrixIndex, Vector3 spawnPosition)
    {
        bool[] neededOrientation = CalculateNeededOrientation(matrix, matrixIndex);
        List<TileBehaviour> possibleTiles = new();
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].PossibleOrientations; j++)
            {
                if (tiles[i].GetPossibleOrientations(j)[2] == neededOrientation[2] && tiles[i].GetPossibleOrientations(j)[3] == neededOrientation[3])
                {
                    // 
                    //Da fixare questa merda porca madonna che devo istanziare per non perdermi i riferimenti
                    //
                    TileBehaviour tb = InstantiateTile(spawnPosition, i, j);
                    tb.SetActive(false);
                    possibleTiles.Add(tb);
                }
            }
        }
        return possibleTiles;
    }

    private void ClearList(List<TileBehaviour> possibleTiles, int indexToSave)
    {
        //brutto brutto brutto ma non ne uscivo
        possibleTiles.RemoveAt(indexToSave);
        foreach (TileBehaviour tile in possibleTiles) Destroy(tile.gameObject);
        possibleTiles.Clear();
    }
}
