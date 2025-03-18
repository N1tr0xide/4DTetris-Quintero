using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Piece))]
public class Board : MonoBehaviour
{
    private Piece _playerPiece;
    private int _clearBoxSize = 3; //width and height of clear box in order to gain score.
    private RectInt Bounds 
    {
        get
        {
            Vector2Int position = new Vector2Int(-_boardSize.x, -_boardSize.y);
            return new RectInt(position, _boardSize * 2);
        }
    }
    
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Vector2Int _boardSize = new(18, 18);
    [SerializeField] private Tetromino[] _tetrominoes;
    [SerializeField] private BoardSpawnPoint[] _spawnPoints;

    public event Action OnClear;
    
    private void Awake()
    {
        _playerPiece = GetComponent<Piece>();
        
        for (int i = 0; i < _tetrominoes.Length; i++)
        {
            _tetrominoes[i].Init();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnRandomPiece();
    }

    public void SpawnRandomPiece()
    {
        Tetromino tetromino = _tetrominoes[Random.Range(0, _tetrominoes.Length)];
        BoardSpawnPoint randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        _playerPiece.Spawn((Vector3Int)randomSpawnPoint.SpawnPoint, randomSpawnPoint.Translation, tetromino);
        SetPiece(_playerPiece);
    }
    
    /// Sets the position of the piece
    public void SetPiece(Piece piece)
    {
        foreach (Vector3Int cell in piece.Cells)
        {
            Vector3Int tilePosition = cell + piece.Position; //Offset each cell by the pieces position.
            _tilemap.SetTile(tilePosition, piece.Tetromino.Tile);
        }
    }
    
    /// Clears the tiles from the position of the piece
    public void ClearPiece(Piece piece)
    {
        foreach (Vector3Int cell in piece.Cells)
        {
            //Offset each cell by the pieces position.
            Vector3Int tilePosition = cell + piece.Position;
            _tilemap.SetTile(tilePosition, null);
        }
    }
    
    /// Checks if a position is valid for the piece in question
    /// <param name="piece">Piece</param>
    /// <param name="position">Position to check for</param>
    /// <param name="isOutOfBounds">if the piece is out of the bounds of the board</param>
    public bool IsValidPosition(Piece piece, Vector3Int position, out bool isOutOfBounds)
    { 
        RectInt bounds = Bounds;
        isOutOfBounds = false;
        
        foreach (Vector3Int cell in piece.Cells)
        {
            Vector3Int tilePosition = cell + position;
            
            if (!bounds.Contains((Vector2Int)position))
            {
                ClearPiece(piece);
                isOutOfBounds = true;
                return false;
            }
            if (_tilemap.HasTile(tilePosition)) return false;
        } 
        
        return true; 
    }

    public void CheckClears()
    {
        RectInt bounds = Bounds;
        
        for (int r = bounds.yMin; r < bounds.yMax; r++)
        {
            for (int c = bounds.xMin; c < bounds.xMax; c++)
            {
                if (IsFullBox(r, c)) ClearBox(r,c);
            }
        }
    }
    
    /// Checks if every cell in a box of the clearBoxSize has a tile.
    /// <param name="row">the x position of the box</param>
    /// <param name="column">the y position of the box</param>
    private bool IsFullBox(int row, int column)
    {
        RectInt bounds = new RectInt(new Vector2Int(row, column), new Vector2Int(_clearBoxSize, _clearBoxSize));

        for (int r = bounds.yMin; r < bounds.yMax; r++)
        {
            for (int c = bounds.xMin; c < bounds.xMax; c++)
            {
                if (c == 0 && r == 0) return false; //center tile does not count towards clear.
                Vector3Int position = new Vector3Int(c, r, 0);
                if (!_tilemap.HasTile(position)) return false;
            }
        }

        return true;
    }
    
    private void ClearBox(int row, int column)
    {
        OnClear?.Invoke();
        
        RectInt bounds = new RectInt(new Vector2Int(row, column), new Vector2Int(_clearBoxSize, _clearBoxSize));

        for (int r = bounds.yMin; r < bounds.yMax; r++)
        {
            for (int c = bounds.xMin; c < bounds.xMax; c++)
            {
                Vector3Int position = new Vector3Int(c, r, 0);
                _tilemap.SetTile(position, null);
            }
        }
    }
}