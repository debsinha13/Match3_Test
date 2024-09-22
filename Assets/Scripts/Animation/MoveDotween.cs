using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDotween : MonoBehaviour
{
    [SerializeField] private SO_AnimationAttributes _swapAnimationAttributes;
    public void Move(Vector3 startPos, Vector3 destPos, Action callback = null)
    {
        Debug.Log("Move Dotween");
        transform.position = startPos;
        transform.DOMove(destPos, _swapAnimationAttributes.Time()).SetEase(_swapAnimationAttributes.Curve()).OnComplete(()=>callback?.Invoke());
    }
}
