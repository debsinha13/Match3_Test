using System;
using System.Collections;
using UnityEngine;

public class MoveCoroutines: MonoBehaviour
{
    [SerializeField] private Animation _animation;
    private Coroutine moveRoutine;
    [SerializeField] private GameObject _spriteObj;
    public void StartMove(Vector3 startPos, Vector3 destPos, float timeToMove, AnimationCurve curve, bool toggleSpriteObj, float delay = 0,  Action callback = null)
    { 
        if(moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine(startPos, destPos, timeToMove, curve, toggleSpriteObj, delay, callback));
    }
    public IEnumerator MoveRoutine(Vector3 startPos, Vector3 destPos, float timeToMove, AnimationCurve animationCurve, bool toggleSpriteObj, float delay = 0,  Action callback = null)
    {
        Debug.Log("Move Coroutine" + timeToMove + " " + toggleSpriteObj);
        transform.position = startPos;
        if (toggleSpriteObj) _spriteObj.SetActive(false);
        if(delay > 0) yield return new WaitForSeconds(delay); 
        _spriteObj.SetActive(true);
        _animation.Play("AN_Cascading");
        float elapsedTime = 0f;

        float time = timeToMove;

        float t = 0;
        while (t < 1)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            t = animationCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, destPos, t);
            yield return null;
        }
        _animation.Play("AN_CascadeDone");
        // move the game piece
        transform.position = destPos;

        // wait until next frame
        yield return null;
        callback?.Invoke();

    }
}
