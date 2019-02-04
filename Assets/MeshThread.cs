using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class MeshThread : MonoBehaviour
{
    public static LogicMeshData res;
    public static long time;
    public RenderChunk RC;
    public bool _threadRunning;
    Thread _thread;
    public Chunk chunk;
 //   public LogicMeshData LogicMesh;


    private void Awake()
    {
        time = 0;
        res = null;
        RC = null;
    }

    void Update()
    {
  //     if(_thread != null)
      //      Debug.Log(_thread.ThreadState.ToString());
    }

    public void StartThread()
    {
        _threadRunning = true;
        _thread = new Thread(ThreadedWork);
        _thread.Start();
    }


    void ThreadedWork()
    {
        bool workDone = false;
        Stopwatch sw = Stopwatch.StartNew();

        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
       //     Debug.Log("work");
            Work();
            workDone = true;
            // Do Work...
        }
        sw.Stop();
        UnityEngine.Debug.Log(" processed in " + sw.ElapsedMilliseconds);
        time = sw.ElapsedMilliseconds;
        _threadRunning = false;
    }


    void Work()
    {
        LogicMeshData LM = new LogicMeshData();
        res = new LogicMeshData();

        Vector3 pos = new Vector3();
        //    Debug.Log(pos.ToString());
        Vector3 temp = new Vector3();
        Block block;
        Block nextBlock;
        Vector3 DirectionVector;
        //   Chunk chunk = Info.world.GetChunk(0, 0, 0);
        //   = RC.chunk;
        int Px, Py, Pz;
        for (int x = 0; x < 32; x++)
        {
            pos.x = x;
            for (int z = 0; z < 32; z++)
            {
                pos.z = z;
                for (int y = 0; y < 32; y++)
                {

                    pos.y = y;

                    block = null;
                    nextBlock = null;
                    block = chunk.GetBlockLocal(x, y, z);
                    Px = chunk.x * 32; Py = chunk.y * 32; Pz = chunk.z * 32;
                    if (block != null && !(block is Air))
                    {
                        for (Direction dir = Direction.North; (int)dir < 6; dir++)
                        {
                            DirectionVector = dir.getVector();
                            temp.x = pos.x + DirectionVector.x; temp.y = pos.y + DirectionVector.y; temp.z = pos.z + DirectionVector.z;
                            nextBlock = Info.world.GetBlock(Px + (int)temp.x, Py + (int)temp.y, Pz + (int)temp.z);

                            if (nextBlock is Air)
                                LM.AddQuad(pos, dir);
                        }
                    }

                }
            }
        }

      //  Debug.Log(" processed [" + chunk.x + "," + chunk.y+","+chunk.z+"]");
        res = LM;
        //  Debug.Log(LM.vertices.Count);
        //    RC.meshData.Assign(LogicMesh);
        //    RC.meshData.Render();
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }
}