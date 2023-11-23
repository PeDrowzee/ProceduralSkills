using UnityEngine;
using System.Collections;
using System;

public class GridSpawner : MonoBehaviour
{
    public GameObject tile;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [Tooltip("Lower value means 'more randomized' noise")][SerializeField] [Range(1.05f,50f)] private float frequency;
    [Tooltip("Max height reachable by che objects spawned")][Range(1f,20f)] public int amplitude;

    public float gridSpacingOffset = 1f;
    
    private Vector3 gridOrigin;

    private int _randomizedX;
    private int _randomizedZ;

    



    // Start is called before the first frame update
    void Start()
    {
        _randomizedX = UnityEngine.Random.Range(-10000,10000);
        _randomizedZ = UnityEngine.Random.Range(-10000,10000);
        gridOrigin = transform.position;
    
        SpawnGrid();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnGrid(){
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
                GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                spawnedTile.transform.SetParent(transform);
                print(GenerateNoise(x,z,frequency));
            }
        }
    }

    private float GenerateNoise(int x, int z, float frequence) {
        float noiseX = (x+_randomizedX + transform.position.x) / frequence;
        float noiseZ = (z+_randomizedZ + transform.position.z) / frequence;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

}
