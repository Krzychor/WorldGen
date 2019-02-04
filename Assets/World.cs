using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class World : MonoBehaviour
{
    Mutex mutex;
    List<Chunk> chunks;
    public static Block air;
    public static Block stone;


    public Block GetBlock(int x, int y, int z)
    {
        Chunk C = GetChunk(new Vector3(x, y, z));
        if (C != null)
            return C.GetBlockAbso(x, y, z);
        else
            return null;
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        Chunk C = GetChunk(new Vector3(x, y, z));
        C.SetBlockAbso(x, y, z, block);
    }

    public void RequestChunk(int ChunkX, int ChunkY, int ChunkZ)
    {
   //     Debug.Log("request");
    //    Chunk C = GetChunk(ChunkX, ChunkY, ChunkZ);
   //     if (C == null)
   //         C = GenerateChunk(ChunkX, ChunkY, ChunkZ);
        
    }

    public Chunk GetChunk(Vector3 pos)
    {
        int x = (int) pos.x / 32;
        int y = (int) pos.y / 32;
        int z = (int) pos.z / 32;

        for(int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i].Contain(pos))
                return chunks[i];
        }
        return null;
    }

    public Chunk GetChunk(int ChunkPosX, int ChunkPosY, int ChunkPosZ)
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i].x == ChunkPosX && chunks[i].y == ChunkPosY && chunks[i].z == ChunkPosZ)
                return chunks[i];
        }
        return null;
    }

    public bool isGenerated(Chunk C)
    {
        return true;
    }

    public void Generate()
    {
        Debug.Log("generate");
        int PChX = (int)Info.player.transform.position.x / 32;
        int PChY = (int)Info.player.transform.position.y / 32;
        int PChZ = (int)Info.player.transform.position.z / 32;
        Chunk C;


        return;

        int l = 0;
        for (int Ix = 0; Ix < 10; Ix++)
        {
            for(int Iy = 0; Iy < 10; Iy++)
            {
                for(int Iz = 0; Iz < 10; Iz++)
                {
                    C = new Chunk(Ix+PChX, Iy+PChY, Iz+PChZ);
                    chunks.Add(C);
                    for (int x = 0; x < 32; x++)
                    {
                        for (int z = 0; z < 32; z++)
                        {
                            for (int y = 0; y < 32; y++)
                                C.SetBlockAbso(x, y, z, air);

                            l = (int) (Mathf.PerlinNoise((float)(x+Ix*32+PChX*32)/100f, (float)(z+Iz*32+PChZ*32)/100f)*20);
                            l += (int)(40*Mathf.PerlinNoise((float)(Ix+PChX)/100f, (float) (Iz+PChZ)/100f) );
                            for (int y = 0; Iy * 32 + y < l; y++)
                            {
                                C.SetBlockAbso(x, y, z, stone);
                            }
                        }
                    }
                }
            }
        }
     //   Debug.Log("loaded chunks = " + chunks.Count);
    }

    public void GenerateAround(int round, int height)
    {
        Debug.Log("generate");
        int PChX = (int)Info.player.transform.position.x / 32;
        int PChY = (int)Info.player.transform.position.y / 32;
        int PChZ = (int)Info.player.transform.position.z / 32;
        Chunk C;

        for (int x = 0; x < round; x++)
        {
            for(int z = 0; z < round; z++)
            {
                for(int y = 0; y < height; y++)
                {
                    GenerateChunk(PChX - round/2 + x, PChY - height/2 + y, PChZ - round/2 + z);
                }
            }
        }
    }

    void GenerateChunk(Chunk C, int CX, int CY, int CZ)
    {
    //    Debug.Log(CX + "," + CY + "," + CZ);
        int s = 0;
        int l;
        int AbsoH = C.y*32;
        for(int x = 0; x < 32; x++)
        {
            for(int z = 0; z < 32; z++)
            {
                for (int y = 0; y < 32; y++)
                    C.SetBlockLocal(x, y, z, air);
                l = (int)(Mathf.PerlinNoise((float)(x + C.x*32) / 100f, (float)(z + C.z * 32) / 100f) * 20);

                for (int y = 0; y + AbsoH < l && y < 32; y++)
                {
                    C.SetBlockAbso(x, y, z, stone);
                    s++;
                }
            }
        }
  //      Debug.Log("stone = " + s);
    }

    Chunk GenerateChunk(int CX, int CY, int CZ)
    {
        Chunk C = GetChunk(CX, CY, CZ);
        if(C == null)
        {
            C = new Chunk(CX, CY, CZ);
            chunks.Add(C);
            GenerateChunk(C, CX, CY, CZ);
        }
        else
            GenerateChunk(C, CX, CY, CZ);
        return C;
    }

    void Start()
    {
        GenerateAround(10, 3);
        Debug.Log("generated");
        Info.player.gameObject.SetActive(true);
    }

    void Awake()
    {
        mutex = new Mutex();
        Info.world = this;
        chunks = new List<Chunk>();
        air = new Air();
        stone = new Stone();
	}
	
	void Update ()
    {
		
	}

}
