using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public int height = 64;
    public int width = 256;
    public int depth = 256;

    public TerrainGenerator(int terrainWidth, int terrainDepth)
    {
        width = terrainWidth;
        depth = terrainDepth;
    }

    float interpolateBilinear(float x1, float y1, float x2, float y2, float v1, float v2, float v3, float v4, float tx, float ty)
    {
        //P1:{x1,y1,v1} - P2:{x2,y1,v2} - P3:{x1,y2,v3} - P4:{x2,y2,v4}
        //Target:{tx,ty}
        //NOTE: Bind each area with the oposite value
        float area_v1 = System.Math.Abs((tx - x1) * (ty - y1)) * v4;
        float area_v2 = System.Math.Abs((tx - x2) * (ty - y1)) * v3;
        float area_v3 = System.Math.Abs((tx - x1) * (ty - y2)) * v2;
        float area_v4 = System.Math.Abs((tx - x2) * (ty - y2)) * v1;
        float area_total = (x2 - x1) * (y2 - y1);
        return (area_v1 + area_v2 + area_v3 + area_v4) / area_total;
    }

    float[,] octave_2d(float frequency, float amplitude)
    {
        int step_length = (int)Mathf.Floor(width / frequency);
        float[,] wave = new float[width + 1, depth+1];
        for (int i = 0; i <= width; i += step_length)
        {
            for (int j = 0; j <= depth; j += step_length)
            {
                //Create the nodes of the wave
                wave[i, j] = (amplitude / 2.0f) - (Random.Range(0.0f, 1.0f) * amplitude);
                //Interpolation between nodes
                if (i >= step_length && j >= step_length)
                {
                    for (int a = i - (step_length); a <= i; a++)
                    {
                        for (int b = j - (step_length); b <= j; b++)
                        {
                            int x1 = i - (step_length);
                            int x2 = i;
                            int y1 = j - (step_length);
                            int y2 = j;
                            wave[a, b] = interpolateBilinear(x1, y1, x2, y2, wave[x1, y1], wave[x2, y1], wave[x1, y2], wave[x2, y2], a, b);
                        }
                    }
                }
            }
        }
        return wave;
    }

    float [,] addWaves(float[,] A, float[,] B)
    {
        float[,] result = new float[width, depth];
        for(int i=0; i<width; i++)
            for(int j=0; j< depth; j++)
            {
                result[i, j] = A[i, j] + B[i, j];
            }
        return result;
    }

    public float[,] generatePerlin()
    {
        float[,] final_wave = new float[width, depth];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
                final_wave[i, j] = 0.0f;
        float current_pow = 1.0f;
        for (int i = 1; i < 8; i++) //7 octaves
        {
            current_pow *= 2.0f;
            float[,] octave = octave_2d(current_pow, 1.0f / current_pow);
            final_wave = addWaves(final_wave, octave);
        }

        float minimum_height = 9990.0f;
        float maximum_height = -9999.0f;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
            {
                if (final_wave[i, j] < minimum_height)
                    minimum_height = final_wave[i, j];
                if (final_wave[i, j] > maximum_height)
                    maximum_height = final_wave[i, j];
            }

        for(int i = 0; i < width; i++)
            for (int j = 0; j < depth; j++)
                final_wave[i,j] = (1/(maximum_height-minimum_height)) * (final_wave[i,j] - minimum_height);
        return final_wave;
    }

}
