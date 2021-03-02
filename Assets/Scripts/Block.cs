

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block
{

    static uint IDMask = (2^8)-1;
    static public int GetId(uint block)
    {
        return (int)(block & IDMask); ; 
    }

    static public void SetId(ref uint block, int newId)
    {
        block = block & (~IDMask);
        block |= (uint)newId;
    }
}

