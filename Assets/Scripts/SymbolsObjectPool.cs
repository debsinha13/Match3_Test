
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SymbolsObjectPool : MonoBehaviour
{
    [SerializeField] private GamePiece _broomPrefab;
    [SerializeField] private GamePiece _crackerPrefab;
    [SerializeField] private GamePiece _heartPrefab;
    [SerializeField] private GamePiece _shieldPrefab;
    [SerializeField] private GamePiece _squarePrefab;

    public ObjectPool<GamePiece> _broomPrefabPool;
    public ObjectPool<GamePiece> _crackerPrefabPool;
    public ObjectPool<GamePiece> _heartPrefabPool;
    public ObjectPool<GamePiece> _shieldPrefabPool;
    public ObjectPool<GamePiece> _squarePrefabPool;

    [SerializeField] private int _defaultCapacity, _maxCapacity;
    [SerializeField] private Transform _symbolsOnActiveTransform;

    public List<ObjectPool<GamePiece>> symbolsPools;
    private void Awake()
    {
        InitPools(ref _broomPrefabPool, _broomPrefab);
        InitPools(ref _squarePrefabPool, _squarePrefab);
        InitPools(ref _shieldPrefabPool, _shieldPrefab);
        InitPools(ref _heartPrefabPool, _heartPrefab);
        InitPools(ref _crackerPrefabPool, _crackerPrefab);
        symbolsPools = new List<ObjectPool<GamePiece>>();
        symbolsPools.Add(_squarePrefabPool);
        symbolsPools.Add(_shieldPrefabPool);
        symbolsPools.Add(_heartPrefabPool);
        symbolsPools.Add(_crackerPrefabPool);
        symbolsPools.Add(_broomPrefabPool);
    }

    private void InitPools(ref ObjectPool<GamePiece> objectPool, GamePiece pieceFrefab)
    {
        objectPool = new ObjectPool<GamePiece>(
            () =>
            {
                return Instantiate(pieceFrefab, this.transform);
            },
            GamePiece =>
            {
                GamePiece.gameObject.SetActive(true);
            },
            GamePiece =>
            {
                GamePiece.gameObject.SetActive(false);
            },
            GamePiece =>
            {
                Destroy(GamePiece.gameObject);
            },
            false, _defaultCapacity, _maxCapacity
            );
    }

    public void ReleasePoolPrefab(MatchType type, GamePiece piece)
    { 
        switch (type)
        {
            case MatchType.Square:
                //piece.transform.parent = transform;
                _squarePrefabPool.Release(piece);
                break;
            case MatchType.Heart:
                //piece.transform.parent = transform;
                _heartPrefabPool.Release(piece);
                break;
            case MatchType.Shield:
                //piece.transform.parent = transform;
                _shieldPrefabPool.Release(piece);
                break;
            case MatchType.Broom:
                //piece.transform.parent = transform;
                _broomPrefabPool.Release(piece);
                break;
            case MatchType.Cracker:
                //piece.transform.parent = transform;
                _crackerPrefabPool.Release(piece);
                break;
        }
    }

}
