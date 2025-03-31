using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTexture : MonoBehaviour
{
    public static MakeTexture Instance;
    private void Awake()
    {
        // ΩÃ±€≈Ê º≥¡§
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    

    private void Start()
    {
        //Renderer renderer = GetComponent<Renderer>();
        //if (renderer != null)
        //{
        //    renderer.material.mainTexture = GenerateTexture();
        //}
    }

    
}
