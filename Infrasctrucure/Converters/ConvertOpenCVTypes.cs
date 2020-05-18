using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace Infrasctructure.Converters
{
    public static class ConvertOpenCVTypes
    {
        public static float[,] ConvertMatToFloatArray(Mat matObject)
        {
            float[,] array = new float[matObject.Rows, matObject.Cols];

            for (int i = 0; i < matObject.Rows; i++)
            {
                for (int j = 0; j < matObject.Cols; j++)
                {
                    float[] point = new float[1];
                    matObject.GetArray(i, j, point);
                    array[i, j] = point[0];
                }
            }

            return array;
        }

        public static Mat ConvertFloatArrayToMat(float[,] floatArray)
        {
            return new Mat(floatArray.GetLength(0), floatArray.GetLength(1), MatType.CV_32FC1, floatArray);
        }
    }
}
