using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="AnimationAttributes",menuName = "Scriptables/AnimationAttributes")]
public class SO_AnimationAttributes : ScriptableObject
{
    [SerializeField] private float _time;
    [SerializeField] private float _delay;
    [SerializeField] private AnimationCurve _curve;

    public float Time() { return _time; }
    public float Delay() { return _delay; }
    public AnimationCurve Curve() { return _curve;}
}
