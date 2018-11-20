using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPainter : MonoBehaviour {

    private World2 world;

    Terrain terrain;
    TerrainData terrainData;

    // Use this for initialization
    void Start () {
        GameObject theWorld = GameObject.Find("World");
        world = theWorld.GetComponent<World2>();

        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }
	
	public void paintTemperatures()
    {
        float[,,] alphaData;
        alphaData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.alphamapWidth; i++)
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                for (int k = 0; k < 8; k++)
                    alphaData[i, j, k] = 0.0f;
                if (world.meanTemperature[i, j] < 0.1f)
                    alphaData[i, j, 0] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.15f)
                    alphaData[i, j, 1] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.2f)
                    alphaData[i, j, 2] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.4f)
                    alphaData[i, j, 3] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.6f)
                    alphaData[i, j, 4] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.8f)
                    alphaData[i, j, 5] = 100.0f;
                else if (world.meanTemperature[i, j] < 0.9f)
                    alphaData[i, j, 6] = 100.0f;
                else
                    alphaData[i, j, 7] = 100.0f;

            }
        terrainData.SetAlphamaps(0, 0, alphaData);
    }

    public void paintDistanceToSea()
    {
        float[,,] alphaData;
        alphaData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.alphamapWidth; i++)
            for (int j = 0; j < terrainData.alphamapHeight; j++)
                    {
                for (int k = 0; k < 8; k++)
                    alphaData[i, j, k] = 0.0f;
                if (world.distanceToSea[i, j] < 0.1f)
                    alphaData[i, j, 0] = 100.0f;
                else if (world.distanceToSea[i, j] < 15f)
                    alphaData[i, j, 1] = 100.0f;
                else if (world.distanceToSea[i, j] < 30f)
                    alphaData[i, j, 2] = 100.0f;
                else if (world.distanceToSea[i, j] < 50f)
                    alphaData[i, j, 3] = 100.0f;
                else if (world.distanceToSea[i, j] < 100f)
                    alphaData[i, j, 4] = 100.0f;
                else if (world.distanceToSea[i, j] < 200f)
                    alphaData[i, j, 5] = 100.0f;
                else if (world.distanceToSea[i, j] < 300f)
                    alphaData[i, j, 6] = 100.0f;
                else
                    alphaData[i, j, 7] = 100.0f;

            }
        terrainData.SetAlphamaps(0, 0, alphaData);
    }

    public void paintHeights()
    {
        float[,,] alphaData;
        alphaData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.alphamapWidth; i++)
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                for (int k = 0; k < 8; k++)
                    alphaData[i, j, k] = 0.0f;
                if (world.height[i, j] < 0.1f)
                    alphaData[i, j, 0] = 100.0f;
                else if (world.height[i, j] < 0.15f)
                    alphaData[i, j, 1] = 100.0f;
                else if (world.height[i, j] < 0.2f)
                    alphaData[i, j, 2] = 100.0f;
                else if (world.height[i, j] < 0.4f)
                    alphaData[i, j, 3] = 100.0f;
                else if (world.height[i, j] < 0.6f)
                    alphaData[i, j, 4] = 100.0f;
                else if (world.height[i, j] < 0.8f)
                    alphaData[i, j, 5] = 100.0f;
                else if (world.height[i, j] < 0.9f)
                    alphaData[i, j, 6] = 100.0f;
                else
                    alphaData[i, j, 7] = 100.0f;

            }
        terrainData.SetAlphamaps(0, 0, alphaData);
    }

    public void paintRainfall()
    {
        float[,,] alphaData;
        alphaData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.alphamapWidth; i++)
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                for (int k = 0; k < 8; k++)
                    alphaData[i, j, k] = 0.0f;
                if (world.meanRainfall[i, j] < 0.1f)
                    alphaData[i, j, 0] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.15f)
                    alphaData[i, j, 1] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.2f)
                    alphaData[i, j, 2] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.4f)
                    alphaData[i, j, 3] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.6f)
                    alphaData[i, j, 4] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.8f)
                    alphaData[i, j, 5] = 100.0f;
                else if (world.meanRainfall[i, j] < 0.9f)
                    alphaData[i, j, 6] = 100.0f;
                else
                    alphaData[i, j, 7] = 100.0f;

            }
        terrainData.SetAlphamaps(0, 0, alphaData);
    }

    public void paintWater()
    {
        float[,,] alphaData;
        alphaData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        for (int i = 0; i < terrainData.alphamapWidth; i++)
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                for (int k = 0; k < 8; k++)
                    alphaData[i, j, k] = 0.0f;
                if (world.height[i, j] < 0.1f)
                    alphaData[i, j, 0] = 50.0f;
                else if (world.height[i, j] < 0.15f)
                    alphaData[i, j, 1] = 50.0f;
                else if (world.height[i, j] < 0.2f)
                    alphaData[i, j, 2] = 50.0f;
                else if (world.height[i, j] < 0.4f)
                    alphaData[i, j, 3] = 50.0f;
                else if (world.height[i, j] < 0.6f)
                    alphaData[i, j, 4] = 50.0f;
                else if (world.height[i, j] < 0.8f)
                    alphaData[i, j, 5] = 50.0f;
                else if (world.height[i, j] < 0.9f)
                    alphaData[i, j, 6] = 50.0f;
                else
                    alphaData[i, j, 7] = 50.0f;
                if (world.isWater[i, j] == true)
                    alphaData[i, j, 1] = 100.0f;
            }
        terrainData.SetAlphamaps(0, 0, alphaData);
    }
}
