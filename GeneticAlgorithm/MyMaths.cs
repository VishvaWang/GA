using System;

namespace GeneticAlgorithm
{
    public class MyMaths
    {
        //生成保留指定上下界的随机数
        public static double NextDouble(double minValue, double maxValue)
        {
            return new Random().NextDouble() * (maxValue - minValue) + minValue;
        }
        
        public static double StDev(double[] arrData) //计算标准偏差
        {
            double xSum = 0F;
            double xAvg = 0F;
            double sSum = 0F;
            double tmpStDev = 0F;
            int arrNum = arrData.Length;
            for (int i = 0; i < arrNum; i++)
            {
                xSum += arrData[i];
            }
            xAvg = xSum / arrNum;
            for (int j = 0; j < arrNum; j++)
            {
                sSum += ((arrData[j] - xAvg) * (arrData[j] - xAvg));
            }
            tmpStDev = Convert.ToSingle(Math.Sqrt((sSum / (arrNum - 1))).ToString());
            return tmpStDev;
        }

    }
}