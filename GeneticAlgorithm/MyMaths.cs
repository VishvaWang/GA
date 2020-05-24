using System;
using System.Collections.Generic;
using System.Linq;
using static GeneticAlgorithm.MainAlgorithm;
using static  GeneticAlgorithm.AlgorithmParameter;

namespace GeneticAlgorithm
{
    public class MyMaths
    {
        //生成保留指定上下界的随机数
        public static double NextDouble(double minValue, double maxValue)
        {
            return AlgorithmParameter.ran.NextDouble() * (maxValue - minValue) + minValue;
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
            tmpStDev = Convert.ToSingle(Math.Sqrt((sSum / arrNum)).ToString());
            return tmpStDev;
        }
        
        //计算适应度函数值
        public static double Fitness(int[] decoded)
        {    //数组无法作为key,需要转换成字符串或重写GetHashCode及Equales方法
            var decodedStr = string.Join(",",decoded);

            double fitness;
            if (HistoryRecords.TryGetValue(decodedStr, out fitness))//如果存在历史记录,就返回历史记录
            {
                return fitness;
            }
            else
            {
                List<Ship> finshed = new List<Ship>();
                foreach (var index in decoded)
                {
                    var i = ships[index];
                    //todo 暂不需要克隆
                    i.b = 0;
                    int s = 2 * T;
                    int b = 0;

                    //集合A
                    List<Ship> A = new List<Ship>();
                    A = finshed.FindAll(j => j.GetD() > i.a);

                    while (i.b + i.l <= L)
                    {
                        //集合B
                        List<Ship> B = new List<Ship>();
                        B = A.FindAll(j => j.b < i.b + i.l && j.b + j.l > i.b);

                        if (B.Count == 0)
                        {
                            s = i.a;
                            b = i.b;
                            break;
                        }
                        else
                        {
                            var j = B.OrderBy(j => j.GetD()).Last();
                            if (s > j.GetD())
                            {
                                s = j.GetD();
                                b = i.b;
                            }

                            i.b = j.b + j.l;
                            //B = A.FindAll(j => j.b < i.b + i.l && j.b + j.l > i.b);
                        }
                    }

                    i.s = s;
                    i.b = b;
                    finshed.Add(i);
                }

                double[] f = new double[M];
                finshed.OrderBy(i => i.s);//按照si从小到大排列船舶

                for (var k = 0; k < f.Length; k++)
                {
                    finshed.ForEach(i => i.GenR());//随机生成所有船舶实际到达时间ar 和实际作业时间pr
                    
                    foreach (var i in finshed) //todo 暂不需要克隆?
                    {
                        i.sr = 0;
                        int time = Math.Max(i.ar, i.s);
                        List<Ship> A = finshed.FindAll(j => j.GetD() > i.a && j.b < i.b + i.l && j.b + j.l > i.b);
                        if (time < A.Max(j => j.sr + j.pr))
                            time = A.Max(j => j.sr + j.pr);

                        i.sr = time;
                        f[k] = f[k] + i.sr + i.pr - i.ar;
                        //Console.WriteLine("Sr:" + i.sr+",ar:"+i.ar+";pr:"+i.pr+";S:"+i.s);
                    }
                }

                double mean = f.Average();
                double devi = StDev(f);
                
                fitness = mean + λ * devi;
                HistoryRecords[decodedStr] = mean + λ * devi;
                //Console.WriteLine("Mean" + mean);
                //Console.WriteLine("devi" + devi);
                return fitness;
            }
        }
    }
}