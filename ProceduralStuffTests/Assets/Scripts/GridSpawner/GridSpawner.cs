using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Linq;

public class GridSpawner : MonoBehaviour
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

    [Header("Shape and behaviour")]
    [SerializeField] private bool makeCircle;
    [SerializeField] private bool clearTrace;
#endregion
    
#region INTERNAL VARIABLES
    
    private Vector3 gridOrigin;

    private int _randomizedX;
    private int _randomizedZ;
    private float circleRadius;


    private List<GameObject> spawnedTiles = new();
    private List<Vector3> spawnedPositions = new();
    private List<Vector3> newSpawnPositions = new();
    private List<int> indexToRemove = new();
#endregion

    // Start is called before the first frame update
    void Start()
    {
        _randomizedX = UnityEngine.Random.Range(-10000,10000);
        _randomizedZ = UnityEngine.Random.Range(-10000,10000);
        gridOrigin = new(Mathf.Round(playerRef.position.x),playerRef.position.y,Mathf.Round(playerRef.position.z));
    
        circleRadius=gridX>gridZ ? (gridZ/2)*tilesOffset : (gridX/2)*tilesOffset ;
        SpawnGrid(makeCircle);
        
    }
    // Update is called once per frame
    void Update()
    {
        //MAKE CUBES FOLLOW THE PLAYER (IF IS ROUND = CIRCLE, ELSE = GRID)
        LookForNewPositions(makeCircle);
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
                        //print(GenerateNoise(x,z,frequency));
                        spawnedTiles.Add(spawnedTile);
                        spawnedPositions.Add(spawnPosition);
                    }

                } else {
                    GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                    spawnedTile.transform.SetParent(transform);
                    print(GenerateNoise(x,z,frequency));
                    spawnedTiles.Add(spawnedTile);
                    spawnedPositions.Add(spawnPosition);
                }
            }
        }
    }

    private float GenerateNoise(int x, int z, float frequence) {
        float noiseX = (x+_randomizedX + transform.position.x) / frequence;
        float noiseZ = (z+_randomizedZ + transform.position.z) / frequence;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

    private void FollowingBlocks(bool isRound)
    {
        if(isRound){
            
        }
    }

    private void SpawnTile(Vector3 spawnPosition){
        GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
        spawnedTile.transform.SetParent(transform);
        //print(GenerateNoise(x,z,frequency));
        spawnedTiles.Add(spawnedTile);
        spawnedPositions.Add(spawnPosition);
    }

    private void LookForNewPositions(bool isRound){
        if(Mathf.Round(playerRef.position.x)!=gridOrigin.x || Mathf.Round(playerRef.position.z)!=gridOrigin.z){
            gridOrigin = new(Mathf.Round(playerRef.position.x), gridOrigin.y, Mathf.Round(playerRef.position.z));
            //Empty the prev positions list
            newSpawnPositions.Clear();
            for(int x = 0; x < gridX; x++){
                for(int z = 0; z < gridZ; z++){ 
                    int playerX = (int)Math.Round(playerRef.position.x);
                    int playerZ = (int)Math.Round(playerRef.position.z);
                    //Spawn items at each possible location, starting from world origin (not centered yet)
                    Vector3 newSpawnPosition = 
                    new Vector3 //make spawn position
                        (
                            Mathf.Round(x)* tilesOffset-(gridX*tilesOffset/2), 
                            GenerateNoise(x+playerX,z+playerZ,frequency) * amplitude, 
                            Mathf.Round(z) * tilesOffset-(gridZ*tilesOffset/2)
                        ) + gridOrigin;
                        
                    if(isRound){

                        Vector3 distance = new(newSpawnPosition.x-gridOrigin.x,0,newSpawnPosition.z-gridOrigin.z);
                        if(distance.magnitude<=circleRadius){
                            newSpawnPositions.Add(newSpawnPosition);
                        } else {

                        }
                    }
                }
            }

          
            
            SpawnNewInBoundsCubes();
            if(clearTrace){
                DestroyOutOfBoundsCubes();
            }

            print(spawnedTiles.Count);
        }
    }

    private void SpawnNewInBoundsCubes(){
        foreach(Vector3 spot in newSpawnPositions){
                if(spawnedPositions.Contains(spot)){

                } else {
                    SpawnTile(spot);
                }
            }
    }

    private void DestroyOutOfBoundsCubes(){
        indexToRemove.Clear();
        foreach(Vector3 spot in spawnedPositions){
            if(newSpawnPositions.Contains(spot)){
                //Bene
            } else {
                
                int index = spawnedPositions.FindIndex(position => position.Equals(spot));
                indexToRemove.Add(index);
            }
        }
        int i = 0;
        foreach(int index in indexToRemove){
            Destroy(spawnedTiles[index-i]);
            spawnedTiles.RemoveAt(index-i);
            spawnedPositions.RemoveAt(index-i);
            i+=1;
        }
        indexToRemove.Clear();
    }
}
