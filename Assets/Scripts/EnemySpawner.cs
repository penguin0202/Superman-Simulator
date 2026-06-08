using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyType1;
    public float chance1;
    public GameObject enemyType2;
    public float chance2;
    public float spawnRate;
    public float spawnRange;

    public Transform player;

    public float timeBetweenSpawns;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnRate);
    }

    void SpawnEnemy()
    {
        float x = (int)(Random.value * 2 * spawnRange + 1) - spawnRange;
        float z = (int)(Random.value * 2 * spawnRange + 1) - spawnRange;

        Instantiate(enemyType1, new Vector3(player.position.x + x, player.position.y + spawnRange / 32, player.position.z + z), Quaternion.Euler(-31.5f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
