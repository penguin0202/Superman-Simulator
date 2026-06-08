using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;

    public int width = 256;
    public int height = 256;

    public float scale = 20f;

    private float value;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        depth = Random.Range(0, 50);
        scale = Random.Range(0, 50);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        value = Random.value;
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        if (x == 0 || x == width || y == 0 || y == height) return 0.0f;

        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        if (value < 0.25f)
        {
            return Mathf.PerlinNoise(xCoord, yCoord);
        }
        else if (value < 0.50f)
        {
            return Mathf.Abs(1 - Mathf.PerlinNoise(xCoord, yCoord));
        }
        else if (value < 0.75f)
        {
            return Mathf.PerlinNoise(xCoord, yCoord) * Mathf.PerlinNoise(xCoord, yCoord);
        }
        else
        {
            return 1.5f * Mathf.Sqrt(Mathf.Abs(Mathf.PerlinNoise(xCoord, yCoord)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
