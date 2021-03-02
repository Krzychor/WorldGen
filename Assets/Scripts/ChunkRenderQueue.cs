using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChunkRenderQueue<T>
{

    class Node
    {
        public float Priority { get; set; }
        public T Object { get; set; }
    }
    
    List<Node> queue = new List<Node>();
    int heapSize = -1;
    public int Count { get { return queue.Count; } }



    public void Enqueue(float priority, T obj)
    {
        Node node = new Node() { Priority = priority, Object = obj };
        queue.Add(node);
        heapSize++;
        BuildHeapMin(heapSize);
    }

    public T Dequeue()
    {
        if (heapSize > -1)
        {
            var returnVal = queue[0].Object;
            queue[0] = queue[heapSize];
            queue.RemoveAt(heapSize);
            heapSize--;
            //Maintaining lowest or highest at root based on min or max queue
            MinHeapify(0);

            return returnVal;
        }
        else
            throw new Exception("Queue is empty");
    }

    public void UpdatePriority(T obj, float priority)
    {
        int i = 0;
        for (; i <= heapSize; i++)
        {
            Node node = queue[i];
            if (object.ReferenceEquals(node.Object, obj))
            {
                node.Priority = priority;
                
                BuildHeapMin(i);
                MinHeapify(i);
            }
        }
    }

    public bool Contains(T obj)
    {
        foreach (Node node in queue)
            if (object.ReferenceEquals(node.Object, obj))
                return true;
        return false;
    }

    private void BuildHeapMin(int i)
    {
        while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

   
    private void MinHeapify(int i)
    {
        int left = ChildL(i);
        int right = ChildR(i);

        int lowest = i;

        if (left <= heapSize && queue[lowest].Priority > queue[left].Priority)
            lowest = left;
        if (right <= heapSize && queue[lowest].Priority > queue[right].Priority)
            lowest = right;

        if (lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }
  
    private void Swap(int i, int j)
    {
        var temp = queue[i];
        queue[i] = queue[j];
        queue[j] = temp;
    }
    private int ChildL(int i)
    {
        return i * 2 + 1;
    }
    private int ChildR(int i)
    {
        return i * 2 + 2;
    }
}
