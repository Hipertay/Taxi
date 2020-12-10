﻿using UnityEngine;
using System;
using System.Collections;

public class GameObjectPool : MonoBehaviour
{
	static GameObjectPool Pool;
	public ObjectCache[] caches;

	public Hashtable activeCachedObjects;

    [Serializable]

	public class ObjectCache
	{
        [SerializeField]
		public GameObject prefab;       
		[SerializeField]
		public int cacheSize = 10;

		private int cacheIndex = 0;
		private GameObject[] objects;

		[HideInInspector]
		public void Initialize()
		{
			objects = new GameObject[cacheSize];
			for (var i = 0; i < cacheSize; i++)
			{
				objects[i] = MonoBehaviour.Instantiate (prefab) as GameObject;
				objects[i].SetActive (false);
				objects[i].name = objects[i].name + i;
			}
		}

		public GameObject GetNextObjectInCache()
		{
			GameObject obj = null;
			for (var i = 0; i < cacheSize; i++) 
			{
				obj = objects[cacheIndex];
                if (!obj.activeSelf)
                {
                    break;
                }
				// If not, increment index and make it loop around
				// if it exceeds the size of the cache
				cacheIndex = (cacheIndex + 1) % cacheSize;
			}
			if (obj.activeSelf) 
			{
				Debug.LogWarning (
					"Spawn of " + prefab.name +
					" exceeds cache size of " + cacheSize +
					"! Reusing already active object.", obj);
				GameObjectPool.Unspawn(obj);
			}
			cacheIndex = (cacheIndex + 1) % cacheSize;
			return obj;
		}               
	}

	void Awake()
	{
        Pool = this;
		int amount = 0;
		for (var i = 0; i < caches.Length; i++) 
		{
			caches[i].Initialize ();
			amount += caches[i].cacheSize;
		}
		activeCachedObjects = new Hashtable(amount);
	}

	public static GameObject Spawn ( GameObject prefab, Vector3 position, Quaternion rotation )
	{
		ObjectCache cache = null;
		if ( Pool != null )
		{
			for ( var i = 0; i < Pool.caches.Length; i++)
			{
				if ( Pool.caches[i].prefab == prefab )
				{
                    cache = Pool.caches[i];
				}
			}
		}
		if ( cache == null ) 
		{
			return GameObject.Instantiate ( prefab, position, rotation ) as GameObject;
		}
		GameObject obj = cache.GetNextObjectInCache();
		obj.transform.position = position;
		obj.transform.rotation = rotation;
		obj.SetActive(true);
		Pool.activeCachedObjects[obj] = true;
		return obj;
	}

	public static void Unspawn( GameObject objectToDestroy )
	{
		if ( Pool != null && Pool.activeCachedObjects.ContainsKey(objectToDestroy) ) 
		{
			//Debug.Log (Pool.activeCachedObjects.ContainsKey(objectToDestroy).ToString ());
			objectToDestroy.SetActive(false);
			Pool.activeCachedObjects[objectToDestroy] = false;
		}
		else
		{ 
			Unspawn ( objectToDestroy );
		}
	}
}