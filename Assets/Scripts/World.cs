using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public float seaLevel;
    public float maxDistanceToSea;
    public int width = 512;
    public int depth = 512;
    public int maxHeight = 64;

    public float[,] height;
    public bool[,] isWater;
    public float[,] distanceToSea;
    public float[,] distanceToWater;
    public float[,] meanTemperature;
    public float[,] varTemperature;
    public float[,] meanRainfall;
    public float[,] varRainfall;
    public bool[,] freshWater;

    Terrain terrain;

	// Use this for initialization
	void Start () {
		seaLevel = 0.2f;
        height = new float[width, depth];
        distanceToSea = new float[width+1, depth+1];
        distanceToWater = new float[width, depth];
        meanTemperature = new float[width, depth];
        varTemperature = new float[width, depth];
        meanRainfall = new float[width, depth];
        varRainfall = new float[width, depth];
        isWater = new bool[width, depth];
        freshWater = new bool[width, depth];

        //TerrainGenerator terrainGenerator = new TerrainGenerator(width, depth);
        //height = terrainGenerator.generatePerlin();

        GameObject theTerrain = GameObject.Find("Terrain");
        terrain = theTerrain.GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        TerrainGenerator terrainGenerator = theTerrain.GetComponent<TerrainGenerator>();

        height = terrainGenerator.generatePerlin();

        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, terrainGenerator.height, depth);
        terrainData.SetHeights(0, 0, height);

        calculateDistancesToSea();
        calculateMeanTemperature();
        calculateVarTemperature();
        calculateMeanRainfall();
        generateFreshWater();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void calculateDistancesToSea()
    {
        int cellsChecked = 0;
        
        for(int i=0; i<width; i++)
            for(int j=0; j<depth; j++)
            {
                distanceToSea[i, j] = -1f;
            }

        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
                if(height[i,j] < seaLevel)
                {
                    distanceToSea[i, j] = 0f;
                    cellsChecked++;
                }

        float currentDistance = 0f;        
        while(cellsChecked < width * depth)
        {
            for (int i = 1; i < width-1; i++)
                for (int j = 1; j < depth-1; j++)
                    if (distanceToSea[i,j] < 0f &&
                        (distanceToSea[i - 1, j] == currentDistance ||
                        distanceToSea[i + 1, j] == currentDistance ||
                        distanceToSea[i, j - 1] == currentDistance ||
                        distanceToSea[i, j + 1] == currentDistance))
                    {
                        distanceToSea[i, j] = currentDistance + 1;
                        cellsChecked++;
                    }
            //Borders
            for (int i = 1; i < width - 1; i++)
            {
                if (distanceToSea[i, 0] < 0f &&
                        (distanceToSea[i - 1, 0] == currentDistance ||
                        distanceToSea[i + 1, 0] == currentDistance ||
                        distanceToSea[i, 1] == currentDistance))
                {
                    distanceToSea[i, 0] = currentDistance + 1;
                    cellsChecked++;
                }
                if (distanceToSea[i, depth - 1] < 0f &&
                        (distanceToSea[i - 1, depth - 1] == currentDistance ||
                        distanceToSea[i + 1, depth - 1] == currentDistance ||
                        distanceToSea[i, depth - 2] == currentDistance))
                {
                    distanceToSea[i, depth - 1] = currentDistance + 1;
                    cellsChecked++;
                }
            }
            for (int i = 1; i < depth - 1; i++)
            { 
                if (distanceToSea[width-1, i] < 0f &&
                        (distanceToSea[width - 1, i - 1] == currentDistance ||
                        distanceToSea[width - 1, i + 1] == currentDistance ||
                        distanceToSea[width - 2, i] == currentDistance))
                {
                    distanceToSea[width - 1, i] = currentDistance + 1;
                    cellsChecked++;
                }
                if (distanceToSea[0, i] < 0f &&
                        (distanceToSea[0, i - 1] == currentDistance ||
                        distanceToSea[0, i + 1] == currentDistance ||
                        distanceToSea[1, i] == currentDistance))
                {
                    distanceToSea[0, i] = currentDistance + 1;
                    cellsChecked++;
                }
            }
            //Corners
            if (distanceToSea[0, 0] < 0f &&
                                    (distanceToSea[0, 1] == currentDistance ||
                                    distanceToSea[1, 0] == currentDistance))
            {
                distanceToSea[0, 0] = currentDistance + 1;
                cellsChecked++;
            }
            if (distanceToSea[width - 1, 0] < 0f &&
                    (distanceToSea[width - 1, 1] == currentDistance ||
                    distanceToSea[width - 2, 0] == currentDistance))
            {
                distanceToSea[width - 1, 0] = currentDistance + 1;
                cellsChecked++;
            }
            if (distanceToSea[0, depth - 1] < 0f &&
                    (distanceToSea[1, depth - 10] == currentDistance ||
                    distanceToSea[0, depth - 2] == currentDistance))
            {
                distanceToSea[0, depth - 1] = currentDistance + 1;
                cellsChecked++;
            }
            if (distanceToSea[width - 1, depth - 1] < 0f &&
                    (distanceToSea[width - 2, depth - 1] == currentDistance ||
                    distanceToSea[width - 1, depth - 2] == currentDistance))
            {
                distanceToSea[width - 1, depth - 1] = currentDistance + 1;
                cellsChecked++;
            }
            currentDistance++;
        }
        maxDistanceToSea = currentDistance;
    }

    void calculateMeanTemperature()
    {
        float latitude, altitude, a, b;
        b = (1f + Constants.temperatureOnEquator * Constants.latitudeOfMaxTemperature * Constants.latitudeOfMaxTemperature - Constants.temperatureOnEquator)
            / (Constants.latitudeOfMaxTemperature * (1f - Constants.latitudeOfMaxTemperature));
        a = -b - Constants.temperatureOnEquator;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                latitude = ((float) j) / ((float)depth);
                altitude = height[i, j];
                meanTemperature[i, j] = Constants.latitudeContributionToTemperature * (a * latitude * latitude + b * latitude + Constants.temperatureOnEquator) +
                    Constants.altitudeContributionToTemperature * (-altitude + 1f);
            }
    }

    void calculateVarTemperature()
    {
        //float maxDistance = (float) (width * width + depth * depth);
        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                varTemperature[i, j] = distanceToSea[i, j] * distanceToSea[i, j] / maxDistanceToSea;
            }
    }

    void calculateMeanRainfall()
    {
        float latitude, a1, b1, c1, a2, b2, c2, a3, b3, c3;
        a1 = (Constants.medianRainfall - 1f) / (Constants.latitudeOfModerateRainfall * Constants.latitudeOfModerateRainfall);
        b1 = 0f;
        c1 = 1f;
        a2 = Constants.medianRainfall / ((Constants.latitudeOfModerateRainfall - Constants.latitudeOfMinRainfall) * (Constants.latitudeOfModerateRainfall - Constants.latitudeOfMinRainfall));
        b2 = -2 * a2 * Constants.latitudeOfMinRainfall;
        c2 = a2 * Constants.latitudeOfMinRainfall * Constants.latitudeOfMinRainfall;
        a3 = (Constants.medianRainfall - Constants.secondMaxRainfall) /
            ((2 * Constants.latitudeOfMinRainfall - Constants.latitudeOfModerateRainfall - Constants.latitudeOfSecondMaxRainfall) * 
            (2 * Constants.latitudeOfMinRainfall - Constants.latitudeOfModerateRainfall - Constants.latitudeOfSecondMaxRainfall));
        b3 = -2 * a3 * Constants.latitudeOfSecondMaxRainfall;
        c3 = Constants.secondMaxRainfall + a3 * Constants.latitudeOfSecondMaxRainfall * Constants.latitudeOfSecondMaxRainfall;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                latitude = ((float)j) / ((float)depth);
                if(latitude < Constants.latitudeOfModerateRainfall)
                {
                    meanRainfall[i, j] = a1 * latitude * latitude + b1 * latitude + c1;
                }
                else if(latitude <(2 * Constants.latitudeOfMinRainfall - Constants.latitudeOfModerateRainfall))
                {
                    meanRainfall[i, j] = a2 * latitude * latitude + b2 * latitude + c2;
                }
                else
                {
                    meanRainfall[i, j] = a3 * latitude * latitude + b3 * latitude + c3;
                }
                meanRainfall[i, j] *= Constants.latitudeContributionToRainfall;
                meanRainfall[i, j] += Constants.altitudeContributionToRainfall * height[i, j] * height[i, j] +
                    Constants.distanceToSeaContributionToRainfall * (-(distanceToSea[i, j] * distanceToSea[i, j]) / (maxDistanceToSea * maxDistanceToSea) + 1);
            }
    }

    void generateFreshWater()
    {
        int riverNumber = (int)(Constants.riverDensity * width * depth);
        for (int iRiver = 0; iRiver < riverNumber; iRiver++)
        {
            //Initialize a matrix for water heights
            float[,] filledWater = new float[width, depth];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < depth; j++)
                    filledWater[i, j] = 0f;

            //Generate the coordinates of the source of the river
            int xSource, ySource;
            do
            {
                xSource = UnityEngine.Random.Range(0, width);
                ySource = UnityEngine.Random.Range(0, depth);
            } while (height[xSource, ySource] < Constants.minAltitudeSource);
            isWater[xSource, ySource] = true;
            freshWater[xSource, ySource] = true;

            int xCurrent = xSource, yCurrent = ySource;
            int direction;
            bool finalStep = false;
            int nstep = 0;
            while (!finalStep)
            {
                nstep++;
                filledWater[xSource, ySource] += Constants.deltaRiverGeneration;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < depth; j++)
                        if (filledWater[i, j] > 0f)
                        {
                            direction = findFlowDirection(filledWater, i, j);
                            if (direction != -1)
                            {
                                xCurrent = i; yCurrent = j;
                                if (direction == 0)
                                    xCurrent++;
                                if (direction == 1)
                                    xCurrent--;
                                if (direction == 2)
                                    yCurrent++;
                                if (direction == 3)
                                    yCurrent--;
                                if (!isWater[xCurrent, yCurrent]) isWater[xCurrent, yCurrent] = true;
                                if (!freshWater[xCurrent, yCurrent]) freshWater[xCurrent, yCurrent] = true;
                                filledWater[xCurrent, yCurrent] += Constants.deltaRiverGeneration;
                                filledWater[i, j] -= Constants.deltaRiverGeneration;

                                if (height[xCurrent, yCurrent] < seaLevel)
                                    finalStep = true;
                            }
                            //TODO: DEBUG
                            if (nstep >= Constants.maxIterationsRiverGeneration) finalStep = true;
                        }
            }
        }
    }

    //Finds which tile adjacent to some (x,y) tile has lowest height
    //Returns:
    //-1 if no adjacent tile has lower height
    //0 if (x+1,y) has lowest height
    //1 if (x-1,y) has lowest height
    //2 if (x, y+1) has lowest height
    //3 if (x, y-1) has lowest height
    int findLowestHeight(int x, int y)
    {
        int flag;

        float heightN, heightS, heightE, heightW;
        if (x < width - 1)
            heightN = height[x + 1, y];
        else
            heightN = 1f;
        if (x > 0)
            heightS = height[x - 1, y];
        else
            heightS = 1f;
        if (y < depth - 1)
            heightW = height[x, y+1];
        else
            heightW = 1f;
        if (y > 0)
            heightE = height[x, y-1];
        else
            heightE = 1f;

        float comparingHeight = height[x, y];
        flag = -1;
        if(heightN < comparingHeight)
        {
            comparingHeight = heightN;
            flag = 0;
        }
        if(heightS < comparingHeight)
        {
            comparingHeight = heightS;
            flag = 1;
        }
        if (heightW < comparingHeight)
        {
            comparingHeight = heightW;
            flag = 2;
        }
        if (heightE < comparingHeight)
        {
            comparingHeight = heightE;
            flag = 3;
        }
        return flag;
    }

    //Finds which tile adjacent to some (x,y) tile has lowest height including its water height.
    //Returns:
    //-1 if no adjacent tile has lower height
    //0 if (x+1,y) has lowest height
    //1 if (x-1,y) has lowest height
    //2 if (x, y+1) has lowest height
    //3 if (x, y-1) has lowest height
    int findFlowDirection(float[,] filledWater, int x, int y)
    {
        int flag;

        float heightN, heightS, heightE, heightW;
        if (x < width - 1)
            heightN = height[x + 1, y] + filledWater[x + 1, y];
        else
            heightN = 1f;
        if (x > 0)
            heightS = height[x - 1, y] + filledWater[x - 1, y];
        else
            heightS = 1f;
        if (y < depth - 1)
            heightW = height[x, y + 1] + filledWater[x, y + 1];
        else
            heightW = 1f;
        if (y > 0)
            heightE = height[x, y - 1] + filledWater[x, y + 1];
        else
            heightE = 1f;

        float comparingHeight = height[x, y] + filledWater[x, y];
        flag = -1;
        if (heightN < comparingHeight)
        {
            comparingHeight = heightN;
            flag = 0;
        }
        if (heightS < comparingHeight)
        {
            comparingHeight = heightS;
            flag = 1;
        }
        if (heightW < comparingHeight)
        {
            comparingHeight = heightW;
            flag = 2;
        }
        if (heightE < comparingHeight)
        {
            comparingHeight = heightE;
            flag = 3;
        }
        return flag;
    }

}
