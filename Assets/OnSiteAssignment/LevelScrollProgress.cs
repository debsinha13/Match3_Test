using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class LevelScrollProgress : MonoBehaviour
{
    [SerializeField] private GameObject _levelTab;
    [SerializeField] private Transform _tabParent;
    [SerializeField] private RectTransform _progressBar;
    [SerializeField] private Transform _starPos;
    [SerializeField] private float _xDist = 262f;
    [SerializeField] private int _numTabs = 10;
    [SerializeField] private Vector3 _tabPos;
    [SerializeField] private ScrollRect _scrollRect;
    private List<Animator> _tabAnimators = new List<Animator>();

    private void Start()
    {
        lastXScrollPos = _scrollRect.horizontalNormalizedPosition;
        _tabPos = _levelTab.transform.position;
        _tabAnimators.Add(_levelTab.GetComponent<Animator>());
        for (int i = 0; i < _numTabs; i++)
        {
            GameObject obj = Instantiate(_levelTab, new Vector3(_tabPos.x + _xDist, _tabPos.y, _tabPos.z), Quaternion.identity, _tabParent);
            _tabAnimators.Add(obj.GetComponent<Animator>());
            _tabPos.x += _xDist;
        }
        //_scrollRect.onValueChanged.AddListener(OnScrollValueUpdate);
        move = Move.Idle;
    }

    private void Update()
    {
        if (lastXScrollPos - _scrollRect.horizontalNormalizedPosition > _minScrollMove)
        {
            lastXScrollPos = _scrollRect.horizontalNormalizedPosition;
            //Debug.Log(pos);
            if (move == Move.Left)
            {
                move = Move.Right;
                buffer = 0;
            }
            else
            {
                move = Move.Right;
                if (buffer > 0) return;
            }

            buffer = animationBuffer;

            foreach (Animator animator in _tabAnimators)
            {
                if (Random.Range(1, 5) > 3) animator.SetTrigger("SwayingLeft");
                else animator.SetTrigger("SwayingLeft-1");

            }
        }
        else if ( lastXScrollPos - _scrollRect.horizontalNormalizedPosition < -_minScrollMove)
        {
            lastXScrollPos = _scrollRect.horizontalNormalizedPosition;
            //Debug.Log(pos);
            if (move == Move.Right)
            {
                move = Move.Left;
                buffer = 0;
            }
            else
            {
                move = Move.Left;
                if (buffer > 0) return;
            }

            buffer = animationBuffer;

            foreach (Animator animator in _tabAnimators)
            {
                if (Random.Range(1, 5) > 3) animator.SetTrigger("SwayingRight");
                else animator.SetTrigger("SwayingRight-1");
            }

        }
        if (buffer > 0)
        {
            buffer -= Time.deltaTime;
        }
    }
    [SerializeField] private float lastXScrollPos;
    public enum Move { Left,Right,Idle};
    public Move move;
    [SerializeField] private float _minScrollMove = 0.05f;
    [SerializeField] private float animationBuffer = 1.1f;
    [SerializeField] private float buffer = 0;


    public void IncrementProgressBar()
    {
        Vector2 i = new Vector2(_progressBar.sizeDelta.x + _xDist, _progressBar.sizeDelta.y);
        _progressBar.sizeDelta = i;
        _starPos.position = new Vector3(_starPos.position.x + _xDist, _starPos.position.y, _starPos.position.z);
    }
}
