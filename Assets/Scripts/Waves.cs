using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public float scale = 10.0f;
    public float speed = 1.0f;
    public float noiseStrength = 4.0f;
    public float noiseWalk = 1f;

    private Vector3[] baseHeight;

    public MeshFilter meshFilter;
    public Mesh mesh;

    private void Start()
    {
        mesh = meshFilter.mesh; 
    }

    void Update()
    {
        

        if (baseHeight == null)
            baseHeight = mesh.vertices;

        var vertices = new Vector3[baseHeight.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            var vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
