using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderModule : MonoBehaviour /// GetFree wymaga przypisania otrzymanego obiektu
{
    public GameObject RenderChunk_Pref;
    public int RenderDistance = 2;
    public int RenderDistanceVertical = 1;

  //  RenderChunk[,,] RenderSpace;
    Transform RenderParent;
    List<RenderChunk> ToRender;
    List<RenderChunk> Rendered;
    List<RenderChunk> RenderPool;

    public MeshThread O1;

    void Awake()
    {
        Info.RenderM = this;
        Rendered = new List<RenderChunk>();
        RenderPool = new List<RenderChunk>();
        ToRender = new List<RenderChunk>();
        GameObject G = new GameObject
        { name = "RenderParent" };
        RenderParent = G.transform;


        for(int i = 0; i < (2*RenderDistance +1)* (2*RenderDistance +1)*(2*RenderDistanceVertical +1) + 5; i++)
        {
            G = Instantiate(RenderChunk_Pref);
            G.transform.SetParent(RenderParent, true);
            RenderPool.Add(G.GetComponent<RenderChunk>());
        }

     //   RenderSpace = new RenderChunk[RenderDistance * 2 + 1, RenderDistanceVertical * 2 + 1, RenderDistance * 2 + 1];
        for (int x = 0; x < RenderDistance * 2 + 1; x++)
        {
            for (int z = 0; z < RenderDistance * 2 + 1; z++)
            {
                for (int y = 0; y < RenderDistanceVertical * 2 + 1; y++)
                {
                    //RenderSpace[x, y, z] = null;
                }
            }
        }
    }

    void Start()
    {
        RefreshChunks();    
    }

    void Update()
    {
        ProcessRendering();
    }

    RenderChunk GetRenderChunk(int x, int y, int z)
    {
        for(int i = 0; i < Rendered.Count; i++)
        {
            if (Rendered[i].chunk.x == x && Rendered[i].chunk.y == y && Rendered[i].chunk.z == z)
                return Rendered[i];
        }
        return null;
    }

    RenderChunk GetFree()
    {
        RenderChunk RC;
        if(RenderPool.Count > 0)
        {
            RC = RenderPool[RenderPool.Count - 1];
            RenderPool.RemoveAt(RenderPool.Count - 1);
            return RC;
        }
        GameObject G = Instantiate(RenderChunk_Pref);
        G.transform.SetParent(RenderParent);
        return G.GetComponent<RenderChunk>();
    }

    public void RefreshChunks()
    {
        Debug.Log("refresh");
        int CurrChunkX = (int)transform.position.x / 32;
        int CurrChunkY = (int)transform.position.y / 32;
        int CurrChunkZ = (int)transform.position.z / 32;
        Chunk C;
        int X, Y, Z;
        RenderChunk RC;
        for (int x = 0; x < RenderDistance * 2 + 1; x++)
        {
            X = CurrChunkX - RenderDistance + x;
            for (int z = 0; z < RenderDistance * 2 + 1; z++)
            {
                Z = CurrChunkZ - RenderDistance + z;
                for (int y = 0; y < RenderDistanceVertical * 2 + 1; y++)
                {
                    Y = CurrChunkY - RenderDistanceVertical + y;

                    RC = GetRenderChunk(X, Y, Z);
                    if(RC == null)
                    {
                        RC = GetFree();
                        C = Info.world.GetChunk(X, Y, Z);
                        if(C == null)
                        {
                            Info.world.RequestChunk(X, Y, Z);
                            RenderPool.Add(RC);
                        }
                        else
                        {
                            RC.AssignChunk(C);
                            StartRender(RC);
                        }
                    }
                    else
                    {
                        // if RC.isDirty to trzeba wrzucic na kolejke do przerenderowania
                    }

                }
            }
        }
        SortRenderQueue();
    }

    void SortRenderQueue()
    {
     //   Debug.Log("sort");
        RenderChunk temp;
        Vector3 pos;
        Vector3 pos2;
        for(int j = 0; j < ToRender.Count-1; j++)
        for (int i = 0; i < ToRender.Count-1; i++)
        {
            pos = new Vector3(ToRender[i].chunk.x*32, ToRender[i].chunk.y*32, ToRender[i].chunk.z*32);
            pos = pos - Info.player.transform.position;
            pos2 = new Vector3(ToRender[i+1].chunk.x*32, ToRender[i+1].chunk.y*32, ToRender[i+1].chunk.z*32);
            pos2 = pos2 - Info.player.transform.position;
            if ( pos.magnitude > pos2.magnitude )
            {
                temp = ToRender[i];
                ToRender[i] = ToRender[i + 1];
                ToRender[i + 1] = temp;
            }
        }
    }

    void StartRender(RenderChunk RC)
    {
        if(!isRendering(RC.chunk.x, RC.chunk.y, RC.chunk.z))
            if(!ToRender.Contains(RC))
                ToRender.Add(RC);
    }

    public void StartRender(Chunk C)
    {
        StartRender(GetFree());
    }

    /*
    public bool isLoaded(int x, int y, int z)
    {
        for (int i = 0; i < Rendered.Count; i++)
            if (Rendered[i].chunk.x == x && Rendered[i].chunk.y == y && Rendered[i].chunk.z == z)
                return true;
        return false;
    }*/

    public bool isRendering(int x, int y, int z)
    {
        if(O1._threadRunning)
        if (O1.chunk.x == x && O1.chunk.y == y && O1.chunk.z == z)
            return true;
        for (int i = 0; i < ToRender.Count; i++)
            if (ToRender[i].chunk.x == x && ToRender[i].chunk.y == y && ToRender[i].chunk.z == z)
                return true;
        return false;
    }

    void ProcessRendering()
    {
        if (!O1._threadRunning && O1.RC != null)
        {
      //      Debug.Log("finished");
            O1.RC.AssignResults(MeshThread.res, O1.chunk);
            Rendered.Add(O1.RC);
            Debug.Log(O1.RC.gameObject.name);
            O1.RC.gameObject.name = "["+O1.RC.chunk.x+","+O1.RC.chunk.y+","+O1.RC.chunk.z+"] "+MeshThread.time;
            
            O1.RC = null;
        }
          
        if (ToRender.Count > 0)
        {
            if (!O1._threadRunning)
            {
                SortRenderQueue();
      //          Debug.Log("start");
        //        Debug.Log(ToRender[0].chunk.x + "," + ToRender[0].chunk.y + "," + ToRender[0].chunk.z);
                O1.RC = ToRender[0];
                O1.chunk = ToRender[0].chunk;
                ToRender.RemoveAt(0);
                O1.StartThread();
            }
        }
    }

    public void MoveLoadChunks()
    {
        RefreshChunks();
        /*
        int CurrchunkX = (int)transform.position.x / 32;
        int CurrchunkY = (int)transform.position.y / 32;
        int CurrchunkZ = (int)transform.position.z / 32;
        Chunk C;
        RenderChunk RC;
        int Px, Py, Pz;
        for (int x = 0; x < RenderDistance * 2 + 1; x++)
        {
            for (int z = 0; z < RenderDistance * 2 + 1; z++)
            {
                for (int y = 0; y < RenderDistanceVertical * 2 + 1; y++)
                {
                    if (RenderSpace[x, y, z].chunk == null || RenderSpace[x, y, z].chunk.x != CurrchunkX - RenderDistance + x || RenderSpace[x, y, z].chunk.y != CurrchunkY - RenderDistanceVertical + y || RenderSpace[x, y, z].chunk.z != CurrchunkZ - RenderDistance + z)
                    {
                        C = Info.world.GetChunk(CurrchunkX - RenderDistance + x, CurrchunkY - RenderDistanceVertical + y, CurrchunkZ - RenderDistance + z);
                        if (C != null)
                        {
                            RC = AssignedTo(C, RenderSpace[x, y, z], out Px, out Py, out Pz);
                            if (RC != null)
                            {
                                //          Debug.LogWarning("Swap");
                                RenderChunk RCT = RenderSpace[x, y, z];
                                RenderSpace[x, y, z] = RenderSpace[Px, Py, Pz];
                                RenderSpace[Px, Py, Pz] = RCT;
                            }
                            else
                            {
                                //    Debug.Log(x + ", " + y + "," + z);
                                RenderSpace[x, y, z].AssignChunk(C);
                                StartRender(RenderSpace[x, y, z]);
                                ToRender.Add(RenderSpace[x, y, z]);
                            }
                        }
                    }
                }
            }
        }
        */
    }

}
