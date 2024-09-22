using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public enum MatchType
{
    Broom,
    Cracker,
    Heart,
    Shield,
    Square
}

[RequireComponent(typeof(MoveDotween))]
[RequireComponent (typeof(MoveCoroutines))]
public class GamePiece : MonoBehaviour {

    [SerializeField] private SO_AnimationAttributes _cascadeAttributes;
    [SerializeField] private MoveDotween _moveDotween;
    [SerializeField] private MoveCoroutines _moveCoroutines;
    [SerializeField] private GameObject _spriteObj;
	private int _xIndex;
    private int _yIndex;
    public int xIndex { get => _xIndex; }
	public int yIndex { get => _yIndex; }

	private Board _board;

	bool m_isMoving = false;

    public MatchType type;

    private int _onMoveCompleteX;
    private int _onMoveCompleteY;

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

    private float RemapCascadeTime(float starty,float dest)
    {
        return _cascadeAttributes.Time() * (starty - dest);
    }

    private float RemapDelayTime(int num, float delay)
    {
        if (num == 0) return 0;
        return delay * num;
    }
    public void MovePieceOnRefill(Vector3 startPos,int destX, int destY, int symbolsCascadingCount)
    {
		if (m_isMoving) return;
        Debug.Log("Refill " + destY);
        _onMoveCompleteX = destX;
        _onMoveCompleteY = destY;
        m_isMoving = true;
        _moveCoroutines.StartMove(startPos, new Vector3(destX, destY, 0f), RemapCascadeTime(startPos.y,destY), _cascadeAttributes.Curve(), true, RemapDelayTime(symbolsCascadingCount,_cascadeAttributes.RefillDelay()),
            SetPiecePosition);
    }


    public void MoveOnSwap (int destX, int destY, float delay = 0f)
	{
        if (m_isMoving) return;

        m_isMoving = true;
        _onMoveCompleteX = destX;
        _onMoveCompleteY = destY;
        _moveDotween.Move(transform.position, new Vector3(destX, destY, 0f), SetPiecePosition);
	}

    public void MoveOnCollapseColumn(int destX, int destY, int symbolsCascadingCount)
    {
        if (m_isMoving) return;
        _onMoveCompleteX = destX;
        _onMoveCompleteY = destY;
        _moveCoroutines.StartMove(transform.position, new Vector3(destX, destY, 0f), RemapCascadeTime(transform.position.y,destY), _cascadeAttributes.Curve(), false, RemapDelayTime(symbolsCascadingCount, _cascadeAttributes.CollapseDelay()),
            SetPiecePosition);
    }


    public void EnableSpriteObj(bool toggle)
    { 
        _spriteObj.SetActive(toggle);
    }
    public void SetPiecePosition(int x, int y)
    {
        transform.position = new Vector3(x, y, 0);
        transform.rotation = Quaternion.identity;
        m_isMoving = false;
    }
    public void SetPiecePosition()
    {
        transform.position = new Vector3(_onMoveCompleteX, _onMoveCompleteY, 0);
        transform.rotation = Quaternion.identity;
        m_isMoving = false;
    }
}
