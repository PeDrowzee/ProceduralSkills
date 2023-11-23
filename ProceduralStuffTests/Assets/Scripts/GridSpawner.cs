using UnityEngine;
using System.Collections;

public class GridSpawner : MonoBehaviour
{
    public GameObject tile;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    public int maxNoiseHeight;

    public float gridSpacingOffset = 1f;
    
    private Vector3 gridOrigin;

    private float timer;

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
         timer += Time.deltaTime;
    }

    private void SpawnGrid(){
        for(int x = 0; x < gridX; x++){
            for(int z = 0; z < gridZ; z++){
                //Spawn items at each possible location, starting from world origin (not centered yet)
                Vector3 spawnPosition = 
                new Vector3 //make spawn position
                    (
                        x * gridSpacingOffset-(gridX*gridSpacingOffset/2), 
                        GenerateNoise(x,z,5f) * maxNoiseHeight, 
                        z * gridSpacingOffset-(gridZ*gridSpacingOffset/2)
                    ) + gridOrigin;
                GameObject spawnedTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                spawnedTile.transform.SetParent(transform);
                print(GenerateNoise(x,z,10f));
            }
        }
    }

    private float GenerateNoise(int x, int z, float detailScale) {
        float noiseX = (x+_randomizedX + transform.position.x) / detailScale;
        float noiseZ = (z+_randomizedZ + transform.position.z) / detailScale;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

}
