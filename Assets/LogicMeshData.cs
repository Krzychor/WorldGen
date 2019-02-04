using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicMeshData
{
    // remember to update swap after adding new members
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Color> colors;



    public void AddQuad(Vector3 pos, Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                {
                    pos.z++;
                    AddQuadINVERSED(pos, pos + new Vector3(1, 0, 0), pos + new Vector3(1, 1, 0), pos + new Vector3(0, 1, 0));
                    break;
                }
            case Direction.East:
                {
                    pos.x++;
                    AddQuad(pos, pos + new Vector3(0, 0, 1), pos + new Vector3(0, 1, 1), pos + new Vector3(0, 1, 0));
                    break;
                }
            case Direction.South:
                {
                    AddQuad(pos, pos + new Vector3(1, 0, 0), pos + new Vector3(1, 1, 0), pos + new Vector3(0, 1, 0));
                    break;
                }
            case Direction.West:
                {
                    AddQuadINVERSED(pos, pos + new Vector3(0, 0, 1), pos + new Vector3(0, 1, 1), pos + new Vector3(0, 1, 0));
                    break;
                }
            case Direction.Up:
                {
                    pos.y++;
                    AddQuad(pos, pos + new Vector3(1, 0, 0), pos + new Vector3(1, 0, 1), pos + new Vector3(0, 0, 1));
                    break;
                }
            case Direction.Down:
                {
                    AddQuadINVERSED(pos, pos + new Vector3(1, 0, 0), pos + new Vector3(1, 0, 1), pos + new Vector3(0, 0, 1));
                    break;
                }
        }
    }

    void AddQuad(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
        vertices.Add(D);
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 4);
    }

    void AddQuadINVERSED(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
        vertices.Add(D);
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
    }

    public void AddTriangle(Vector3 A, Vector3 B, Vector3 C)
    {
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
    }

    public void AddTriangleINVERSED(Vector3 A, Vector3 B, Vector3 C)
    {
        vertices.Add(A);
        vertices.Add(B);
        vertices.Add(C);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
        colors.Add(Color.grey);
    }


    public void Clear()
    {
        colors.Clear();
        vertices.Clear();
        triangles.Clear();
    }

    public LogicMeshData()
    {
        colors = new List<Color>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }
}

