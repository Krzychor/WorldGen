using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData : MonoBehaviour
{
    LogicMeshData data;

    public void Clear()
    {
        data.Clear();
        Render();
    }
    public void Render()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh.Clear();

        filter.mesh.SetVertices(data.vertices);
        filter.mesh.triangles = data.triangles.ToArray();
        filter.mesh.colors = data.colors.ToArray();
        filter.mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = filter.mesh;
    }

    void Awake ()
    {
        data = null;
	}

    public void Assign(LogicMeshData data)
    {
        this.data = data;
        Render();
    }
}

