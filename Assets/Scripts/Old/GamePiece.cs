using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

    [SerializeField] private SO_AnimationAttributes _cascadeAttributes;
    [SerializeField] private GameObject _spriteObj;
	private int _xIndex;
    private int _yIndex;
    public int xIndex { get => _xIndex; }
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

    public void MovePiece(Vector3 startPos,int destX, int destY, int currentSymbolCascadeCount, bool isCascading)
    {
		if (m_isMoving) return;
		if (isCascading)
		{
			_onMoveCompleteX = destX;
			_onMoveCompleteY = destY;
			m_isMoving=true;
            StartCoroutine(MoveRoutine(startPos, new Vector3(destX, destY, 0), RemapCascadeTime((float)destY), _cascadeAttributes.Delay() * currentSymbolCascadeCount ));
            //StartCoroutine(Move.MoveRoutine(transform, startPos, new Vector3(destX, destY, 0), _cascadingTime, _cascadingMoveCurve, _cascadeDelay, () => OnMoveCompletion()));
        }
		else 
		{
            _onMoveCompleteX = destX;
            _onMoveCompleteY = destY;
            m_isMoving = true;
			StartCoroutine(MoveRoutine(startPos,new Vector3(destX, destY, 0), RemapCascadeTime((float)destY), 0f));
            //StartCoroutine(Move.MoveRoutine(transform, startPos, new Vector3(destX, destY, 0), _cascadingTime, _cascadingMoveCurve, 0f, () => OnMoveCompletion()));
        }
    }

    private float RemapCascadeTime(float pos)
    {
        return _cascadeAttributes.Time() / pos;
    }

	private int _onMoveCompleteX;
	private int _onMoveCompleteY;
	private void OnMoveCompletion()
	{
        if (_board != null)
        {
            _board.PlaceGamePiece(this, _onMoveCompleteX, _onMoveCompleteY);
        }
    }

    public void MovePiece (int destX, int destY, float timeToMove, float delay = 0f)
	{

		if (!m_isMoving)
		{
             StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove, delay));
        }
	}

    public void MoveOnCollapseColumn(int destX, int destY, float timeToMove, float delay = 0f)
    {

        if (!m_isMoving)
        {
            if (delay > 0f) StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), _cascadeAttributes.CollapseTime() * (yIndex - destY), delay * _cascadeAttributes.GetDelayCollapseColumn()));
            else StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove, delay));

        }
    }

    IEnumerator MoveRoutine(Vector3 startPos, Vector3 destination, float timeToMove, float delay = 0)
    {
        Debug.Log("Move with startpos routine");
        transform.position = startPos;
        _spriteObj.SetActive(false);
        if (delay > 0) { yield return new WaitForSeconds(delay); }
        _spriteObj.SetActive(true);
        float elapsedTime = 0f;

        m_isMoving = true;

        float time = timeToMove;

        float t = 0;
        while (t < 1)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            t = _cascadeAttributes.Curve().Evaluate(t);

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


    IEnumerator MoveRoutine(Vector3 destination, float timeToMove, float delay = 0)
	{
        Debug.Log("Move routine");
		if(delay > 0) { yield return new WaitForSeconds(delay); }
		Vector3 startPos = transform.position;

		float elapsedTime = 0f;

		m_isMoving = true;

        float time = timeToMove;

        float t = 0;
        while (t < 1)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            t = _cascadeAttributes.Curve().Evaluate(t);

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

    public void EnableSpriteObj(bool toggle)
    { 
        _spriteObj.SetActive(toggle);
    }
}
