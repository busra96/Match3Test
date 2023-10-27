using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3Test
{
    public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private GameObject _objectPoolEmptyHolder;
    
    private static GameObject _tileObjectsEmpty;
    private static GameObject _dropItemObjectsEmpty;

    public enum PoolType
    {
        TileObj,
        DropItemObj,
        None
    }

    public static PoolType poolingType;

    private void Awake()
    {
        SetupEmties();
    }

    private void SetupEmties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");
        
        _tileObjectsEmpty = new GameObject("Pooled Tile Objects");
        _tileObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        
        _dropItemObjectsEmpty = new GameObject("Pooled Drop Item Objects");
        _dropItemObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        
    }

    public static GameObject SpawnObject(GameObject objectToSpawn,PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        //if  the pool doesn't exist, create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }
        
        //check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            
            //Find the parent of the empty object
            GameObject parentObject = SetParetObject(poolType);
            
            //if there are no inactivate objects, create a new one
            spawnableObj = Instantiate(objectToSpawn);

            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
             //if there is an inactive object, reactive it
             pool.InactiveObjects.Remove(spawnableObj);
             spawnableObj.SetActive(true);

        }

        return spawnableObj;
    }


    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.None)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); // by taking off 7, we are removing the (Clone) from the name of the passed in obj

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled " + obj.name);
        }

        else
        {
            //Find the parent of the empty object
            GameObject parentObject = SetParetObject(poolType);
            if (parentObject != null)
            {
                obj.transform.SetParent(parentObject.transform);
            }
            
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }


    private static GameObject SetParetObject(PoolType poolType)
    {
        switch (poolType)
        {
            case  PoolType.TileObj:
                return _tileObjectsEmpty;
            
            case  PoolType.DropItemObj:
                return _dropItemObjectsEmpty;
            
            case  PoolType.None:
                return null;
            
            default:
                return null;
        }
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}

}
