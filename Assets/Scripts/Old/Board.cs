using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{

    [SerializeField] private float _tileZPosition = 1;
    [SerializeField] private Transform _spawnPositionParent;

    public int width;
    public int height;

    public float borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

    [SerializeField] private Cascading[] _symbolCascading;

    public float swapTime = 0.5f;

    Tile[,] m_allTiles;
    [SerializeField] private GamePiece[,] _allGamePieces;

    Tile m_clickedTile;
    Tile m_targetTile;

    [SerializeField] bool m_playerInputEnabled = true;

    void Start()
    {
        m_allTiles = new Tile[width, height];
        _allGamePieces = new GamePiece[width, height];
        //_symbolCascading = new Cascading [width];

        ResetSymbolCascadingAttributes();
        SetupTiles();
        SetupCamera();
        SetUpSymbolSpawnPos();
        FillBoard();

    }

    private void ResetSymbolCascadingAttributes()
    {
        for (int i = 0; i < _symbolCascading.Length; i++)
        {
            _symbolCascading[i].symbolCascadeCount = 0;
            _symbolCascading[i].isCascading = false;
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;

        float verticalSize = (float)height / 2f + (float)borderSize;

        float horizontalSize = ((float)width / 2f + borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }

    // Setting up the tiles
    //TODO: Use object pooling instead of instantiating the Tiles
    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, _tileZPosition), Quaternion.identity) as GameObject;

                tile.name = "Tile (" + i + "," + j + ")";

                m_allTiles[i, j] = tile.GetComponent<Tile>();

                tile.transform.parent = transform;

                m_allTiles[i, j].Init(i, j, this);

            }
        }
    }

    [SerializeField] private GameObject[] _symbolSpawnPosition;
    private void SetUpSymbolSpawnPos()
    {
        _symbolSpawnPosition = new GameObject[width];
        for (int i = 0; i < width; i++)
        {
            _symbolSpawnPosition[i] = new GameObject();
            _symbolSpawnPosition[i].transform.parent = _spawnPositionParent;
            _symbolSpawnPosition[i].transform.position = new Vector3(i,height,0);
            _symbolSpawnPosition[i].name = "SwymbolSpawnPosition " + i;
        }
    }

    GameObject GetRandomGamePiece()
    {
        int randomIdx = Random.Range(0, gamePiecePrefabs.Length);

        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("BOARD:  " + randomIdx + "does not contain a valid GamePiece prefab!");
        }

        return gamePiecePrefabs[randomIdx];
    }
    /// <summary>
    /// Place Gamepiece, checks if it is within bounds
    /// </summary>
    /// <param name="gamePiece"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD:  Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            _allGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoord(x, y);
    }


    /// <summary>
    /// Check if the pieces is within bounds of width and height
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    #region Setting up the board on load

    /// <summary>
    /// Fill Gamepiece at a Tile, checking if it matches with nearby neighbors 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="falseYOffset"></param>
    /// <param name="moveTime"></param>
    /// <returns></returns>
    GamePiece FillRandomAt(int x, int y)
    {
        GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;

        if (randomPiece != null)
        {
            randomPiece.GetComponent<GamePiece>().Init(this);
            PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), x, y);

            randomPiece.transform.parent = transform;
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }

    void FillBoard()
    {
        int maxInterations = 100;
        int iterations = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (_allGamePieces[i, j] == null)
                {
                    GamePiece piece = FillRandomAt(i, j);
                    iterations = 0;

                    while (HasMatchOnFill(i, j))
                    {
                        ClearPieceAt(i, j);
                        piece = FillRandomAt(i, j);
                        iterations++;

                        if (iterations >= maxInterations)
                        {
                            Debug.Log("BOARD FillBoard: max iterations reached! =====================");
                            break;
                        }
                    }
                    piece.EnableSpriteObj(true);
                    PlaceGamePiece(piece, i, j);
                }
            }
        }
    }

    
    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        return (leftMatches.Count > 0 || downwardMatches.Count > 0);

    }

    #endregion

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (m_playerInputEnabled)
        {
            GamePiece clickedPiece = _allGamePieces[clickedTile.XIndex, clickedTile.YIndex];
            GamePiece targetPiece = _allGamePieces[targetTile.XIndex, targetTile.YIndex];

            if (targetPiece != null && clickedPiece != null)
            {
                clickedPiece.MovePiece(targetTile.XIndex, targetTile.YIndex, swapTime);
                targetPiece.MovePiece(clickedTile.XIndex, clickedTile.YIndex, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);

                if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0)
                {
                    clickedPiece.MovePiece(clickedTile.XIndex, clickedTile.YIndex, swapTime);
                    targetPiece.MovePiece(targetTile.XIndex, targetTile.YIndex, swapTime);
                }
                else
                {
                    yield return new WaitForSeconds(swapTime);

                    ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());

                }
            }
        }
    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.XIndex - end.XIndex) == 1 && start.YIndex == end.YIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.YIndex - end.YIndex) == 1 && start.XIndex == end.XIndex)
        {
            return true;
        }

        return false;
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = _allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = _allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
			
        return null;

    }

    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }
        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }

        return matches;
    }

    List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GamePiece> matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    void HighlightTileOn(int x, int y)
    {
         m_allTiles[x, y].DoHightlightAnimation();

    }

    void HighlightPieces(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex);
            }
        }
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = _allGamePieces[x, y];

        if (pieceToClear != null)
        {
            _allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

    }

    private void OnDisable()
    {
        ClearBoard();
    }
    void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < height - 1; i++)
        {
            if (_allGamePieces[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (_allGamePieces[column, j] != null)
                    {
                        if (_symbolCascading[column].isCascading)
                        {
                            _symbolCascading[column].symbolCascadeCount++;
                            _allGamePieces[column, j].MoveOnCollapseColumn(column, i, collapseTime * (j - i), _symbolCascading[column].symbolCascadeCount);
                        }
                        else 
                        {
                            _allGamePieces[column, j].MoveOnCollapseColumn(column, i, collapseTime * (j - i));
                            _symbolCascading[column].isCascading = true;
                            _symbolCascading[column].symbolCascadeCount++;
                        }

                        _allGamePieces[column, i] = _allGamePieces[column, j];
                        _allGamePieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(_allGamePieces[column, i]))
                        {
                            movingPieces.Add(_allGamePieces[column, i]);
                        }

                        _allGamePieces[column, j] = null;
                        break;

                    }
                }
            }
        }
        return movingPieces;

    }

    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;

    }

    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }

        return columns;

    }

    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {

        m_playerInputEnabled = false;
        List<GamePiece> matches = gamePieces;

        do
        {
            // clear and collapse
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            //refill
            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();

            yield return new WaitForSeconds(0.5f);
        }
        while (matches.Count != 0);

        m_playerInputEnabled = true;
    }

    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        HighlightPieces(gamePieces);
        yield return new WaitForSeconds(0.5f);
        bool isFinished = false;
        ResetSymbolCascadingAttributes();
        while (!isFinished)
        {
            ClearPieceAt(gamePieces);

            yield return new WaitForSeconds(0.25f);
            movingPieces = CollapseColumn(gamePieces);

            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            matches = FindMatchesAt(movingPieces);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    IEnumerator RefillRoutine()
    {
        ReFillBoard();
        yield return null;
    }

    void ReFillBoard()
    {
        int maxInterations = 100;
        int iterations = 0;

        ResetSymbolCascadingAttributes();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (_allGamePieces[i, j] == null)
                {
                    
                    GamePiece piece = FillRandomAt(i, j);
                    iterations = 0;

                    while (HasMatchOnFill(i, j))
                    {
                        ClearPieceAt(i, j);
                        piece = FillRandomAt(i, j);
                        iterations++;

                        if (iterations >= maxInterations)
                        {
                            Debug.Log("BOARD FillBoard: max iterations reached! =====================");
                            break;
                        }
                    }
                    if (_symbolCascading[i].isCascading)
                    {
                        _symbolCascading[i].symbolCascadeCount++;
                        piece.MovePiece(_symbolSpawnPosition[i].transform.position, i, j, _symbolCascading[i].symbolCascadeCount,true);
                        
                    }
                    else
                    {
                        piece.MovePiece(_symbolSpawnPosition[i].transform.position, i, j, _symbolCascading[i].symbolCascadeCount, false);;
                        _symbolCascading[i].isCascading = true;
                        _symbolCascading[i].symbolCascadeCount++;
                    }
                    
                }
            }
        }
    }

    bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {

            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

[System.Serializable]
public class Cascading
{
    public int symbolCascadeCount;
    public bool isCascading = false;
}
