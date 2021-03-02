using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class World : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> chunks;


    public bool TryGetChunk(int x, int y, int z, out Chunk chunk)
    {
        return chunks.TryGetValue(new Vector3Int(x, y, z), out chunk);
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        if(!chunks.ContainsKey(new Vector3Int(x, y, z)))
        {
            Chunk C = new Chunk(x, y, z);
            GenerateTerrain(C);
            chunks[new Vector3Int(x, y, z)] = C;
        }
        return chunks[new Vector3Int(x, y, z)];
    }

    void GenerateTerrain(Chunk chunk)
    {
        float posX = chunk.x * Chunk.Size;
        float posY = chunk.y * Chunk.Size;
        float posZ = chunk.z * Chunk.Size;
        for (int x = 0; x < 32; x++)
            for (int z = 0; z < 32; z++)
                for (int y = 0; y < 32; y++)
                {
                    float l = Mathf.PerlinNoise((posX + x) / 100, (posZ + z) / 100) * 20;
                    if (posY + y < l)
                        chunk.SetBlockLocal(x, y, z, 1);

                }
    }

    void Awake()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
	}
	
}
