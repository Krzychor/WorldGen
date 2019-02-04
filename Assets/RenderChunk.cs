using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Threading;


[RequireComponent(typeof(MeshData))]
public class RenderChunk : MonoBehaviour
{
    public Chunk chunk { get; private set; }
    public MeshData meshData { get; private set; }
    public bool isForced;


    void Awake()
    {
        isForced = false;
        meshData = GetComponent<MeshData>();
        chunk = null;
    }

    public void AssignChunk(Chunk C)
    {
        chunk = C;
    }

    public void AssignChunk(RenderChunk RC)
    {
        chunk = RC.chunk;
        RC.DeassignChunk();
        MeshData.Swap(meshData, RC.meshData);
        meshData.Render();
        RC.meshData.Render();
    }

    public void AssignResults(LogicMeshData data, Chunk C)
    {
        chunk = C;
        transform.position = new Vector3(C.x * 32, C.y * 32, C.z * 32);
        meshData.Assign(data);
    }

    public void DeassignChunk()
    {
        chunk = null;
    }

    public void Render()
    {
        if (chunk == null)
            return;
        transform.position = new Vector3();
        meshData.Clear();
        Vector3 pos = new Vector3();
        Vector3 temp = new Vector3();
        Block block;
        Block nextBlock;
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
                    block = chunk.GetBlockLocal(x, y, z);
                    if (block != null && !(block is Air))
                    {
                        for (Direction dir = Direction.North; (int)dir < 6; dir++)
                        {
                            DirectionVector = dir.getVector();
                            temp.x = pos.x + DirectionVector.x; temp.y = pos.y + DirectionVector.y; temp.z = pos.z + DirectionVector.z;
                            //temp = pos + dir.getVector();
                            nextBlock = Info.world.GetBlock(chunk.x*32 + (int)temp.x, chunk.y*32 + (int)temp.y, chunk.z*32 + (int)temp.z);

                            if (nextBlock == null || nextBlock is Air)
                                meshData.AddQuad(pos, dir);
                        }
                    }
                }
            }
        }
        
        meshData.Render();
        transform.position = new Vector3(chunk.x*32, chunk.y*32, chunk.z*32);
    }

}
