using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GeneticAlgorithm
{
    class Program
    {
        //编译器常量
        private const int V = 20;//船舶数量
        private const int L = 60;//岸线总长度
        private const int T = 2016;//时间总长度
        private const int N = 200;//遗传算法种群数量
        private const int G = 200;//遗传算法执行代数
        private const int M = 10;//仿真实验次数
        private const double λ = 0.1;//适应度函数参数
        private const double pc = 0.4;//遗传算法交叉概率
        private const double pm = 0.1;//遗传算法变异概率
        private const int Q = 20;//强化算法执行代数
        
        //运行期常量
        private static readonly Random ran = new Random();

        static List<Ship> ships = new List<Ship>();

        //历史序列集合
        private static Dictionary<string, double> history = new Dictionary<string, double>();
        
        static void Main(string[] args)
        {
            //生成V艘船并存入列表
            for (int i = 0; i < V; i++)
                ships.Add(new Ship());
            
            //初始化染色体数组
            List<Chromosome> initialChromosome = new List<Chromosome>();
            for (var i = 0; i < N; i++)
                initialChromosome.Add(new Chromosome());
            
            //手动生成最后一条染色体
            var shipsOrderByA = ships.OrderBy(s => s.a).ToList();
            // foreach (var ship in shipsOrderByA)
            // {
            //     initialChromosome.Last().encoded[]
            // }
            //算法一
            //首先复制初始染色体,注意,因为list是一个引用类型,不能简单的赋值,又因为lsit中存放的也是引用类型,也不能使用浅克隆.
            var generation = Copy(initialChromosome);
            
            //开始计时
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < G; i++)
            {
                generation.Sort();//从小到大排列染色体
               
                //舍弃适应度函数值较大的后2/1染色体
                generation.RemoveRange(N / 2, N / 2); //todo 考虑v是奇数的情况
                //强化前二分之一
                
                Intensify(generation,generation.Max());
                
                var elite = Copy(generation);
                
                //选择操作
                elite = Select(elite);
                //交叉
                Crossover(elite);
                //变异
                Mutation(elite);
                generation.AddRange(elite);
                Console.Out.WriteLine(i);
            }
            
            generation.Sort();
            Console.WriteLine("方法1最优染色体编码为: " + string.Join(",",generation.First().encoded));
            Console.WriteLine("方法1最优染色体序列为: " + string.Join(",",generation.First().GetDecoded()));
            Console.WriteLine("方法1最优适应函数值为: " + generation.First().GetFitness());

            
            //结束计时
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("方法1总共花费{0}ms.", ts2.TotalMilliseconds);
            
            generation = Copy(initialChromosome);
            //算法二
            
            //开始计时
            sw = new Stopwatch();
            sw.Start();
           
            for (int i = 0; i < G; i++)
            {
                generation.Sort();//从小到大排列染色体
                var elite = generation.GetRange(0, N / 2);
                //精英 进行 选择 ,交叉,变异
                elite = Select(elite);//todo 考虑v是奇数的情况
                Crossover(elite);
                Mutation(elite);
                
                Intensify(generation.GetRange(N / 2, N / 2),generation.Max());
                Console.Out.WriteLine(i);
            }
            generation.Sort();
            Console.WriteLine("方法2最优染色体编码为: " + string.Join(",",generation.First().encoded));
            Console.WriteLine("方法2最优染色体序列为: " + string.Join(",",generation.First().GetDecoded()));
            Console.WriteLine("方法2最优适应函数值为: " + generation.First().GetFitness());

            //结束计时
            sw.Stop();
            ts2 = sw.Elapsed;
            Console.WriteLine("方法2总共花费{0}ms.", ts2.TotalMilliseconds);
            
        }

        private static List<Chromosome> Copy(List<Chromosome> chromosomeList)
        {
            return chromosomeList.Select(chromosome => chromosome.Clone()).ToList();
        }

        private static void Mutation(List<Chromosome> waitMutation)
        {    //排序,便于找出cBest 
            waitMutation.Sort();
            //对每个染色体执行变异
            waitMutation
                .Skip(1)// 跳过第一个,即 cBest
                .ToList()
                .ForEach(chromosome => chromosome.Mutation());
        }

        private static void Crossover(List<Chromosome> waitCrossover)
        {   
            
            //cBest 不参与交叉
            waitCrossover.Sort();
            var cBest = waitCrossover.First();
            waitCrossover.RemoveAt(0);
            
            //需要交叉的染色体序号
            List<int> needChromosomeIndexs = new List<int>();
            for (int i = 0; i < waitCrossover.Count - 1; i++)
            {
                double p = NextDouble(0, 1);
                if (p < pc)
                    needChromosomeIndexs.Add(i);
            }

            if (needChromosomeIndexs.Count % 2 != 0)
                needChromosomeIndexs.Add(waitCrossover.Count - 1);
            
            while (needChromosomeIndexs.Count != 0)
            {
                var index1 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                var index2 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                
                Chromosome chromosome = waitCrossover[index1];
                chromosome.Cross(waitCrossover[index2]);
                
                needChromosomeIndexs.Remove(index1);
                needChromosomeIndexs.Remove(index2);
            }
            
            waitCrossover.Add(cBest);
        }

        private static List<Chromosome> Select(List<Chromosome> chromosomes)
        {
            List<Chromosome> selected = new List<Chromosome>();
            selected.Add(chromosomes.First().Clone());
            selected.Add(chromosomes.First().Clone());

            while (selected.Count < chromosomes.Count)
                selected.Add(chromosomes[Math.Min(ran.Next(0, chromosomes.Count - 1), ran.Next(0, chromosomes.Count - 1))]);

            return selected;
        }

        private static void Intensify(List<Chromosome> wantIntensify, Chromosome cBest)
        {   double[][] x = new double[wantIntensify.Count][];
            double[][] p = new double[wantIntensify.Count][];
            double[][] v = new double[wantIntensify.Count][];
            
            for (int time = 0; time < Q; time++)//强化Q次
              for (var k = 0; k< wantIntensify.Count; k++)//
                  {
                      if (time == 0) //第一次强化,设置初始化值
                      {
                        x[k] = (double[]) wantIntensify[k].encoded.Clone();
                        p[k] = (double[]) x[k].Clone();
                        v[k] = new double[V]; 
                      }

                      for (int i = 0; i < V; i++)
                      {
                          var r1 = ran.NextDouble();
                          var r2 = ran.NextDouble();

                          v[k][i] = 0.729 * (v[k][i] + 2.05 * r1 * (p[k][i] - x[k][i]) +
                                             2.05 * r2 * (cBest[i] - x[k][i]));
                          x[k][i] = x[k][i] + v[k][i];
                          if (x[k][i] < 0)
                              x[k][i] = 0;
                          if (x[k][i] > 10)
                              x[k][i] = 10;
                      }
      
                      Chromosome y = new Chromosome((double[]) x[k].Clone());//todo 增加GetFitness()静态方法
      
                      if (y.GetFitness() < cBest.GetFitness())
                          cBest.encoded = y.encoded;
                      if (y.GetFitness() < wantIntensify[k].GetFitness())
                          wantIntensify[k].encoded = (double[]) y.encoded.Clone();
                  }  
        }

        private static double Fitness(int[] decoded)
        {    
            var decodedStr = string.Join(",",decoded);
            if (history.ContainsKey(decodedStr))
            {
                return history[decodedStr];
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
                            B = A.FindAll(j => j.b < i.b + i.l && j.b + j.l > i.b);
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
                        int time = Math.Max(i.ar, i.s);
                        List<Ship> A = finshed.FindAll(j => j.GetD() > i.a && j.b < i.b + i.l && j.b + j.l > i.b);
                        if (time < A.Max(j => j.sr + j.pr))
                            time = A.Max(j => j.sr + j.pr);

                        i.sr = time;
                        f[k] = f[k] + i.sr + i.pr - i.ar;
                    }
                }

                double mean = f.Average();
                double devi = StDev(f);

                history[decodedStr] = mean + λ * devi;

                return mean + λ * devi;
            }
        }

        //生成保留指定上下界的随机数
        public static double NextDouble(double minValue, double maxValue)
        {
            return ran.NextDouble() * (maxValue - minValue) + minValue;
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
        
        class Chromosome:IComparable<Chromosome> ,ICloneable
        {
            public double[] encoded = new double[V];

            public Chromosome()
            {
                for (int i =  0; i < encoded.Length; i++)
                {    
                    encoded[i] = NextDouble(0, 10);
                }
            }
            public Chromosome(double[] encoded)
            {
                this.encoded = (double[]) encoded.Clone();
            }
            
            public double GetFitness()
            {
                return Fitness(GetDecoded());
            }
            
            //解码染色体
            public  int[] GetDecoded()
            {
                double[] ChromosomeSorted = (double[]) encoded.Clone();
                Array.Sort(ChromosomeSorted);

                int[] result = new int[V];
                Dictionary<double,int> lastFind = new Dictionary<double, int>();
                for (int i = 0; i < result.Length; i++)
                {
                    var segment = encoded[i];
                    int last = 0;
                    lastFind.TryGetValue(segment, out last);
                        
                    result[i] =  Array.IndexOf(ChromosomeSorted, encoded[i], last);
                    lastFind[segment] = Array.IndexOf(ChromosomeSorted, encoded[i], last) + 1;
                }

                return result;
            }
            
            //与另一条染色体进行交叉
            public void Cross(Chromosome chromosome)
            {
                int a = ran.Next(V);
                int b = ran.Next(V);

                for (int i = Math.Min(a, b); i < Math.Max(a, b); i++)
                {
                    var temp = encoded[i];
                    encoded[i] = chromosome.encoded[i];
                    chromosome.encoded[i] = temp;
                }
                
            } 
            //变异
            public void Mutation()
            {
                for (int i = 0; i < 3; i++)
                {
                    var p = ran.NextDouble();
                    if (p < pm)
                        encoded[ran.Next(0, V - 1)] = NextDouble(0, 10);
                }
            }
            public int CompareTo(Chromosome other)
            {    
                return GetFitness().CompareTo(other.GetFitness());
            }
            
            public Chromosome Clone()
            {
                Chromosome chromosome = new Chromosome((double[]) encoded.Clone());
                return chromosome;
            }
             object ICloneable.Clone()
             {
                 return Clone();
             }

             public double this[int i]
             {
                 get { return encoded[i]; }
             }
        }

    }
    
    class Ship
    {    private static readonly Random ran = new Random();

        public readonly int a = ran.Next(2016);//到达时间
        public  int ar;//实际到达时间
        public readonly int p = ran.Next(60, 2016);//作业时间
        public  int pr;//实际作业时间
        public readonly int l = ran.Next(10, 15);//长度

        public int b;//停泊位置
        public int s;//开始作业时间
        public int sr;//实际开始作业时间
        
        public int GetD()//离港时间
        {
            return s + p;
        }

        //随机生成船舶实际到达时间ar 和实际作业时间pr ,并计算实际开始作业时间sr
        public void GenR()
        {
            ar = ran.Next(a, a + 30);
            pr = ran.Next(p - 30, p + 30);

            sr = ar + s - a;
        }
    }
}