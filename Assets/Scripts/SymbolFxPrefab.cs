using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class SymbolFxPrefab : MonoBehaviour
{
    public ObjectPool<SymbolFxPrefab> _pool;
    [SerializeField] private float delay = 1f;
    public void Init(ObjectPool<SymbolFxPrefab> pool, Vector3 pos)
    {
        _pool = pool;
        gameObject.SetActive(true);
        transform.position = pos;
        DOVirtual.DelayedCall(delay, OnDelayComplete);  
    }

    public void FxStart()
    {
        
    }

    private void OnDelayComplete()
    {
        gameObject.SetActive(false);
        _pool.Release(this);
    }
}
