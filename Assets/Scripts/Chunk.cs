using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConstChunk
{
    
}

public class Chunk
{
    public static int Size = 32;

    public int x { get; private set; } 
    public int y { get; private set; } 
    public int z { get; private set; }
    uint[,,] blocks;

    bool isEmpty = true; //can be true when chunk is empty


    public bool IsEmpty()
    {
        return isEmpty;
    }

    public uint GetBlockLocal(int x, int y, int z)
    {
        if (isEmpty)
            return 0;
        return blocks[x, y, z];
    }

    public void SetBlockLocal(int x, int y, int z, uint block)
    {
        isEmpty = false;
        if(blocks == null)
        {
            if(block != 0)
            {
                InitializeGrid();
                blocks[x, y, z] = block;

            }
        }
        else
            blocks[x, y, z] = block;
    }

    public Chunk(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
        isEmpty = true;
    }

    void InitializeGrid()
    {
        blocks = new uint[32, 32, 32];
        for (int x = 0; x < 32; x++)
            for (int z = 0; z < 32; z++)
                for (int y = 0; y < 32; y++)
                    blocks[x, y, z] = 0;
    }

    public static Vector3Int AbsoPosToChunkCord(Vector3 pos)
    {
        return AbsoPosToChunkCord(pos.x, pos.y, pos.z);
    }

    public static Vector3Int AbsoPosToChunkCord(float x, float y, float z)
    {
        Vector3Int res = new Vector3Int((int)x / Size, (int)y / Size, (int)z / Size);
        if (x < 0)
            res.x--;
        if (y < 0)
            res.y--;
        if (z < 0)
            res.z--;

        return res;
    }

}
