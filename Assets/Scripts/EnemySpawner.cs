using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyType1;
    public float spawnRate;
    public float spawnRange;
    public int spawnNumber;

    public int maxEnemies;
    int enemiesCount = 0;

    public Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnRate);
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            float x = (int)(Random.value * 2 * spawnRange + 1) - spawnRange;
            float z = (int)(Random.value * 2 * spawnRange + 1) - spawnRange;
            if (enemiesCount < maxEnemies)
            {
                Instantiate(enemyType1, new Vector3(player.position.x + x, player.position.y, player.position.z + z), Quaternion.identity);
                enemiesCount++;
            }
        }
    }
}