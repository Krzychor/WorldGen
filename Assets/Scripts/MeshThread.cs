using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class MeshThread
{
    public bool shouldStop = false; 
    Thread _thread;
    MeshingResult result;

    ChunkRenderOrder order;
    ConcurrentBag<ChunkRenderOrder> pool; //place to return RenderOrder
    ConcurrentQueue<ChunkRenderOrder> input;
    ConcurrentQueue<MeshingResult> output;


    public MeshThread(ConcurrentBag<ChunkRenderOrder> pool, 
        ConcurrentQueue<ChunkRenderOrder> input, 
        ConcurrentQueue<MeshingResult> output)
    {
        this.pool = pool;
        this.input = input;
        this.output = output;

        _thread = new Thread(ThreadedWork);
        _thread.Start();
    }


    void ThreadedWork()
    {
        while (!shouldStop)
        {
            if(input.TryDequeue(out order))
            {
                Meshing();

                pool.Add(order);
                order = null;

                output.Enqueue(result);
                result = null;
            }
        }
    }


    void Meshing()
    {
        result = new MeshingResult();
        result.position = order.position;
        result.meshData = new LogicMeshData();

        Vector3Int pos = new Vector3Int();
        Vector3Int temp = new Vector3Int();
        uint block;
        uint nextBlock;
        Vector3 DirectionVector;

        for (int x = 0; x < 32; x++)
        {
            pos.x = x;
            for (int z = 0; z < 32; z++)
            {
                pos.z = z;
                for (int y = 0; y < 32; y++)
                {
                    pos.y = y;

                    block = 0;
                    nextBlock = 0;
                    block = order.grid[x+1, y+1, z+1];
                    if (Block.GetId(block) != 0)
                    {
                        for (Direction dir = Direction.North; (int)dir < 6; dir++)
                        {
                            DirectionVector = dir.getVector();
                            temp.x = pos.x + (int)DirectionVector.x; 
                            temp.y = pos.y + (int)DirectionVector.y; 
                            temp.z = pos.z + (int)DirectionVector.z;
                            nextBlock = order.grid[temp.x+1, temp.y+1, temp.z+1];

                            if (Block.GetId(nextBlock) == 0)
                                result.meshData.AddQuad(pos, dir);
                        }
                    }

                }
            }
        }
    }

    public void Close()
    {
        shouldStop = true;
         _thread.Join();
    }
}