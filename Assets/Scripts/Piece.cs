using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Piece : MonoBehaviour
{
    private Board _board;
    private InputSystem_Actions _input;
    private Vector2Int _translation;
    private int _rotationIndex;
    private bool _autoMove, _gameOver;
    private float _stepDelay = 1f, _currentStepTime;

    [SerializeField] private bool _enablePlayerInput;

    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Tetromino Tetromino { get; private set; }

    public event Action OnGameOver;
    
    private void Start()
    {
        _board = GetComponent<Board>();
        
        if(!_enablePlayerInput) return;
        _input = new InputSystem_Actions();
        _input.Player.Enable();
        _input.Player.Move.performed += PerformMove;
        _input.Player.Rotate.performed += PerformRotate;
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    public void Spawn(Vector3Int position, Vector2Int translation, Tetromino tetromino)
    {
        Position = position;
        _translation = translation;
        Tetromino = tetromino;
        _rotationIndex = 0;
        _currentStepTime = 0;

        Cells = new Vector3Int[Tetromino.Cells.Length];

        for (int i = 0; i < Tetromino.Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)Tetromino.Cells[i];
        }

        for (int i = 0; i < Random.Range(0,4); i++) //set random rotation
        {
            Rotate(1);
        }

        if (!_board.IsValidOnTilemap(this, Position))
        {
            print("init lock: " + Position + " : " + Tetromino.TetrominoType);
            Lock(true);
        }
        
        _board.SetPiece(this);
        
        if(!_autoMove) StartCoroutine(AutoMove());
    }

    #region Input Actions

    private void PerformMove(InputAction.CallbackContext obj)
    {
        Vector2 v2 = obj.ReadValue<Vector2>();
        if(v2 is { x: 1, y: 1 } or { x: -1, y: -1 }) return;
        Vector2Int v2I = Vector2Int.RoundToInt(v2);

        if (v2I == -_translation) //Hard drop if input is inverse of falling direction.
        {
            HardDrop();
            return;
        }

        _board.ClearPiece(this);
        
        if (!Move(v2I, out bool outOfBounds))
        {
            Lock(outOfBounds);
            return;
        }
        
        _board.SetPiece(this);
    }
    
    private void PerformRotate(InputAction.CallbackContext obj)
    {
        int direction = Mathf.RoundToInt(obj.ReadValue<float>());
        if(direction is not (1 or -1)) return;
        
        _board.ClearPiece(this);
        Rotate(direction);
        _board.SetPiece(this);
    }

    #endregion
    
    private bool Move(Vector2Int translation, out bool isOutOfBounds)
    {
        Vector3Int newPos = Position;
        newPos.x += translation.x;
        newPos.y += translation.y;
        
        bool isValidTile = _board.IsValidOnTilemap(this, newPos);
        bool isValidOnBounds = _board.IsValidOnBounds(newPos);
        isOutOfBounds = !isValidOnBounds; //if piece is not on valid bounds it means it's out Of Bounds :)
        
        if (isValidTile && isValidOnBounds) Position = newPos;
        return isValidTile && isValidOnBounds;
    }

    private void Rotate(int direction)
    {
        int originalRotationIndex = _rotationIndex;
        _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);
        
        ApplyRotationMatrix(direction);
        if (WallKickCheck(_rotationIndex, direction)) return;
        
        //if wall kick, revert the changes
        _rotationIndex = originalRotationIndex;
        ApplyRotationMatrix(-direction);
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cellToRotate = Cells[i];
            int x, y;

            switch (Tetromino.TetrominoType)
            {
                case TetrominoTypes.O:
                case TetrominoTypes.I: //These pieces need to be offset because of their nature.
                    cellToRotate.x -= .5f;
                    cellToRotate.y -= .5f;
                    x = Mathf.CeilToInt(cellToRotate.x * TetrominoData.RotationMatrix[0] * direction + cellToRotate.y * TetrominoData.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cellToRotate.x * TetrominoData.RotationMatrix[2] * direction + cellToRotate.y * TetrominoData.RotationMatrix[3] * direction);
                    break;
                case TetrominoTypes.T: case TetrominoTypes.L: case TetrominoTypes.J: case TetrominoTypes.S: case TetrominoTypes.Z: default:
                    x = Mathf.RoundToInt(cellToRotate.x * TetrominoData.RotationMatrix[0] * direction + cellToRotate.y * TetrominoData.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cellToRotate.x * TetrominoData.RotationMatrix[2] * direction + cellToRotate.y * TetrominoData.RotationMatrix[3] * direction);
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool WallKickCheck(int rotationIndex, int direction)
    {
        int index = GetWallKickIndex(rotationIndex, direction);

        for (int i = 0; i < Tetromino.wallKicks.GetLength(1) -1; i++)
        {
            Vector2Int translation = Tetromino.wallKicks[index, i];
            if (Move(translation, out _)) return true;
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int direction)
    {
        int index = rotationIndex * 2;
        if (direction < 0) index--;
        return Wrap(index, 0, Tetromino.wallKicks.GetLength(0) -1);
    }
    
    /// Lock this piece in place. Spawn next piece.
    private void Lock(bool isOutOfBounds)
    {
        if (isOutOfBounds && _enablePlayerInput)
        {
            GameOver();
            return;
        }
            
        _board.SetPiece(this);
        _board.CheckClears();
        _board.SpawnRandomPiece();
    }

    /// keep moving the piece instantly until it hit the bounds or another piece.
    private void HardDrop()
    {
        bool outOfBounds = false;
        _board.ClearPiece(this);
        while (Move(_translation, out outOfBounds)) continue;
        _board.SetPiece(this);
        Lock(outOfBounds);
    }

    private static int Wrap(int value, int min, int max)
    {
        if (value >= max) value = min;
        else if (value <= min) value = max;
        return value;
    }
    
    /// Automatically move this piece by its translation every stepDelay
    private IEnumerator AutoMove()
    {
        _autoMove = true;

        while (!_gameOver)
        {
            while (_currentStepTime < _stepDelay)
            {
                yield return new WaitForEndOfFrame();
                _currentStepTime += Time.deltaTime;
            }

            _currentStepTime = 0;
            _board.ClearPiece(this);
            if (!Move(_translation, out bool isOutOfBounds))
            {
                Lock(isOutOfBounds);
                _board.SetPiece(this);
            }
            _board.SetPiece(this);
        }
    }
    
    /// Ends piece respawning and disables input
    private void GameOver()
    {
        if(_gameOver) return;
        
        print("GAME LOST!");
        _gameOver = true;
        OnGameOver?.Invoke();
        _input.Player.Disable();
    }
    
    /// Modifies the current piece step delay by the seconds value, step delay can not be lower than .3 seconds.
    public void IncreaseStepDelay(float seconds)
    {
        if (_stepDelay + seconds < .3f) return;
        _stepDelay += seconds;
        print("Step delay is now: " + _stepDelay);
    }
}