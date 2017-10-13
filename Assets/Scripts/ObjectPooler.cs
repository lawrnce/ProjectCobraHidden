using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
  public GameObject objectToPool;
  public int amountToPool;
  public bool shouldExpand = true;
}

public class ObjectPooler : MonoBehaviour
{
  public static ObjectPooler Instance;
  public List<GameObject> pooledObjects;
  public List<ObjectPoolItem> itemsToPool;

  void Awake()
  {
    if (Instance == null)
    {
        Instance = this;
    }
    else if(Instance != this)
    {
        Destroy (gameObject);
    }

    pooledObjects = new List<GameObject>();
    foreach (ObjectPoolItem item in itemsToPool)
    {
        for (int i = 0; i < item.amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(item.objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
  }

  void Start ()
  {
    /* pooledObjects = new List<GameObject>(); */
    /* foreach (ObjectPoolItem item in itemsToPool) */
    /* { */
    /*     for (int i = 0; i < item.amountToPool; i++) */
    /*     { */
    /*         GameObject obj = (GameObject)Instantiate(item.objectToPool); */
    /*         obj.SetActive(false); */
    /*         pooledObjects.Add(obj); */
    /*     } */
    /* } */
  }

  public GameObject GetPooledObject(string tag)
  {
    for (int i = 0; i < pooledObjects.Count; i++)
    {
        if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
        {
            return pooledObjects[i];
        }
    }

    foreach (ObjectPoolItem item in itemsToPool)
    {
        if (item.objectToPool.tag == tag)
        {
            if (item.shouldExpand)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                return obj;
            }
        }
    }
    return null;
  }
}

