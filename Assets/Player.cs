using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class Player : MonoBehaviour
{
    RenderModule renderMod;
    public bool inUnload;

    Vector3 LastPos;

    void Awake()
    {
        inUnload = true;
        GetComponent<Rigidbody>().isKinematic = true;
        renderMod = GetComponent<RenderModule>();
        Info.player = this;
        LastPos = transform.position;
        gameObject.SetActive(false);
    }

	void Update ()
    {
        Chunk Last = Info.world.GetChunk(LastPos);
        Chunk New = Info.world.GetChunk(transform.position);
        if(Last != New)
        {
            renderMod.MoveLoadChunks();
            
            if(isReady(New) == false)
            {
                Debug.Log("Block [" + New.x + "," + New.y + "," + New.z + "]");
            //    inUnload = true;
           //     GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        
        if (inUnload && isReady(New))
        {
            inUnload = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        LastPos = transform.position;
    }

    bool isReady(Chunk chunk)
    {
        if (Info.world.isGenerated(chunk))
            if (chunk == null || renderMod.isRendering(chunk.x, chunk.y, chunk.z) == false)
                return true;
        return false;
    }
}
