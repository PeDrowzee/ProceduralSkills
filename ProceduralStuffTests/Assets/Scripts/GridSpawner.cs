using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class GridSpawner : MonoBehaviour
{
    public GameObject tile;
    public Transform player;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [SerializeField] private bool isCircle;
    [SerializeField] private bool clearTrace;
    [Tooltip("Lower value means 'more randomized' noise")][SerializeField] [Range(1.05f,50f)] private float frequency;
    [Tooltip("Max height reachable by che objects spawned")][Range(1f,20f)] public int amplitude;
    
    public float gridSpacingOffset = 1f;
    
    private Vector3 gridOrigin;

    private int _randomizedX;
    private int _randomizedZ;
    private float circleRadius;


    private List<GameObject> spawnedTiles = new();
    private List<Vector3> spawnedPositions = new();
    private List<Vector3> newSpawnPositions = new();
    private List<int> indexToRemove = new();


    // Start is called before the first frame update
    void Start()
    {
        _randomizedX = UnityEngine.Random.Range(-10000,10000);
        _randomizedZ = UnityEngine.Random.Range(-10000,10000);
        gridOrigin = new(Mathf.Round(player.position.x),player.position.y,Mathf.Round(player.position.z));
    
        circleRadius=gridX/2;
        SpawnGrid(isCircle);
        
    }
    // Update is called once per frame
    void Update()
    {
        //MAKE CUBES FOLLOW THE PLAYER (IF IS ROUND = CIRCLE, ELSE = GRID)
        LookForNewPositions(isCircle);
    }

    private void SpawnGrid(bool isRound){
        for(int x = 0; x < gridX; x++){
            for(int z = 0; z < gridZ; z++){
                //Spawn items at each possible location, starting from world origin (not centered yet)
                Vector3 spawnPosition = 
                new Vector3 //make spawn position
                    (
                        x * gridSpacingOffset-(gridX*gridSpacingOffset/2), 
                        GenerateNoise(x,z,frequency) * amplitude, 
                        z * gridSpacingOffset-(gridZ*gridSpacingOffset/2)
                    ) + gridOrigin;

                //SPAWN THE ACTUAL CUBES (IF IS ROUND = CIRCLE, ELSE = GRID)
                if(isRound){

                    Vector3 distance = new(spawnPosition.x-gridOrigin.x,0,spawnPosition.z-gridOrigin.z);
                    if(distance.magnitude<=circleRadius){
                        GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                        spawnedTile.transform.SetParent(transform);
                        print(GenerateNoise(x,z,frequency));
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
        if(Mathf.Round(player.transform.position.x)!=gridOrigin.x || Mathf.Round(player.transform.position.z)!=gridOrigin.z){
            gridOrigin = new(Mathf.Round(player.position.x), gridOrigin.y, Mathf.Round(player.position.z));
            //Empty the prev positions list
            newSpawnPositions.Clear();
            for(int x = 0; x < gridX; x++){
                for(int z = 0; z < gridZ; z++){
                    int playerX = (int)Math.Round(player.transform.position.x);
                    int playerZ = (int)Math.Round(player.transform.position.z);
                    //Spawn items at each possible location, starting from world origin (not centered yet)
                    Vector3 newSpawnPosition = 
                    new Vector3 //make spawn position
                        (
                            Mathf.Round(x)* gridSpacingOffset-(gridX*gridSpacingOffset/2), 
                            GenerateNoise(x+playerX,z+playerZ,frequency) * amplitude, 
                            Mathf.Round(z) * gridSpacingOffset-(gridZ*gridSpacingOffset/2)
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

            //Prima controllo quali punti della griglia precedente non esistono nella nuova griglia.
            //Quelli che non esistono piu li prendo e li rimuovo, facendo Destroy sullo specifico cubo e poi eliminando lo slot vuoto rimasto nella lista
            
            // foreach(GameObject singleTile in spawnedTiles){   
            //     if(newSpawnPositions.Contains(singleTile.transform.position)){
            //         //OK
            //     }   else {
            //         Destroy(singleTile);

            //         spawnedTiles.Remove(singleTile);
            //     }
            // }
            
            SpawnNewInBoundsCubes();
            if(clearTrace){
                DestroyOutOfBoundsCubes();
            }
            //Poi ricontrollo quali dei nuovi valori non sono nella lista precedente.
            //Quelli che non ci sono li uso come spawnPoint per dei nuovi tile, che aggiungerò alla lista originale.
            //In questo modo la mia lista sarà sempre aggiornata ogni volta che il player cambia location. I cubi ancora validi non dovrebbero despawnare
                
            // }
            // foreach(GameObject singleTile in spawnedTiles){   
            //     if(newSpawnPositions.Contains(singleTile.transform.position)){
            //         //OK
            //     }   else {
            //         Destroy(singleTile);
            //         spawnedTiles.Remove(singleTile);
            //     }
            // }
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
                //Distruggi il cubo in quella posizione introvabile
                int index = spawnedPositions.FindIndex(position => position.Equals(spot));
                indexToRemove.Add(index);
            }
        }
        foreach(int index in indexToRemove){
            Destroy(spawnedTiles[index]);
            spawnedTiles.Remove(spawnedTiles[index]);
            spawnedPositions.Remove(spawnedPositions[index]);
        }
        indexToRemove.Clear();
    }
}
