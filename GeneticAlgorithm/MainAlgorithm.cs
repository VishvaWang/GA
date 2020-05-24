﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static  GeneticAlgorithm.AlgorithmParameter;
namespace GeneticAlgorithm
{
    class MainAlgorithm
    {
        //初始化船舶
        public static List<Ship> ships = Ship.InitShips();

        //历史序列集合
        public static Dictionary<string, double> HistoryRecords = new Dictionary<string, double>();
        //初始化染色体数组
        private static List<Chromosome> initialChromosome = Chromosome.initChromsomes();
        static void Main(string[] args)
        {
            Console.Out.WriteLine(" ");
            var fullName = new DirectoryInfo("data.txt").FullName;
            Console.Out.WriteLine("文件完整路径为: " + fullName);
            Console.Out.WriteLine(" ");
            using (StreamWriter sw = new StreamWriter("data.txt"))
            {
                for (int i = 0; i < ships.Count; i++)
                {
                    sw.WriteLine("{0}    {1}    {2}    {3}", i + 1, ships[i].a, ships[i].l, ships[i].p);
                }
            }
            
            Algorithm2();
            //Algorithm1();
            //Algorithm3();//注意 方法三由于没有使用排序,所以使用的是generation.Min()而非其他两个的generation.First()
            
        }
        private static void Algorithm1()
        {
            //算法一
            //首先复制初始染色体,注意,因为list是一个引用类型,不能简单的赋值,又因为lsit中存放的也是引用类型,也不能使用浅克隆.
            var generation = Chromosome.Copy(initialChromosome);

            //开始计时
            Stopwatch sw = new Stopwatch();
            sw.Start();

            double lastMin = 0;
            
            for (int i = 0,noIm = 0; i < G; i++)
            {
                generation.Sort(); //从小到大排列染色体
                
                if (i != 0)//至少执行一次
                {
                    if (lastMin - generation.First().GetFitness() <= NoImprovedCriticalCondition)//如果没有改进,计数加一
                    {
                        noIm++;
                    }
                    else
                    {
                        noIm = 0;
                    }

                }
                
                if (noIm >= MaxGenerationNoImproved)//
                {
                    Console.WriteLine("代数：" + i);
                    break;
                }
                lastMin = generation.First().GetFitness();//更新 lastmain

                //舍弃适应度函数值较大的后2/1染色体
                generation.RemoveRange(N / 2, N / 2);
                //强化前二分之一

                Chromosome.Intensify(generation, generation.Min());

                var elite = Chromosome.Copy(generation);

                //选择操作
                elite = Chromosome.Select(elite);
                //交叉
                Chromosome.Crossover1(elite);
                //变异
                Chromosome.Mutation(elite);
                generation.AddRange(elite);
                
            }

            generation.Sort();
            Console.WriteLine("方法1最优染色体编码为: " + string.Join(",", generation.First().encoded));
            Console.WriteLine("方法1最优染色体序列为: " + string.Join(",", generation.First().GetDecoded()));
            Console.WriteLine("方法1最优适应函数值为: " + generation.First().GetFitness());
            Console.WriteLine("最优适应函数值为: " + HistoryRecords.Values.Min());

            //结束计时
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("方法1总共花费{0}ms.", ts2.TotalMilliseconds);
            Console.WriteLine("  ");
        }
        private static void Algorithm2()
        {
            
            List<Chromosome> generation;
            Stopwatch sw;
            TimeSpan ts2;
            generation = Chromosome.Copy(initialChromosome);
            //算法二

            //开始计时
            sw = new Stopwatch();
            sw.Start();
            
            double lastMin = 0;

            for (int i = 0,noIm = 0; i < G; i++)
            {
                generation.Sort(); //从小到大排列染色体
                // Console.WriteLine("方法2最优染色体编码为: " + string.Join(",", generation.First().encoded));
                // Console.WriteLine("方法2最优适应函数值为: " + generation.First().GetFitness());
                // Console.WriteLine("方法2最差适应函数值为: " + generation.Last().GetFitness());
                if (i != 0)//至少执行一次
                {
                    if (lastMin - generation.First().GetFitness() <= NoImprovedCriticalCondition)//如果没有改进,计数加一
                    {
                        noIm++;
                    }
                    else
                    {
                        noIm = 0;
                    }

                }
                
                if (noIm >= MaxGenerationNoImproved)//
                {
                    Console.WriteLine("代数：" + i);
                    break;
                }
                lastMin = generation.First().GetFitness();//更新 lastmain
                
                var elite = generation.GetRange(0, N / 2);
                var ordinary = generation.GetRange(N/2, N/2);
                
                Console.WriteLine("代数：" + i);

                Console.WriteLine("目前代最优适应函数值为: " + generation.Min().GetFitness());
                Console.WriteLine("目前精英最优适应函数值为: " + elite.Min().GetFitness());
                //精英 进行 选择 ,交叉,变异
                elite = Chromosome.Select(elite);
                Console.WriteLine("目前代最优适应函数值为: " + generation.Min().GetFitness());
                Console.WriteLine("目前精英最优适应函数值为: " + elite.Min().GetFitness());

                Chromosome.Crossover(elite);
                Console.WriteLine("目前代最优适应函数值为: " + generation.Min().GetFitness());
                Console.WriteLine("目前精英最优适应函数值为: " + elite.Min().GetFitness());


                Chromosome.Mutation(elite);
                Console.WriteLine("目前代最优适应函数值为: " + generation.Min().GetFitness());
                Console.WriteLine("目前精英最优适应函数值为: " + elite.Min().GetFitness());


                Chromosome.Intensify(ordinary, elite.Min());
                
                generation.Clear();
                generation.AddRange(elite);
                generation.AddRange(ordinary);
                
                Console.WriteLine("目前代最优适应函数值为: " + generation.Min().GetFitness());
                Console.WriteLine("目前精英最优适应函数值为: " + elite.Min().GetFitness());

                Console.WriteLine("  ");

            }

            generation.Sort();
            Console.WriteLine("方法2最优染色体编码为: " + string.Join(",", generation.First().encoded));
            Console.WriteLine("方法2最优染色体序列为: " + string.Join(",", generation.First().GetDecoded()));
            Console.WriteLine("方法2最优适应函数值为: " + generation.First().GetFitness());
            Console.WriteLine("最优适应函数值为: " + HistoryRecords.Values.Min());


            //结束计时
            sw.Stop();
            ts2 = sw.Elapsed;
            Console.WriteLine("方法2总共花费{0}ms.", ts2.TotalMilliseconds);
            Console.WriteLine("  ");
        }
        
