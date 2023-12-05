using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArrayListFunctions : MonoBehaviour
{

    [Header("Reference")]
    [SerializeField] private Transform playerRef;
    [SerializeField] private GameObject tile;
    public List<List<GameObject>> matrix = new();

    [Header("Grid size")]
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [Tooltip("Distance between the origin of adjacen tiles")][SerializeField] private float tilesOffset = 1f;
    //[Space]

    [Header("Noise")]
    [Tooltip("Lower value = more frequent peaks")][SerializeField] [Range(1.05f,50f)] private float frequency;
    [Tooltip("Max height of a peak")][Range(1f,20f)] public int amplitude;
    //[Space]
    public Material debugMat;
   
   
    private float circleRadius;
    private int _randomizedX;
    private int _randomizedZ;
    private Vector3 prevPosition;



    // Start is called before the first frame update
    void Start(){
        circleRadius = gridX>gridZ ? gridZ/2*tilesOffset: gridX/2*tilesOffset;
        _randomizedX = UnityEngine.Random.Range(-10000,10000);
        _randomizedZ = UnityEngine.Random.Range(-10000,10000);
        CreateMatrix();
        prevPosition = playerRef.position;
    }

    
    
    // Update is called once per frame
    void Update()
    {
        if(Mathf.Round(playerRef.position.z)<prevPosition.z){
            MoveMatrixUpDown(matrix,false);
            prevPosition.z=Mathf.Round(playerRef.position.z);
        } else if(Mathf.Round(playerRef.position.z)>prevPosition.z){
            MoveMatrixUpDown(matrix,true);
            prevPosition.z=Mathf.Round(playerRef.position.z);
        }
    }

    private void MoveMatrixUpDown(List<List<GameObject>> myMatrix, bool shouldReverse){
        for(int i = 0; i < myMatrix.Count; i++){
            MoveBottomToTop(myMatrix[i], shouldReverse);
        }
    }

    private void MoveBottomToTop(List<GameObject> myList,bool moveDown){
        int? start;
        int? end;
        
        start = moveDown ? FindTop(myList) : FindBottom(myList);
        end = moveDown ? FindBottom(myList) : FindTop(myList);

        if(start!=null){
            GameObject itemToMove = myList[(int)start];
            //Debug
            itemToMove.GetComponent<MeshRenderer>().material=debugMat;
            //Move to new position
            itemToMove.transform.position = myList[(int)end].transform.position;
            //SlideByOne
            itemToMove.transform.Translate(moveDown?Vector3.forward:-Vector3.forward);
            //Set y with the same noise as before
            itemToMove.transform.position = new(itemToMove.transform.position.x,GenerateNoise((int)itemToMove.transform.position.x+(int)playerRef.position.x,(int)itemToMove.transform.position.z+(int)playerRef.position.z, frequency) * amplitude,itemToMove.transform.position.z);
            //At the end shift it in the list
            myList.RemoveAt((int)start);
            myList.Insert((int)end , itemToMove);
        }
    }

#region LIST HANDLING
    private int? FindTop(List<GameObject> myList){
        for(int i = 0; i<myList.Count; i++){
            if(myList[i] != null){
                return i;
            }
        } return null;
    }

    private int? FindBottom(List<GameObject> myList){
        for(int i = myList.Count-1; i>=0 ; i--){
            if(myList[i] != null){
                return i;
            }
        } return null;
    }
#endregion

    private void CreateMatrix(){
        for(int x = 0; x < gridX; x++){
            List<GameObject> row= new();
            matrix.Add(row);
            for(int z = 0; z < gridZ; z++){

                Vector3 xzPosition = 
                new Vector3 //make spawn position
                    (
                        x * tilesOffset-(gridX*tilesOffset/2), 
                        0, 
                        z * tilesOffset-(gridZ*tilesOffset/2)
                    ) + playerRef.position;
                if(Vector2.Distance(new Vector2(xzPosition.x,xzPosition.z),new Vector2(playerRef.position.x,playerRef.position.z)) <= circleRadius){
                    Vector3 spawnPosition = new(xzPosition.x,GenerateNoise(x,z,frequency)*amplitude,xzPosition.z);
                    row.Add(Instantiate(tile, spawnPosition, Quaternion.identity));
                }
            }
        }
    }

    private float GenerateNoise(int x, int z, float frequence) {
        float noiseX = (x+_randomizedX + transform.position.x) / frequence;
        float noiseZ = (z+_randomizedZ + transform.position.z) / frequence;

        return Mathf.PerlinNoise(noiseX, noiseZ);
    }

}