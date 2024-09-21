using UnityEngine;

public class Tile : MonoBehaviour {

	[SerializeField] private Animation _tileAnimation;

	private int _xIndex;
	private int _yIndex;

	public int XIndex { get => _xIndex; }
	public int YIndex { get => _yIndex;}

    [SerializeField] Board m_board;


	public void Init(int x, int y, Board board)
	{
        _xIndex = x;
		_yIndex = y;
		m_board = board;

	}

	void OnMouseDown()
	{
        if (m_board !=null)
		{
			m_board.ClickTile(this);
		}

	}

	void OnMouseEnter()
	{
		if (m_board !=null)
		{
			m_board.DragToTile(this);
		}

	}

	void OnMouseUp()
	{
		if (m_board !=null)
		{
			m_board.ReleaseTile();
		}

	}

	public void DoHightlightAnimation()
	{ 
		_tileAnimation.Play();
	}
}
