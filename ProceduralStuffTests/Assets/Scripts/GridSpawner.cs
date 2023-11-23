using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class GridSpawner : MonoBehaviour
{
    public GameObject tile;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [SerializeField] private int maxNoiseHeight;

    public float gridSpacingOffset = 1f;
    
    private Vector3 gridOrigin = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
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
                        GenerateNoise(x,z,10f) * maxNoiseHeight, 
                        z * gridSpacingOffset-(gridZ*gridSpacingOffset/2)
                    );
                GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                spawnedTile.transform.SetParent(transform);
            }
        }
    }

    private float GenerateNoise(int x, int z, float detailScale) {
        float noiseX = (x + this.transform.position.x) / detailScale;
        float noiseZ = (z + this.transform.position.z) / detailScale;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

}
