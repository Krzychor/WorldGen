using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    Stack<GameObject> stack = new Stack<GameObject>();
    GameObject ObjectPrefab;

    public ObjectPool(int initialSize, GameObject ObjectPrefab)
    {
        this.ObjectPrefab = ObjectPrefab;
        for(int x = 0; x < initialSize; x++)
            stack.Push(GameObject.Instantiate(ObjectPrefab));
    }

    public GameObject GetFromPool()
    {
        if(stack.Count == 0)
        {
            return GameObject.Instantiate(ObjectPrefab);
        }
        else
            return stack.Pop();
    }

    public void AddToPool(GameObject G)
    {
        G.SetActive(false);
        stack.Push(G);
    }


}
