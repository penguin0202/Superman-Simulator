using UnityEngine;

public class TheEnemy : MonoBehaviour
{
    Transform player;
    public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("FlyingPlayer").transform;

        float value = Random.value;
        if (value < 0.15f) moveSpeed *= 3;
        else if (value < 0.5f) moveSpeed *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        Vector3 direction = player.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion tiltRotation = Quaternion.Euler(67, 0, 0);

        transform.rotation = lookRotation * tiltRotation;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) Die();
    }
}
