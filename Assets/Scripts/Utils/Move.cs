using System.Collections;
using UnityEngine;

public static class Move
{
    public static IEnumerator MoveRoutine(Transform obj, Vector3 startPos, Vector3 destination, float timeToMove, AnimationCurve animationCurve,float delay = 0)
    {
        if(delay > 0) { yield return new WaitForSeconds(delay); }

        float elapsedTime = 0f;

        float time = timeToMove;

        float t = 0;
        while (t < 1)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            t = animationCurve.Evaluate(t);

            obj.position = Vector3.Lerp(startPos, destination, t);
            yield return null;
        }

        // move the game piece
        obj.position = destination;

        // wait until next frame
        yield return null;
    }
}
