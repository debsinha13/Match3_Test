using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	[SerializeField] private AnimationCurve _cascadingMoveCurve;
	public AnimationCurve GetCascadingAnimationCurve()
	{ 
		return _cascadingMoveCurve;
	}
	private int _xIndex;
	public int xIndex { get => _xIndex; }
    private int _yIndex;
	public int yIndex { get => _yIndex; }

	private Board _board;

	bool m_isMoving = false;

	public MatchValue matchValue;

	public enum MatchValue
	{
		Broom,
		Cracker,
		Heart,
		Shield,
		Square
	}

	

	public void Init(Board board)
	{
		_board = board;
	}

	public bool HasBoard()
	{ 
		return _board != null;
	}
	public void SetCoord(int x, int y)
	{
		_xIndex = x;
		_yIndex = y;
	}

	public void Move (int destX, int destY, float timeToMove)
	{

		if (!m_isMoving)
		{
			StartCoroutine(MoveRoutine(new Vector3(destX, destY,0), timeToMove));	
		}
	}


	IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
	{
		Vector3 startPos = transform.position;

		float elapsedTime = 0f;

		m_isMoving = true;

        float time = timeToMove;

        float t = 0;
        while (t < 1)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            t = _cascadingMoveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, destination, t);
            yield return null;
        }

        // move the game piece
        transform.position = destination;
        if (_board != null)
        {
            _board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
        }
        // wait until next frame
        yield return null;

		m_isMoving = false;

	}

}
