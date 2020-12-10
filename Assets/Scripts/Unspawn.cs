using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unspawn : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(10f);
        transform.parent = null;
        GameObjectPool.Unspawn(gameObject);
    }
}
