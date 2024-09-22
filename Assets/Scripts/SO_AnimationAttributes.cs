using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="AnimationAttributes",menuName = "Scriptables/AnimationAttributes")]
public class SO_AnimationAttributes : ScriptableObject
{
    [SerializeField] private float _time;
    [SerializeField] private float _collapseDelay;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _refillDelay = 0.3f;

    public float Time() { return _time; }
    public float CollapseDelay() { return _collapseDelay; }
    public AnimationCurve Curve() { return _curve;}

    public float RefillDelay() { return _refillDelay; }

}
