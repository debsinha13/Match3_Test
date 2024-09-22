
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class FXSymbolsPool : MonoBehaviour
{
    [SerializeField] private SymbolFxPrefab _broomFxPrefab;
    [SerializeField] private SymbolFxPrefab _crackerFxPrefab;
    [SerializeField] private SymbolFxPrefab _heartFxPrefab;
    [SerializeField] private SymbolFxPrefab _shieldFxPrefab;
    [SerializeField] private SymbolFxPrefab _squareFxPrefab;

    public ObjectPool<SymbolFxPrefab> _broomPrefabFxPool;
    public ObjectPool<SymbolFxPrefab> _crackerPrefabFxPool;
    public ObjectPool<SymbolFxPrefab> _heartPrefabFxPool;
    public ObjectPool<SymbolFxPrefab> _shieldPrefabFxPool;
    public ObjectPool<SymbolFxPrefab> _squarePrefabFxPool;

    [SerializeField] private int _defaultCapacity, _maxCapacity;

    private void Awake()
    {
        InitPools(ref _broomPrefabFxPool, _broomFxPrefab);
        InitPools(ref _squarePrefabFxPool, _squareFxPrefab);
        InitPools(ref _shieldPrefabFxPool, _shieldFxPrefab);
        InitPools(ref _heartPrefabFxPool, _heartFxPrefab);
        InitPools(ref _crackerPrefabFxPool, _crackerFxPrefab);

    }

    private void InitPools(ref ObjectPool<SymbolFxPrefab> objectPool, SymbolFxPrefab pieceFrefab)
    {
        objectPool = new ObjectPool<SymbolFxPrefab>(
            () =>
            {
                return Instantiate(pieceFrefab, this.transform);
            },
            SymbolFxPrefab =>
            {
                //SymbolFxPrefab.Init(objectPool);
            },
            GameObject =>
            {
                //GameObject.SetActive(false);
            },
            GameObject =>
            {
                Destroy(GameObject);
            },
            false, _defaultCapacity, _maxCapacity
            );
    }

    
}
