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
                //Console.WriteLine("重复");
                return fitness;
            }
            else
            {
                List<Ship> finshed = new List<Ship>();
                //int para = 0;
                foreach (var index in decoded)
                {
                    var i = ships[index];
                    //todo 暂不需要克隆
                    //Console.WriteLine("船舶"+finshed.Count);
                    int ss = i.a;
                    int b = 0;
                    double greedy = ran.NextDouble();
                    if (greedy < 0.5)
                    {
                        int position = 0;
                        while (position == 0)
                        {
                            i.b = 0;
                            i.s = ss;
                            ss = T * T;
                            //Console.WriteLine("代数0："+i);
                            while (i.b + i.l <= L)
                            {
                                List<Ship> A = new List<Ship>();
                                List<Ship> B = new List<Ship>();
                                A = finshed.FindAll(j => j.b < i.b + i.l && j.b + j.l > i.b);
                                //Console.WriteLine("泊位冲突："+A.Count);
                                B = A.FindAll(j => j.s < i.s + i.p && j.s + j.p > i.s);
                                //Console.WriteLine("时间冲突："+B.Count);
                                if (B.Count == 0)
                                {
                                    //i.s = s;
                                    position = 1;
                                    finshed.Add(i);
                                    break;
                                }
                                else
                                {
                                    var j = B.OrderBy(j => j.b + j.l).Last();
                                    i.b = j.b + j.l;
                                    var k = B.OrderBy(k => k.GetD()).First();
                                    if (ss >= k.GetD())
                                    {
                                        ss = k.GetD();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int position = 0;
                        while (position == 0)
                        {
                            i.b = L - i.l;
                            i.s = ss;
                            ss = T * T;

                            while (i.b >= 0) 
                            {
                                List<Ship> A = new List<Ship>();
                                List<Ship> B = new List<Ship>();
                                A = finshed.FindAll(j => j.b < i.b + i.l && j.b + j.l > i.b);
                                //Console.WriteLine("泊位冲突2："+A.Count);
                                B = A.FindAll(j => j.s < i.s + i.p && j.s + j.p > i.s);
                                //Console.WriteLine("时间冲突2："+B.Count);
                                if (B.Count == 0)
                                {
                                    //i.s = s;
                                    position = 1;
                                    finshed.Add(i);
                                    break;
                                }
                                else
                                {
                                    var j = B.OrderBy(j => j.b).First();
                                    i.b = j.b - i.l;
                                    var k = B.OrderBy(k => k.GetD()).First();
                                    if (ss >= k.GetD())
                                    {
                                        ss = k.GetD();
                                    }
                                }
                            }
                        }
                    }
                }

                // for (int jj = 0; jj < V; jj++)
                // {
                //    Console.WriteLine("b:" + finshed[jj].b + ",l:" + finshed[jj].l + ",a:" + finshed[jj].a + ",s:" + finshed[jj].s+ ",d:" + finshed[jj].GetD());
                // }

                double[] f = new double[M];
                finshed = finshed.OrderBy(i => i.s).ToList();//按照si从小到大排列船舶

                for (var k = 0; k < f.Length; k++)
                {
                    finshed.ForEach(i => i.GenR());//随机生成所有船舶实际到达时间ar 和实际作业时间pr
                    //finshed.ForEach(i => i.sr = 0);
                    foreach (var i in finshed) //todo 暂不需要克隆?
                    {
                        //i.sr = 0;
                        int time = Math.Max(i.ar, i.s);
                        List<Ship> A = finshed.FindAll(j => j.s < i.s && j.b < i.b + i.l && j.b + j.l > i.b);
                        if (A.Count == 0)
                            i.sr = time;
                        else
                        {
                            if (time < A.Max(j => j.sr + j.pr))
                                time = A.Max(j => j.sr + j.pr);
                        }

                        i.sr = time;
                        f[k] = f[k] + i.sr + i.pr - i.ar;
                        
                        //Console.WriteLine("Sr:" + i.sr+",ar:"+i.ar+";pr:"+i.pr+";S:"+i.s);
                        
                    }
                }

                double mean = f.Average();
                //Console.WriteLine("mean:" + mean);
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