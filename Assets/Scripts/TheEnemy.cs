using UnityEngine;

public class TheEnemy : MonoBehaviour
{
    Transform player;
    public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("FlyingPlayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
