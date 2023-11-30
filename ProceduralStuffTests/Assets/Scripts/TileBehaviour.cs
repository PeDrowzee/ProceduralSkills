using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public static int materialIndex;
    
    public GridSpawner spawner; //Not used yet, might come in handy in the future
    public Material[] materials;
    private Color albedo = new Color();
    // Start is called before the first frame update
    

    
    void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();  
        meshRenderer.material=materials[materialIndex];
        albedo = meshRenderer.material.color;
        
        
        albedo = new(albedo.r*(transform.localPosition.y/spawner.amplitude),albedo.g*(transform.localPosition.y/spawner.amplitude),albedo.b*(transform.localPosition.y/spawner.amplitude),albedo.a);
        meshRenderer.material.color = albedo;
       
       
       
       
        //materialIndex++;
        // if(materialIndex>materials.Length-1){
        //     materialIndex=0;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
