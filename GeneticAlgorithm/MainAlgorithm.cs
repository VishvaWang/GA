using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static  GeneticAlgorithm.AlgorithmParameter;
namespace GeneticAlgorithm
{
    class MainAlgorithm
    {
        //初始化船舶
        public static List<Ship> ships = Ship.InitShips();

        //历史序列集合
        public static Dictionary<string, double> history = new Dictionary<string, double>();
        //初始化染色体数组
        private static List<Chromosome> initialChromosome = Chromosome.initChromsomes();
        static void Main(string[] args)
        {
            Algorithm2();
            Algorithm1();
        }
        private static void Algorithm1()
        {
            //算法一
            //首先复制初始染色体,注意,因为list是一个引用类型,不能简单的赋值,又因为lsit中存放的也是引用类型,也不能使用浅克隆.
            var generation = Chromosome.Copy(initialChromosome);

            //开始计时
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < G; i++)
            {
                generation.Sort(); //从小到大排列染色体

                //舍弃适应度函数值较大的后2/1染色体
                generation.RemoveRange(N / 2, N / 2);
                //强化前二分之一

                Chromosome.Intensify(generation, generation.Min());

                var elite = Chromosome.Copy(generation);

                //选择操作
                elite = Chromosome.Select(elite);
                //交叉
                Chromosome.Crossover(elite);
                //变异
                Chromosome.Mutation(elite);
                generation.AddRange(elite);
            }

            generation.Sort();
            Console.WriteLine("方法1最优染色体编码为: " + string.Join(",", generation.First().encoded));
            Console.WriteLine("方法1最优染色体序列为: " + string.Join(",", generation.First().GetDecoded()));
            Console.WriteLine("方法1最优适应函数值为: " + generation.First().GetFitness());
            Console.WriteLine("最优适应函数值为: " + history.Values.Min());

            //结束计时
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("方法1总共花费{0}ms.", ts2.TotalMilliseconds);
            Console.WriteLine("  ");
            Console.WriteLine("  ");
        }
        private static void Algorithm2()
        {
            if (N % 2 != 0)
                throw new ArgumentException("种群数量必须为偶数");
            
            List<Chromosome> generation;
            Stopwatch sw;
            TimeSpan ts2;
            generation = Chromosome.Copy(initialChromosome);
            //算法二

            //开始计时
            sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < G; i++)
            {
                generation.Sort(); //从小到大排列染色体
                var elite = generation.GetRange(0, N / 2);
                //精英 进行 选择 ,交叉,变异
                elite = Chromosome.Select(elite); 
                Chromosome.Crossover(elite);
                Chromosome.Mutation(elite);

                Chromosome.Intensify(generation.GetRange(N / 2, N / 2), generation.Min());
            }

            generation.Sort();
            Console.WriteLine("方法2最优染色体编码为: " + string.Join(",", generation.First().encoded));
            Console.WriteLine("方法2最优染色体序列为: " + string.Join(",", generation.First().GetDecoded()));
            Console.WriteLine("方法2最优适应函数值为: " + generation.First().GetFitness());
            Console.WriteLine("最优适应函数值为: " + history.Values.Min());

            //结束计时
            sw.Stop();
            ts2 = sw.Elapsed;
            Console.WriteLine("方法2总共花费{0}ms.", ts2.TotalMilliseconds);
            Console.WriteLine("  ");
            Console.WriteLine("  ");
        }
        
    }
}