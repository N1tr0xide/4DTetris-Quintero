using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoTypes
{
    I, O, T, L, J, S, Z
}

[System.Serializable]
public struct Tetromino
{
    public TetrominoTypes TetrominoType;
    public Tile Tile;
    public Vector2Int[] Cells { get; private set; }

    public void Init()
    {
        Cells = TetrominoData.Cells[TetrominoType];
    }
}

public static class TetrominoData
{
    private static readonly float Cos = Mathf.Cos(Mathf.PI / 2f);
    private static readonly float Sin = Mathf.Sin(Mathf.PI / 2f);
    
    /// Determines how each cell will rotate
    public static readonly float[] RotationMatrix = new float[] { Cos, Sin, -Sin, Cos };
    
    /// Determines how each cell is organized for each tetromino in order to form each shape.
    public static readonly Dictionary<TetrominoTypes, Vector2Int[]> Cells = new()
    {
        { TetrominoTypes.I, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 1, 1), new( 2, 1) } },
        { TetrominoTypes.J, new Vector2Int[] { new(-1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { TetrominoTypes.L, new Vector2Int[] { new( 1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { TetrominoTypes.O, new Vector2Int[] { new( 0, 1), new( 1, 1), new( 0, 0), new( 1, 0) } },
        { TetrominoTypes.S, new Vector2Int[] { new( 0, 1), new( 1, 1), new(-1, 0), new( 0, 0) } },
        { TetrominoTypes.T, new Vector2Int[] { new( 0, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { TetrominoTypes.Z, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 0, 0), new( 1, 0) } },
    };
}
