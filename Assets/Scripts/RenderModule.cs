using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;



public class ChunkRenderOrder
{
    public Vector3Int position;
    public uint[,,] grid;


    public ChunkRenderOrder()
    {
        grid = new uint[Chunk.Size+2, Chunk.Size+2, Chunk.Size+2];
    }

}

public class MeshingResult
{
    public Vector3Int position;
    public LogicMeshData meshData;
}

public class RenderModule : MonoBehaviour
{
    public GameObject RenderChunkPrefab;
    public World world;
    public Transform player;

    [Min(1)]
    public int MaxChunksRenderedPerFrame = 20;
    [Min(1)]
    public int MaxChunksPreparedPerFrame = 20; 
    ObjectPool renderedPool;
    readonly Dictionary<Vector3Int, MeshData> Rendered = new Dictionary<Vector3Int, MeshData>();
    readonly ChunkRenderQueue<Vector3Int> ToRender = new ChunkRenderQueue<Vector3Int>();
    readonly ConcurrentQueue<MeshingResult> renderResults = new ConcurrentQueue<MeshingResult>();
    readonly ConcurrentQueue<ChunkRenderOrder> readyToRender = new ConcurrentQueue<ChunkRenderOrder>();
    readonly ConcurrentBag<ChunkRenderOrder> ordersPool = new ConcurrentBag<ChunkRenderOrder>();
    public int PoolSize = 10;

    public MeshThread meshingThread;

