using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int depth;

    public float scale = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        PaintTextures(terrainData);
        return terrainData;
    }

    void PaintTextures(TerrainData data)
    {
        int width = data.alphamapWidth;
        int height = data.alphamapHeight;

        float[,,] map = new float[width, height, 4]; // 4 terrain layers

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normX = (float)x / width;
                float normY = (float)y / height;

                float terrainHeight =
                    data.GetInterpolatedHeight(normX, normY)
                    / data.size.y;
                Debug.Log(terrainHeight);

                if (terrainHeight < 0.2f)
                {
                    map[x, y, 0] = 1f; // sand
                }
                else if (terrainHeight < 0.6f)
                {
                    map[x, y, 1] = 1f; // grass
                }
                else if (terrainHeight < 0.8f)
                {
                    map[x, y, 2] = 1f; // rock
                }
                else
                {
                    map[x, y, 3] = 1f; // snow
                }
            }
        }

        data.SetAlphamaps(0, 0, map);
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
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        return Mathf.Clamp01(
    Mathf.PerlinNoise(xCoord, yCoord) * 0.7f +
    Mathf.PerlinNoise(xCoord * 3f, yCoord * 3f) * 0.2f +
    Mathf.PerlinNoise(xCoord * 8f, yCoord * 8f) * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
