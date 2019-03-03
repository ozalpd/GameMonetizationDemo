using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedShader : MonoBehaviour
{
    public float curvature = 2f;
    public float trimming = 0.1f;

    private void Start()
    {
        Shader.SetGlobalFloat("_Curvature", curvature);
        Shader.SetGlobalFloat("_Trimming", trimming);
    }
}
