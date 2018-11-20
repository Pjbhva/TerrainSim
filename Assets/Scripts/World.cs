using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World2 : MonoBehaviour {

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
    void Start() {
        Debug.LogFormat("At Start");
        seaLevel = 0.2f;
        height = new float[width, depth];
        distanceToSea = new float[width + 1, depth + 1];
        distanceToWater = new float[width, depth];
        meanTemperature = new float[width, depth];
        varTemperature = new float[width, depth];
        meanRainfall = new float[width, depth];
        varRainfall = new float[width, depth];
        isWater = new bool[width, depth];
        freshWater = new bool[width, depth];
        Debug.LogFormat("Variables Created");
        //TerrainGenerator terrainGenerator = new TerrainGenerator(width, depth);
        //height = terrainGenerator.generatePerlin();

        GameObject theTerrain = GameObject.Find("Terrain");
        terrain = theTerrain.GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        TerrainGenerator terrainGenerator = theTerrain.GetComponent<TerrainGenerator>();
        Debug.LogFormat("Terrain found");
        height = terrainGenerator.generatePerlin();
        Debug.LogFormat("Terrain generated");
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, terrainGenerator.height, depth);
        terrainData.SetHeights(0, 0, height);

        /*calculateDistancesToSea();
        calculateMeanTemperature();
        calculateVarTemperature();
        calculateMeanRainfall();*/
        //generateFreshWater();
    }

    // Update is called once per frame
    void Update() {

    }

    void calculateDistancesToSea()
    {
        int cellsChecked = 0;

        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                distanceToSea[i, j] = -1f;
            }

        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
                if (height[i, j] < seaLevel)
                {
                    distanceToSea[i, j] = 0f;
                    cellsChecked++;
                }

        float currentDistance = 0f;
        while (cellsChecked < width * depth)
        {
            for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < depth - 1; j++)
                    if (distanceToSea[i, j] < 0f &&
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
                if (distanceToSea[width - 1, i] < 0f &&
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
                latitude = ((float)j) / ((float)depth);
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
                if (latitude < Constants.latitudeOfModerateRainfall)
                {
                    meanRainfall[i, j] = a1 * latitude * latitude + b1 * latitude + c1;
                }
                else if (latitude < (2 * Constants.latitudeOfMinRainfall - Constants.latitudeOfModerateRainfall))
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
            while (!finalStep)
            {
                filledWater[xSource, ySource] += Constants.deltaRiverGeneration;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < depth; j++)
                        if(filledWater[i,j] > 0f)
                        {
                            direction = findFlowDirection(filledWater, i, j);
                            if(direction != -1)
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
        if (x < width - 1 && !isWater[x+1,y])
            heightN = height[x + 1, y];
        else
            heightN = 1f;
        if (x > 0 && !isWater[x - 1, y])
            heightS = height[x - 1, y];
        else
            heightS = 1f;
        if (y < depth - 1 && !isWater[x, y+1])
            heightW = height[x, y + 1];
        else
            heightW = 1f;
        if (y > 0 && !isWater[x, y-1])
            heightE = height[x, y - 1];
        else
            heightE = 1f;

        float comparingHeight = height[x, y];
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
            heightW = height[x, y + 1] + filledWater[x,y+1];
        else
            heightW = 1f;
        if (y > 0)
            heightE = height[x, y - 1] + filledWater[x,y+1];
        else
            heightE = 1f;

        float comparingHeight = height[x, y] + filledWater[x,y];
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

    /*int[] buildLake(int x, int y)
    {
        Debug.LogFormat("Entering buildLake: {0}, {1}", x,y);
        int direction = findLowestHeight(x, y);
        int[,] escapeAll = new int[2, 4];
        int[] escape = { -1, -1 };
        for(int i = 0; i<4; i++)
        {
            escapeAll[0, i] = -1;
            escapeAll[1, i] = -1;
        }
        if(direction == -1)
        {
            freshWater[x, y] = true;
            isWater[x, y] = true;
            if (x < width - 1 && isWater[x + 1, y] == false)
                escape = buildLake(x + 1, y);
            escapeAll[0, 0] = escape[0];
            escapeAll[1, 0] = escape[1];
            if (x > 0 && isWater[x - 1, y] == false)
                escape = buildLake(x - 1, y);
            escapeAll[0, 1] = escape[0];
            escapeAll[1, 1] = escape[1];
            if (y < depth - 1 && isWater[x, y+1] == false)
                escape = buildLake(x, y+1);
            escapeAll[0, 2] = escape[0];
            escapeAll[1, 2] = escape[1];
            if (y > 0 && isWater[x, y-1] == false)
                escape = buildLake(x, y-1);
            escapeAll[0, 3] = escape[0];
            escapeAll[1, 3] = escape[1];
        }
        if(direction == 0)
        {
            if(isWater[x+1,y])
            {
                freshWater[x, y] = true;
                isWater[x, y] = true;
                if (x > 0 && isWater[x - 1, y] == false)
                    escape = buildLake(x - 1, y);
                escapeAll[0, 1] = escape[0];
                escapeAll[1, 1] = escape[1];
                if (y < depth - 1 && isWater[x, y + 1] == false)
                    escape = buildLake(x, y + 1);
                escapeAll[0, 2] = escape[0];
                escapeAll[1, 2] = escape[1];
                if (y > 0 && isWater[x, y - 1] == false)
                    escape = buildLake(x, y - 1);
                escapeAll[0, 3] = escape[0];
                escapeAll[1, 3] = escape[1];
            }
            else
            {
                escapeAll[0, 0] = x + 1;
                escapeAll[1, 0] = y;
            }
        }
        if (direction == 1)
        {
            if (isWater[x - 1, y])
            {
                freshWater[x, y] = true;
                isWater[x, y] = true;
                if (x < width - 1 && isWater[x + 1, y] == false)
                    escape = buildLake(x + 1, y);
                escapeAll[0, 0] = escape[0];
                escapeAll[1, 0] = escape[1];
                if (y < depth - 1 && isWater[x, y + 1] == false)
                    escape = buildLake(x, y + 1);
                escapeAll[0, 2] = escape[0];
                escapeAll[1, 2] = escape[1];
                if (y > 0 && isWater[x, y - 1] == false)
                    escape = buildLake(x, y - 1);
                escapeAll[0, 3] = escape[0];
                escapeAll[1, 3] = escape[1];
            }
            else
            {
                escapeAll[0, 1] = x - 1;
                escapeAll[1, 1] = y;
            }
        }
        if (direction == 2)
        {
            if (isWater[x, y+1])
            {
                freshWater[x, y] = true;
                isWater[x, y] = true;
                if (x < width - 1 && isWater[x + 1, y] == false)
                    escape = buildLake(x + 1, y);
                escapeAll[0, 0] = escape[0];
                escapeAll[1, 0] = escape[1];
                if (x > 0 && isWater[x - 1, y] == false)
                    escape = buildLake(x - 1, y);
                escapeAll[0, 1] = escape[0];
                escapeAll[1, 1] = escape[1];
                if (y > 0 && isWater[x, y - 1] == false)
                    escape = buildLake(x, y - 1);
                escapeAll[0, 3] = escape[0];
                escapeAll[1, 3] = escape[1];
            }
            else
            {
                escapeAll[0, 2] = x;
                escapeAll[1, 2] = y+1;
            }
        }
        if (direction == 3)
        {
            if (isWater[x, y - 1])
            {
                freshWater[x, y] = true;
                isWater[x, y] = true;
                if (x < width - 1 && isWater[x + 1, y] == false)
                    escape = buildLake(x + 1, y);
                escapeAll[0, 0] = escape[0];
                escapeAll[1, 0] = escape[1];
                if (x > 0 && isWater[x - 1, y] == false)
                    escape = buildLake(x - 1, y);
                escapeAll[0, 1] = escape[0];
                escapeAll[1, 1] = escape[1];
                if (y < depth - 1 && isWater[x, y + 1] == false)
                    escape = buildLake(x, y + 1);
                escapeAll[0, 2] = escape[0];
                escapeAll[1, 2] = escape[1];
            }
            else
            {
                escapeAll[0, 3] = x;
                escapeAll[1, 3] = y - 1;
            }
        }
        float comparingHeight = 2f;
        int minIndex = 0;
        for(int i=0; i<4; i++)
        {
            if(escapeAll[0,i] != -1)
            if(comparingHeight> height[escapeAll[0, i], escapeAll[1, i]])
            {
                comparingHeight = height[escapeAll[0, i], escapeAll[1, i]];
                minIndex = i;
            }
        }
        escape[0] = escapeAll[0, minIndex];
        escape[1] = escapeAll[1, minIndex];
        Debug.LogFormat("The escape is: {0}, {1}. Direction: {2}", escape[0], escape[1], direction);
        return escape;
    }*/

    /*int[] buildLake(int x, int y)
    {
        Debug.LogFormat("Entering buildLake: {0}, {1}", x, y);
        //int direction = findLowestHeight(x, y);
        int[,] escapeAll = new int[2, 4];
        int[] escape = { -1, -1 };
        for (int i = 0; i < 4; i++)
        {
            escapeAll[0, i] = -1;
            escapeAll[1, i] = -1;
        }
        freshWater[x, y] = true;
        isWater[x, y] = true;
        if (x < width - 1 && isWater[x + 1, y] == false)
            escape = buildLake(x + 1, y);
        escapeAll[0, 0] = escape[0];
        escapeAll[1, 0] = escape[1];
        if (x > 0 && isWater[x - 1, y] == false)
            escape = buildLake(x - 1, y);
        escapeAll[0, 1] = escape[0];
        escapeAll[1, 1] = escape[1];
        if (y < depth - 1 && isWater[x, y + 1] == false)
            escape = buildLake(x, y + 1);
        escapeAll[0, 2] = escape[0];
        escapeAll[1, 2] = escape[1];
        if (y > 0 && isWater[x, y - 1] == false)
            escape = buildLake(x, y - 1);
        escapeAll[0, 3] = escape[0];
        escapeAll[1, 3] = escape[1];
        
        float comparingHeight = 2f;
        int minIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            if (escapeAll[0, i] != -1)
                if (comparingHeight > height[escapeAll[0, i], escapeAll[1, i]])
                {
                    comparingHeight = height[escapeAll[0, i], escapeAll[1, i]];
                    minIndex = i;
                }
        }
        escape[0] = escapeAll[0, minIndex];
        escape[1] = escapeAll[1, minIndex];
        Debug.LogFormat("The escape is: {0}, {1}. Direction: {2}", escape[0], escape[1], direction);
        return escape;
    }*/

    /*int[] buildLake(int x, int y)
    {
        int[] escape = { -1, -1 };
        int[,] candidates = new int[width * depth, 2];
        int candidatesSize = 0;
        if(x<width - 1 )
        {
            candidates[candidatesSize, 0] = x + 1;
            candidates[candidatesSize, 1] = y;
            candidatesSize++;
            freshWater[x + 1, y] = true;
            isWater[x + 1, y] = true;
        }
        if (x>0)
        {
            candidates[candidatesSize, 0] = x - 1;
            candidates[candidatesSize, 1] = y;
            candidatesSize++;
            freshWater[x - 1, y] = true;
            isWater[x - 1, y] = true;
        }
        if (y < depth - 1)
        {
            candidates[candidatesSize, 0] = x;
            candidates[candidatesSize, 1] = y+1;
            candidatesSize++;
            freshWater[x, y+1] = true;
            isWater[x, y+1] = true;
        }
        if (y>0)
        {
            candidates[candidatesSize, 0] = x;
            candidates[candidatesSize, 1] = y-1;
            candidatesSize++;
            freshWater[x, y-1] = true;
            isWater[x, y-1] = true;
        }
        int[,] nextCandidates = new int[width * depth, 2];
        int nextCandidatesSize = 0;
        float scapeHeight = 1f;
        while(candidatesSize>0)
        {
            for(int i=0; i<candidatesSize; i++)
            {
                if (candidates[i, 0] < width - 1 && freshWater[candidates[i, 0] + 1, candidates[i, 1]] == false)
                {
                    nextCandidates[nextCandidatesSize, 0] = candidates[i, 0] + 1;
                    nextCandidates[nextCandidatesSize, 1] = candidates[i, 1];
                    nextCandidatesSize++;
                    freshWater[candidates[i, 0] + 1, candidates[i, 1]] = true;
                    isWater[candidates[i, 0] + 1, candidates[i, 1]] = true;
                    if(scapeHeight > height[candidates[i, 0] + 1, candidates[i, 1]])
                    {
                        scapeHeight = height[candidates[i, 0] + 1, candidates[i, 1]];
                        escape[0] = candidates[i, 0] + 1;
                        escape[1] = candidates[i, 1];
                    }
                }
                if (candidates[i, 0] > 0 && freshWater[candidates[i, 0] - 1, candidates[i, 1]] == false)
                {
                    nextCandidates[nextCandidatesSize, 0] = candidates[i, 0] - 1;
                    nextCandidates[nextCandidatesSize, 1] = candidates[i, 1];
                    nextCandidatesSize++;
                    freshWater[candidates[i, 0] - 1, candidates[i, 1]] = true;
                    isWater[candidates[i, 0] - 1, candidates[i, 1]] = true;
                    if (scapeHeight > height[candidates[i, 0] - 1, candidates[i, 1]])
                    {
                        scapeHeight = height[candidates[i, 0] - 1, candidates[i, 1]];
                        escape[0] = candidates[i, 0] - 1;
                        escape[1] = candidates[i, 1];
                    }
                }
                if (candidates[i, 1] < depth - 1 && freshWater[candidates[i, 0], candidates[i, 1] + 1] == false)
                {
                    nextCandidates[nextCandidatesSize , 0] = candidates[i, 0];
                    nextCandidates[nextCandidatesSize , 1] = candidates[i, 1] + 1;
                    nextCandidatesSize++;
                    freshWater[candidates[i, 0], candidates[i, 1] + 1] = true;
                    isWater[candidates[i, 0], candidates[i, 1] + 1] = true;
                    if (scapeHeight > height[candidates[i, 0], candidates[i, 1] + 1])
                    {
                        scapeHeight = height[candidates[i, 0], candidates[i, 1] + 1];
                        escape[0] = candidates[i, 0];
                        escape[1] = candidates[i, 1] + 1;
                    }
                }
                if (candidates[i, 1] > 0 && freshWater[candidates[i, 0], candidates[i, 1]-1] == false)
                {
                    nextCandidates[nextCandidatesSize , 0] = candidates[i, 0];
                    nextCandidates[nextCandidatesSize , 1] = candidates[i, 1] - 1;
                    nextCandidatesSize++;
                    freshWater[candidates[i, 0], candidates[i, 1] - 1] = true;
                    isWater[candidates[i, 0], candidates[i, 1] - 1] = true;
                    if (scapeHeight > height[candidates[i, 0], candidates[i, 1] - 1])
                    {
                        scapeHeight = height[candidates[i, 0], candidates[i, 1] - 1];
                        escape[0] = candidates[i, 0];
                        escape[1] = candidates[i, 1] - 1;
                    }
                }
                candidates = nextCandidates;
                candidatesSize = nextCandidatesSize;
                nextCandidatesSize = 0;
            }
        }
        return escape;
    }*/

    /*int[] buildLake(int x, int y)
    {
        int[] escape = { -1, -1 };
        int[,] candidates = new int[width * depth, 2];
        int candidatesSize = 0;
        int[,] lake = new int[width * depth, 2];
        int lakeSize = 0;
        int[,] shore = new int[width * depth, 2];
        int shoreSize = 0;
        float escapeheight = 2f;
        int currentCandidate = 0;

        candidates[0, 0] = x + 1;
        candidates[0, 1] = y;
        candidates[1, 0] = x - 1;
        candidates[1, 1] = y;
        candidates[2, 0] = x;
        candidates[2, 1] = y + 1;
        candidates[3, 0] = x;
        candidates[3, 1] = y - 1;
        candidatesSize = 4;

        lake[0, 0] = x + 1;
        lake[0, 1] = y;
        lake[1, 0] = x - 1;
        lake[1, 1] = y;
        lake[2, 0] = x;
        lake[2, 1] = y + 1;
        lake[3, 0] = x;
        lake[3, 1] = y - 1;
        lake[4, 0] = x;
        lake[4, 1] = y;
        lakeSize = 5;

        while (currentCandidate < candidatesSize)
        {
            //Right
            if (!isIn(lake, lakeSize, candidates[currentCandidate, 0] + 1, candidates[currentCandidate, 1])
                && !isIn(shore, shoreSize, candidates[currentCandidate, 0] + 1, candidates[currentCandidate, 1]))
            {
                if (height[candidates[currentCandidate, 0] + 1, candidates[currentCandidate, 1]] < height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]])
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0] + 1;
                    shore[shoreSize, 1] = candidates[currentCandidate, 1];
                    shoreSize++;
                    if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]] < escapeheight)
                    {
                        escapeheight = height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]];
                        escape[0] = candidates[currentCandidate, 0];
                        escape[1] = candidates[currentCandidate, 1];
                    }
                }
                else if (height[candidates[currentCandidate, 0] + 1, candidates[currentCandidate, 1]] > escapeheight)
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0] + 1;
                    shore[shoreSize, 1] = candidates[currentCandidate, 1];
                    shoreSize++;
                }
                else
                {
                    lake[lakeSize, 0] = candidates[currentCandidate, 0] + 1;
                    lake[lakeSize, 1] = candidates[currentCandidate, 1];
                    lakeSize++;
                    candidates[candidatesSize, 0] = candidates[currentCandidate, 0] + 1;
                    candidates[candidatesSize, 1] = candidates[currentCandidate, 1];
                    candidatesSize++;
                }
            }
            //Left
            if (!isIn(lake, lakeSize, candidates[currentCandidate, 0] - 1, candidates[currentCandidate, 1])
                && !isIn(shore, shoreSize, candidates[currentCandidate, 0] - 1, candidates[currentCandidate, 1]))
            {
                if (height[candidates[currentCandidate, 0] - 1, candidates[currentCandidate, 1]] < height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]])
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0] - 1;
                    shore[shoreSize, 1] = candidates[currentCandidate, 1];
                    shoreSize++;
                    if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]] < escapeheight)
                    {
                        escapeheight = height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]];
                        escape[0] = candidates[currentCandidate, 0];
                        escape[1] = candidates[currentCandidate, 1];
                    }
                }
                else if (height[candidates[currentCandidate, 0] - 1, candidates[currentCandidate, 1]] > escapeheight)
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0] - 1;
                    shore[shoreSize, 1] = candidates[currentCandidate, 1];
                    shoreSize++;
                }
                else
                {
                    lake[lakeSize, 0] = candidates[currentCandidate, 0] - 1;
                    lake[lakeSize, 1] = candidates[currentCandidate, 1];
                    lakeSize++;
                    candidates[candidatesSize, 0] = candidates[currentCandidate, 0] - 1;
                    candidates[candidatesSize, 1] = candidates[currentCandidate, 1];
                    candidatesSize++;
                }
            }
            //Above
            if (!isIn(lake, lakeSize, candidates[currentCandidate, 0], candidates[currentCandidate, 1] + 1)
                && !isIn(shore, shoreSize, candidates[currentCandidate, 0], candidates[currentCandidate, 1] + 1))
            {
                if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1] + 1] < height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]])
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0];
                    shore[shoreSize, 1] = candidates[currentCandidate, 1] + 1;
                    shoreSize++;
                    if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]] < escapeheight)
                    {
                        escapeheight = height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]];
                        escape[0] = candidates[currentCandidate, 0];
                        escape[1] = candidates[currentCandidate, 1];
                    }
                }
                else if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1] + 1] > escapeheight)
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0];
                    shore[shoreSize, 1] = candidates[currentCandidate, 1] + 1;
                    shoreSize++;
                }
                else
                {
                    lake[lakeSize, 0] = candidates[currentCandidate, 0];
                    lake[lakeSize, 1] = candidates[currentCandidate, 1] + 1;
                    lakeSize++;
                    candidates[candidatesSize, 0] = candidates[currentCandidate, 0];
                    candidates[candidatesSize, 1] = candidates[currentCandidate, 1] + 1;
                    candidatesSize++;
                }
            }
            //Below
            if (!isIn(lake, lakeSize, candidates[currentCandidate, 0], candidates[currentCandidate, 1] - 1)
                && !isIn(shore, shoreSize, candidates[currentCandidate, 0], candidates[currentCandidate, 1] - 1))
            {
                if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1] - 1] < height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]])
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0];
                    shore[shoreSize, 1] = candidates[currentCandidate, 1] - 1;
                    shoreSize++;
                    if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]] < escapeheight)
                    {
                        escapeheight = height[candidates[currentCandidate, 0], candidates[currentCandidate, 1]];
                        escape[0] = candidates[currentCandidate, 0];
                        escape[1] = candidates[currentCandidate, 1];
                    }
                }
                else if (height[candidates[currentCandidate, 0], candidates[currentCandidate, 1] - 1] > escapeheight)
                {
                    shore[shoreSize, 0] = candidates[currentCandidate, 0];
                    shore[shoreSize, 1] = candidates[currentCandidate, 1] - 1;
                    shoreSize++;
                }
                else
                {
                    lake[lakeSize, 0] = candidates[currentCandidate, 0];
                    lake[lakeSize, 1] = candidates[currentCandidate, 1] - 1;
                    lakeSize++;
                    candidates[candidatesSize, 0] = candidates[currentCandidate, 0];
                    candidates[candidatesSize, 1] = candidates[currentCandidate, 1] - 1;
                    candidatesSize++;
                }
            }

            currentCandidate++;
        }

        for (int i = 0; i < lakeSize; i++)
        {
            if (height[lake[i, 0], lake[i, 1]] <= escapeheight)
            {
                freshWater[lake[i, 0], lake[i, 1]] = true;
                isWater[lake[i, 0], lake[i, 1]] = true;
            }
        }

        return escape;
    }*/


    bool isIn(int[,] set, int setSize, int x, int y)
    {
        bool result = false;
        for (int i = 0; i < setSize; i++)
        {
            if (set[i, 0] == x && set[i, 1] == y)
            {
                result = false;
            }
        }
        return result;
    }
}
