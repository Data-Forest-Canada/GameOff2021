using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Given items in a specific order, will return individual elements and repeat once the list is exhausted.
public class Bucket<T>
{
    int index;
    List<T> bucketItems;

    public int Size => bucketItems.Count;

    public Bucket()
    {
        bucketItems = new List<T>();
        index = 0;
    }

    public Bucket(T[] items)
    {
        fillFromArray(items);
    }

    public Bucket(List<T> items)
    {
        fillFromArray(items.ToArray());
    }

    public void AddItem(T item) => bucketItems.Add(item);
    public bool RemoveItem(T item) => bucketItems.Remove(item);

    public T GetNextItem()
    {
        T item = bucketItems[index];

        if (item == null) Debug.Log("Bucket could not get item. Is it empty?");

        index++;
        if (index >= Size) index = 0;

        return item;
    }


    // Easy initalize from an array
    void fillFromArray(T[] items)
    {
        bucketItems = new List<T>();
        index = 0;

        foreach (T item in items)
        {
            if (item != null) bucketItems.Add(item);
        }
    }
}
