using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject _tilemap;

    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this;
            return;
        }
        
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPieceToTilemap(GameObject piece)
    {
        piece.transform.parent = _tilemap.transform;
    }
}