        private static void Algorithm3()
        {
            List<Chromosome> generation;
            Stopwatch sw;
            TimeSpan ts3;
            generation = Chromosome.Copy(initialChromosome);
            //算法GA

            //开始计时
            sw = new Stopwatch();
            sw.Start();

            double lastMin = 0;

            for (int i = 0, noIm = 0; i < G; i++)
            {    
                generation.Sort(); //从小到大排列染色体
                Console.WriteLine("方法GA最优染色体编码为: " + string.Join(",", generation.First().encoded));
                Console.WriteLine("方法GA最优适应函数值为: " + generation.First().GetFitness());
                
                if (i != 0)//至少执行一次
                {
                    if (lastMin - generation.First().GetFitness() <= NoImprovedCriticalCondition)//如果没有改进,计数加一
                    {
                        noIm++;
                    }
                    else
                    {
                        noIm = 0;
                    }

                }
                
                if (noIm >= MaxGenerationNoImproved)//
                {
                    Console.WriteLine("代数：" + i);
                    break;
                }
                lastMin = generation.First().GetFitness();//更新 lastmin
                //generation.Sort(); //从小到大排列染色体
                var elite = generation.GetRange(0, N);
                //精英 进行 选择 ,交叉,变异
                elite = Chromosome.Select(elite);
                Chromosome.Crossover(elite);
                Chromosome.Mutation(elite);

                //Chromosome.Intensify(generation.GetRange(N / 2, N / 2), generation.Min());
            }

            generation.Sort();
            Console.WriteLine("方法GA最优染色体编码为: " + string.Join(",", generation.First().encoded));
            Console.WriteLine("方法GA最优染色体序列为: " + string.Join(",", generation.First().GetDecoded()));
            Console.WriteLine("方法GA最优适应函数值为: " + generation.First().GetFitness());
            Console.WriteLine("最优适应函数值为: " + HistoryRecords.Values.Min());

            //结束计时
            sw.Stop();
            ts3 = sw.Elapsed;
            Console.WriteLine("方法GA总共花费{0}ms.", ts3.TotalMilliseconds);
            Console.WriteLine("  ");
        }
        
    }
}