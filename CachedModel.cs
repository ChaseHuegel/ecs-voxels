using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CachedModel
{
    public string name;

    public Vector3[] vertices = new Vector3[0];
    public Vector3[] normals = new Vector3[0];
    public int[] triangles = new int[0];
    public Vector2[] uv = new Vector2[0];
    public Color[] colors = new Color[0];

    public CachedModel(Vector3[] _vertices, Vector3[] _normals, int[] _triangles, Vector2[] _uv, Color[] _colors, string _name = "default")
    {
        vertices = _vertices;
        triangles = _triangles;
        normals = _normals;
        uv = _uv;
        colors = _colors;
        name = _name;
    }
}
