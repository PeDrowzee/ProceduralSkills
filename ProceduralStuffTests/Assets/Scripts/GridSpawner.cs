using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject tile;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;

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
                Vector3 spawnPosition = new Vector3(x * gridSpacingOffset-gridX/2, 0, z * gridSpacingOffset-gridZ/2) + gridOrigin;
                Instantiate(tile, spawnPosition, Quaternion.identity);
            }
        }
    }
}
