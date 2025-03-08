using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Piece : MonoBehaviour
{
    private Board _board;
    
    public Vector3Int Position { get; private set; }

    private Vector2Int _translation;
    private int _rotationIndex;

    public Tetromino Tetromino { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    
    public float StepDelay = .3f;
    public float LockDelay = .5f;

    private void Start()
    {
        _board = GetComponent<Board>();
    }

    public void Spawn(Vector3Int position, Vector2Int translation,Tetromino tetromino)
    {
        Position = position;
        _translation = translation;
        Tetromino = tetromino;
        _rotationIndex = 0;

        Cells = new Vector3Int[Tetromino.Cells.Length];

        for (int i = Tetromino.Cells.Length -1; i >= 0; i--)
        {
            Cells[i] = (Vector3Int)Tetromino.Cells[i];
        }

        for (int i = 0; i < Random.Range(0,4); i++) //set random rotation
        {
            Rotate(1);
        }
        
        StartCoroutine(AutoMove());
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _board.ClearPiece(this);
            Move(Vector2Int.up, out _);
            _board.SetPiece(this);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _board.ClearPiece(this);
            Move(Vector2Int.down, out _);
            _board.SetPiece(this);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _board.ClearPiece(this);
            Move(Vector2Int.left, out _);
            _board.SetPiece(this);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _board.ClearPiece(this);
            Move(Vector2Int.right, out _);
            _board.SetPiece(this);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _board.ClearPiece(this);
            Rotate(-1);
            _board.SetPiece(this);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _board.ClearPiece(this);
            Rotate(1);
            _board.SetPiece(this);
        }
    }

    private bool Move(Vector2Int translation, out bool isOutOfBounds)
    {
        Vector3Int newPos = Position;
        newPos.x += translation.x;
        newPos.y += translation.y;
        
        bool isValid = _board.IsValidPosition(this, newPos, out isOutOfBounds);
        if (isValid) Position = newPos;
        return isValid;
    }

    private void Rotate(int direction)
    {
        _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);

        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cellToRotate = Cells[i];
            int x, y;

            switch (Tetromino.TetrominoType)
            {
                case TetrominoTypes.O: case TetrominoTypes.I: //These pieces need to be offset because of their nature.
                    cellToRotate.x -= .5f;
                    cellToRotate.y -= .5f;
                    x = Mathf.CeilToInt(cellToRotate.x * TetrominoData.RotationMatrix[0] * direction + cellToRotate.y * TetrominoData.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cellToRotate.x * TetrominoData.RotationMatrix[2] * direction + cellToRotate.y * TetrominoData.RotationMatrix[3] * direction);
                    break;
                default:
                    x = Mathf.RoundToInt(cellToRotate.x * TetrominoData.RotationMatrix[0] * direction + cellToRotate.y * TetrominoData.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cellToRotate.x * TetrominoData.RotationMatrix[2] * direction + cellToRotate.y * TetrominoData.RotationMatrix[3] * direction);
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private void Lock(bool isOutOfBounds)
    {
        if(!isOutOfBounds) _board.SetPiece(this);
        _board.SpawnRandomPiece();
    }

    public void HardDrop()
    {
        while (Move(Vector2Int.down, out _)) continue;
    }

    private int Wrap(int value, int min, int max)
    {
        if (value >= max) value = min;
        else if (value <= min) value = max;
        return value;
    }

    private IEnumerator AutoMove()
    {
        bool isOutOfBounds = false;
        
        while (true)
        {
            yield return new WaitForSeconds(StepDelay);
            _board.ClearPiece(this);
            var isValidPos = Move(_translation, out isOutOfBounds);
            if(!isValidPos || isOutOfBounds) break;
            _board.SetPiece(this);
        }
        
        Lock(isOutOfBounds);
    }
}
