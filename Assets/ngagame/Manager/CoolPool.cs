using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPool : Singleton<CoolPool>
{
    Dictionary<string, List<GameObject>> pooledDict = new Dictionary<string, List<GameObject>>(1);
	
    public void Init<T>(string tag, T objectToPool, int size)
    {
		var key = objectToPool.GetHashCode();
        if (pooledDict.ContainsKey(tag)) {
            if(pooledDict[tag] == null)
            {
                pooledDict[tag] = new List<GameObject>(size);
            }
              else
            {
				pooledDict[tag].RemoveAll(item => item == null);
				for (int i = 0; i < pooledDict[tag].Count; i++)
                {
						pooledDict[tag][i].gameObject.SetActive(false);
                }
            }  
        } else
        {
            pooledDict[tag] = new List<GameObject>(size);
        }

        if (size <= pooledDict[tag].Count)
        {
            return;
        }
        for (int i = pooledDict[tag].Count; i < size; i++)
        {
            var ins = Instantiate(objectToPool as GameObject, transform);
            ins.transform.SetParent(transform);
            ins.gameObject.SetActive(false);
            pooledDict[tag].Add(ins);
        }
    }

    public T Spawn<T>(string tag) where T : Object
    {
        if(!pooledDict.ContainsKey(tag))
        {
            return null;
        }
        var pooledObjects = pooledDict[tag];
        if(pooledObjects == null)
        {
            return null;
        }

        // Find inactive game object
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].gameObject.SetActive(true);
                return pooledObjects[i] as T;
            }
        }

        // Create new
        var ins = Instantiate(pooledObjects[0], transform);
        ins.transform.SetParent(transform);
        ins.gameObject.SetActive(true);
        pooledDict[tag].Add(ins);
        return ins as T;
    }

    public void Despawn(string tag, GameObject objectToDespawn, float delay = 0)
    {
        if (objectToDespawn == null || !pooledDict.ContainsKey(tag))
        {
            return;
        }
        var pooledObjects = pooledDict[tag];
        if (pooledObjects == null)
        {
            return;
        }
		
		if (delay > 0)
        {
            StartCoroutine(IEDespawn(pooledObjects, objectToDespawn, delay));
        } else
        {
            objectToDespawn.gameObject.SetActive(false);
        }
        
    }

    IEnumerator IEDespawn(List<GameObject> pooledObjects, GameObject objectToDespawn, float delay)
    {
        yield return new WaitForSeconds(delay);
		if(objectToDespawn != null)
			objectToDespawn.gameObject.SetActive(false);
    }
}
