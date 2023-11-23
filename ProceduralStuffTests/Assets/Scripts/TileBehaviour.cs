using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Material[] materials;
    // Start is called before the first frame update
    
    
    void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();  
        meshRenderer.material=materials[Random.Range(0,materials.Length)];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
