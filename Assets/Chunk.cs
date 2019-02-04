using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public int x { get; private set; }
    public int y { get; private set; } 
    public int z { get; private set; }
    Block[,,] chunk;


    public Block GetBlockAbso(int x, int y, int z)
    {
        x = x % 32; y = y % 32; z = z % 32;
        if (x < 0) x = -1 * x;
        if (y < 0) y = -1 * y;
        if (z < 0) z = -1 * z;
        return chunk[x, y, z];
    }

    public Block GetBlockLocal(int x, int y, int z)
    {
        return chunk[x, y, z];
    }

    public void SetBlockAbso(int x, int y, int z, Block B)
    {
        x = x % 32; y = y % 32; z = z % 32;
        chunk[x, y, z] = B;
    }

    public void SetBlockLocal(int x, int y, int z, Block B)
    {
        chunk[x, y, z] = B;
    }

    public Chunk(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        chunk = new Block[32, 32, 32];
    }
    
    public bool Contain(Vector3 pos)
    {
        if (x * 32 <= pos.x && pos.x < x * 32 + 32 && y * 32 <= pos.y && pos.y < y * 32 + 32 && z * 32 <= pos.z && pos.z < z * 32 + 32)
            return true;
        else
            return false;
    }
}
