using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Material[] materials;
    private Color albedo = new Color();
    // Start is called before the first frame update
    
    
    void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();  
        meshRenderer.material=materials[Random.Range(0,materials.Length)];
        albedo = meshRenderer.material.color;
        
        
        albedo = new(albedo.r,albedo.g,albedo.b*(1-transform.localPosition.y/4),albedo.a);
        meshRenderer.material.color = albedo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
