using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {

    //The temperature normalized to (0,1) at the equator
    public static float temperatureOnEquator = 0.8f;
    //The latitude normalized to (0,1) at which the temperature is maximum
    public static float latitudeOfMaxTemperature = 0.33f;
    //How much the latitude influences the mean temperature
    public static float latitudeContributionToTemperature = 0.4f;
    //How much the altitude influences the mean temperature
    public static float altitudeContributionToTemperature = 0.6f;
    //The latitude at which the regime of Rainfall changes. It marks the belt of lowest Rainfall
    public static float latitudeOfModerateRainfall = 0.2f;
    //The latitude at which the rainfall gets to its minimum
    public static float latitudeOfMinRainfall = 0.33f;
    //The northern latitude at which rainfall is max
    public static float latitudeOfSecondMaxRainfall = 0.75f;
    //The mean rainfall, reached at latitudeOfModerateRainfall
    public static float medianRainfall = 0.5f;
    //The second maximum of rainfall, reached at latitudeOfSecondMaxRainfall
    public static float secondMaxRainfall = 0.75f;
    //How much the latitude influences the mean rainfall (between 0 and 1)
    public static float latitudeContributionToRainfall = 0.33f;
    //How much the altitude influences the mean rainfall
    public static float altitudeContributionToRainfall = 0.33f;
    //How much the distance to the sea influences the mean rainfall
    public static float distanceToSeaContributionToRainfall = 0.33f;

    //How many rivers there are per tile
    public static float riverDensity = 0.00000762939453125f;
    //The minimum altitude required to spawn a water source
    public static float minAltitudeSource = 0.8f;
    //The delta of water to fill at every step for simulating a river at river generation
    public static float deltaRiverGeneration = 0.001f;
}
