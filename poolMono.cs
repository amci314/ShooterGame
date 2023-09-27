using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poolMono<T> where T : MonoBehaviour
{
    public T prefab { get; }
    public bool autoExpand { get; set; }
    public Transform contaiter { get; }

    List<T> pool;

    public poolMono(T prefab, int count)
    {
        this.prefab = prefab;
        this.contaiter = null;
        this.CreatePool(count);
    }

    public poolMono(T prefab, int count, Transform container)
    {
        this.prefab = prefab;
        this.contaiter = container;
        this.CreatePool(count);
    }

    public poolMono(T prefab, int count, Transform container, bool Auto)
    {
        this.prefab = prefab;
        this.contaiter = container;
        autoExpand = Auto;
        this.CreatePool(count);
    }

    public void CreatePool(int count)
    {
        this.pool = new List<T>();

        for (int i = 0; i < count; i++)
            this.CreateObject();
    }

    private T CreateObject(bool Active = false)
    {
        var createdObject = Object.Instantiate(this.prefab, this.contaiter);
        createdObject.gameObject.SetActive(Active);
        this.pool.Add(createdObject);
        return createdObject;
    }

    
    public bool HasFreeElemnt(out T element) 
    {
        foreach (var item in this.pool)
        {
            if (item.GetComponent<ParticleSystem>() != null)
            {
                if (!item.gameObject.GetComponent<ParticleSystem>().isPlaying|| !item.gameObject.activeInHierarchy)
                {
                    element = item;
                    element.gameObject.SetActive(true);
                    element.GetComponent<ParticleSystem>().Play();
                    return true;
                }
            }
            else
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    element = item;
                    element.gameObject.SetActive(true);
                    return true;
                }
            }
        }

        element = null;
        return false;
    }

    public T GetFreeElement()
    {
        if(this.HasFreeElemnt(out var element))
            return element;

        if(this.autoExpand)
            return this.CreateObject(true);

        throw new System.Exception("no free elemnts");
    }
}