    void PrepareRenderOrders()
    {
        ChunkRenderOrder order;
        Chunk chunk;
        int preparedOrders = 0;
        while (ToRender.Count > 0)
        {
            if(preparedOrders > MaxChunksPreparedPerFrame)
                return;


            if (ordersPool.TryTake(out order) == false)
                return;

            preparedOrders++;

            order.position = ToRender.Dequeue();
            chunk = world.GetChunk(order.position.x, order.position.y, order.position.z); 
            if(chunk.IsEmpty())
            {
                ordersPool.Add(order);
                RenderEmptyChunk(order.position); //early exit
                continue;
            }

            for (int x = 0; x < Chunk.Size; x++)
                for (int z = 0; z < Chunk.Size; z++)
                    for (int y = 0; y < Chunk.Size; y++)
                        order.grid[x+1, y+1, z+1] = chunk.GetBlockLocal(x, y, z);


            Chunk nextChunk; //neighbour chunk
            //filling bottom area
            if (world.TryGetChunk(order.position.x, order.position.y - 1, order.position.z, out nextChunk))  
            {
                for (int x = 0; x < Chunk.Size; x++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[x + 1, 0, z + 1] = nextChunk.GetBlockLocal(x, Chunk.Size - 1, z);
            }
            else
            {
                for (int x = 0; x < Chunk.Size; x++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[x + 1, 0, z + 1] = 0;
            }


            //filling top area
            if(world.TryGetChunk(order.position.x, order.position.y + 1, order.position.z, out nextChunk))
            {
                for (int x = 0; x < Chunk.Size; x++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[x + 1, Chunk.Size + 1, z + 1] = nextChunk.GetBlockLocal(x, 0, z);
            }
            else
            {
                for (int x = 0; x < Chunk.Size; x++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[x + 1, Chunk.Size + 1, z + 1] = 0;
            }

            //filling east area (max x)
            if(world.TryGetChunk(order.position.x + 1, order.position.y, order.position.z, out nextChunk))
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[Chunk.Size + 1, y + 1, z + 1] = nextChunk.GetBlockLocal(0, y, z);
            }
            else
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[Chunk.Size + 1, y + 1, z + 1] = 0;
            }

            //filling west area (min x)
            if (world.TryGetChunk(order.position.x - 1, order.position.y, order.position.z, out nextChunk))
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[0, y + 1, z + 1] = nextChunk.GetBlockLocal(Chunk.Size - 1, y, z);
            }
            else
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int z = 0; z < Chunk.Size; z++)
                        order.grid[0, y + 1, z + 1] = 0;
            }


            //filling north area (max z)
            if (world.TryGetChunk(order.position.x, order.position.y, order.position.z + 1, out nextChunk))
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int x = 0; x < Chunk.Size; x++)
                        order.grid[x + 1, y + 1, Chunk.Size + 1] = nextChunk.GetBlockLocal(x, y, 0);
            }
            else
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int x = 0; x < Chunk.Size; x++)
                        order.grid[x + 1, y + 1, Chunk.Size + 1] = 0;
            }

            //filling south area (min z)
            if (world.TryGetChunk(order.position.x, order.position.y, order.position.z - 1, out nextChunk))
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int x = 0; x < Chunk.Size; x++)
                        order.grid[x + 1, y + 1, 0] = nextChunk.GetBlockLocal(x, y, Chunk.Size - 1);
            }
            else
            {
                for (int y = 0; y < Chunk.Size; y++)
                    for (int x = 0; x < Chunk.Size; x++)
                        order.grid[x + 1, y + 1, 0] = 0;
            }

            readyToRender.Enqueue(order);
        }

    }

    void ProcessRenderResults()
    {
        MeshingResult result;
        int renderedChunks = 0;
        while(renderedChunks < MaxChunksRenderedPerFrame && renderResults.TryDequeue(out result))
        {
            renderedChunks++;
            MeshData meshData;
            if (Rendered.TryGetValue(result.position, out meshData))
            {
                if(meshData == null)
                {
                    if (result.meshData.vertices.Count == 0)
                        continue;

                    GameObject G = renderedPool.GetFromPool();
                    G.transform.parent = transform;
                    G.transform.position = result.position * Chunk.Size;
                    meshData = G.GetComponent<MeshData>();
                    meshData.Assign(result.meshData);
                    Rendered[result.position] = meshData;
                }
                else
                    meshData.Assign(result.meshData);
            }
            else
            {
                if (result.meshData.vertices.Count == 0)
                    Rendered[result.position] = null;
                else
                {
                    meshData = CreateChunkObject(result.position);
                    meshData.Assign(result.meshData);
        //            GameObject G = renderedPool.GetFromPool();
         //           G.transform.parent = transform;
         //           G.transform.position = result.position * Chunk.Size;
          //          meshData = G.GetComponent<MeshData>();
          //          meshData.Assign(result.meshData);
          //          Rendered[result.position] = meshData;
                }
            }
        }
    }

    void Awake()
    {
        renderedPool = new ObjectPool(10, RenderChunkPrefab);
        for (int i = 0; i < PoolSize; i++)
            ordersPool.Add(new ChunkRenderOrder());
    }

    void Start()
    {
        meshingThread = new MeshThread(ordersPool, readyToRender, renderResults);
    }

    void Update()
    {
        PrepareRenderOrders();

        ProcessRenderResults();
    }

    private void OnDestroy()
    {
        meshingThread.Close();
    }

    public void RefreshChunk(Vector3Int chunkPos)
    {
        if (!ToRender.Contains(chunkPos))
        {
            float d = ((Vector3)chunkPos*32 - player.position).sqrMagnitude;
            ToRender.Enqueue(d, chunkPos);
        }
        //    ToRender.Add(chunkPos);
    }

    public void RenderChunk(Vector3Int chunkPos)
    {
        if (Rendered.ContainsKey(chunkPos))
            return;
        if (!ToRender.Contains(chunkPos))
        {
            float d = ((Vector3)chunkPos*Chunk.Size - player.position).sqrMagnitude;
            ToRender.Enqueue(d, chunkPos);
        }
    }

    MeshData CreateChunkObject(Vector3Int chunkPos)
    {
        GameObject G = renderedPool.GetFromPool();
        G.transform.parent = transform;
        G.transform.position = chunkPos * Chunk.Size;
        MeshData meshData = G.GetComponent<MeshData>();
        Rendered[chunkPos] = meshData;
        return meshData;
    }

    void RenderEmptyChunk(Vector3Int chunkPos)
    {
        MeshData data;
        Rendered.TryGetValue(chunkPos, out data);
        if (data != null)
            data.Clear();
        else
        {
            Rendered[chunkPos] = null;
        }
    }

    public bool IsRendered(Vector3Int chunkCord)
    {
        return Rendered.ContainsKey(chunkCord);
    }

}
