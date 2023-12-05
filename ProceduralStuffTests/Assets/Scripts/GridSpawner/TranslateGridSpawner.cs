using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateGridSpawner : MonoBehaviour
{
#region EXPOSED VARIABLES
    [Header("Reference")]
    public GameObject tile;
    public Transform playerRef;
    //[Space]

    [Header("Grid size")]
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [Tooltip("Distance between the origin of adjacen tiles")][SerializeField] private float tilesOffset = 1f;
    //[Space]

    [Header("Noise")]
    [Tooltip("Lower value = more frequent peaks")][SerializeField] [Range(1.05f,50f)] private float frequency;
    [Tooltip("Max height of a peak")][Range(1f,20f)] public int amplitude;
    //[Space]
    [Header("Shape")]
    [SerializeField] private bool makeCircle;
#endregion

#region INTERNAL VARIABLES
    private int _randomizedX;
    private int _randomizedZ;
    private Vector3 gridOrigin;
    private float circleRadius;
    private List<GameObject> spawnedTiles = new();
    private List<Vector3> spawnedPositions = new();
    private Vector3 prevPosition;

#endregion
        
    // Start is called before the first frame update
    void Start()
    {
        _randomizedX = Random.Range(-10000,10000);
        _randomizedZ = Random.Range(-10000,10000);
        gridOrigin = new(Mathf.Round(playerRef.position.x),playerRef.position.y,Mathf.Round(playerRef.position.z));
        circleRadius=gridX>gridZ ? (gridZ/2)*tilesOffset : (gridX/2)*tilesOffset ;
        prevPosition = playerRef.position;
        SpawnGrid(makeCircle);
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Round(playerRef.position.x)<prevPosition.x){
            TranslateMatrixUpDown(spawnedTiles,false);
            prevPosition.x=Mathf.Round(playerRef.position.x);
        } else if(Mathf.Round(playerRef.position.x)>prevPosition.x){
            TranslateMatrixUpDown(spawnedTiles,true);
            prevPosition.x=Mathf.Round(playerRef.position.x);
        }
    }

    private void SpawnGrid(bool isRound){
        for(int x = 0; x < gridX; x++){
            for(int z = 0; z < gridZ; z++){
                //Spawn items at each possible location, starting from world origin (not centered yet)
                Vector3 spawnPosition = 
                new Vector3 //make spawn position
                    (
                        x * tilesOffset-(gridX*tilesOffset/2), 
                        GenerateNoise(x,z,frequency) * amplitude, 
                        z * tilesOffset-(gridZ*tilesOffset/2)
                    ) + gridOrigin;

                //SPAWN THE ACTUAL CUBES (IF IS ROUND = CIRCLE, ELSE = GRID)
                if(isRound){

                    Vector3 distance = new(spawnPosition.x-gridOrigin.x,0,spawnPosition.z-gridOrigin.z);
                    if(distance.magnitude<=circleRadius){
                        GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                        spawnedTile.transform.SetParent(transform);
                        print(GenerateNoise(x,z,frequency));
                        spawnedTiles.Add(spawnedTile);
                    }

                } else {
                    GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                    spawnedTile.transform.SetParent(transform);
                    print(GenerateNoise(x,z,frequency));
                    spawnedTiles.Add(spawnedTile);
                }
            }
        }
    }

        private float GenerateNoise(int x, int z, float frequence) {
        float noiseX = (x+_randomizedX + transform.position.x) / frequence;
        float noiseZ = (z+_randomizedZ + transform.position.z) / frequence;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

    void TranslateMatrixRigthLeft(List<GameObject> myList, bool isRight){
        foreach(GameObject tile in myList){
            tile.transform.Translate(isRight? Vector3.right : Vector3.left);
        }
    }

    void TranslateMatrixUpDown(List<GameObject> myList, bool isUp){
        foreach(GameObject tile in myList){
            tile.transform.Translate(isUp? Vector3.forward : -Vector3.forward);
        }
    }

}
