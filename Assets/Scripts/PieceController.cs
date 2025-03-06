using UnityEngine;

public class PieceController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool move = true;
    [SerializeField] private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!move) return;
        rb.linearVelocity = transform.TransformDirection(new Vector2(0, -1) * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        move = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GameManager.instance.AddPieceToTilemap(gameObject);
    }
}
