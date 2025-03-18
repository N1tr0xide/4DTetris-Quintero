using System;
using UnityEngine;

[CreateAssetMenu]
public class BoardSpawnPoint : ScriptableObject
{
    public Vector2Int SpawnPoint;
    
    public enum TranslationDirections
    {
        Up,
        Down,
        Left,
        Right
    }
    [Tooltip("Direction at which the piece 'falls'. should point towards the center of the board.")]
    public TranslationDirections GravityDirection;
    
    /// Direction at which the piece "falls". 
    public Vector2Int Translation
    {
        get
        {
            Vector2Int vector2Int = GravityDirection switch
            {
                TranslationDirections.Up => Vector2Int.up,
                TranslationDirections.Down => Vector2Int.down,
                TranslationDirections.Left => Vector2Int.left,
                TranslationDirections.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };

            return vector2Int;
        }
    }
}


