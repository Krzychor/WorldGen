using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class Player : MonoBehaviour
{
    public RenderModule renderMod;
    public World world;
    public bool inUnloadedChunk = true;
    public int renderDistance = 6;
    public int verticalRenderDist = 3;

    Vector3Int lastChunkPos;

    void Awake()
    {
        inUnloadedChunk = true;
        GetComponent<Rigidbody>().isKinematic = true;
        BlockPlayer();
    }

    private void Start()
    {
        RenderChunksAround();
        RenderChunksAround();
    }

    public void RenderChunksAround()
    {
        Vector3Int chunkPos = Chunk.AbsoPosToChunkCord(transform.position);
        for (int x = chunkPos.x - renderDistance; x < chunkPos.x + renderDistance; x++)
            for (int z = chunkPos.z - renderDistance; z < chunkPos.z + renderDistance; z++)
                for (int y = chunkPos.y - verticalRenderDist; y < chunkPos.y + verticalRenderDist; y++)
                    renderMod.RenderChunk(new Vector3Int(x, y, z));
    }

    public void RenderBorderChunks()
    {
        Vector3Int chunkPos = Chunk.AbsoPosToChunkCord(transform.position);
        Vector3Int min = chunkPos - new Vector3Int(renderDistance, verticalRenderDist, renderDistance);
        Vector3Int max = chunkPos + new Vector3Int(renderDistance, verticalRenderDist, renderDistance);
        for(int x = min.x; x <= max.x; x++)
            for (int y = min.y; y <= max.y; y++)
            {
                renderMod.RenderChunk(new Vector3Int(x, y, min.z));
                renderMod.RenderChunk(new Vector3Int(x, y, max.z));
            }
        for (int z = min.z; z <= max.z; z++)
            for (int y = min.y; y <= max.y; y++)
            {
                renderMod.RenderChunk(new Vector3Int(min.x, y, z));
                renderMod.RenderChunk(new Vector3Int(max.x, y, z));
            }
        for (int x = min.x; x <= max.x; x++)
            for (int z = min.z; z <= max.z; z++)
            {
                renderMod.RenderChunk(new Vector3Int(x, max.y, z));
                renderMod.RenderChunk(new Vector3Int(x, min.y, z));
            }
    }


	void Update ()
    {
        Vector3Int chunkPos = Chunk.AbsoPosToChunkCord(transform.position);
        if(renderMod.IsRendered(chunkPos) && inUnloadedChunk)
            UnblockPlayer();
        else if (!renderMod.IsRendered(chunkPos) && !inUnloadedChunk)
            BlockPlayer();

        if (chunkPos != lastChunkPos)
            RenderBorderChunks();
        lastChunkPos = chunkPos;
    }
    
    void UnblockPlayer()
    {
        inUnloadedChunk = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    void BlockPlayer()
    {
        inUnloadedChunk = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
