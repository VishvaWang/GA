using System;
using System.Collections.Generic;
using System.Linq;
using static  GeneticAlgorithm.AlgorithmParameter;

namespace GeneticAlgorithm
{
     class Chromosome:IComparable<Chromosome> ,ICloneable
        {    
            public double[] encoded = new double[V];

            public Chromosome()
            {
                for (int i =  0; i < encoded.Length; i++)
                {    
                    encoded[i] = MyMaths.NextDouble(0, 10);
                }
            }
            public Chromosome(double[] encoded)
            {    //需要克隆
                this.encoded = (double[]) encoded.Clone();
            }

            //初始化N条染色体
            public static List<Chromosome> initChromsomes()
            {    var chromosomes = new List<Chromosome>();
                //生成初始染色体
                for (var i = 0; i < N; i++)
                    chromosomes.Add(new Chromosome());
            
                //手动生成最后一条染色体
                var shipsOrderByA = MainAlgorithm.ships.OrderBy(s => s.a).ToList();
                for (int i = 0; i < MainAlgorithm.ships.Count; i++)
                    chromosomes.Last().encoded[i] = (shipsOrderByA.IndexOf(MainAlgorithm.ships[i]) + 1) / 2.0;

                return chromosomes;
            }
            
            //对染色体列表进行深度克隆
            public static List<Chromosome> Copy(List<Chromosome> chromosomeList)
            {
                return chromosomeList.Select(chromosome => chromosome.Clone()).ToList();
            }
            
            //变异
            public static void Mutation(List<Chromosome> waitMutation)
            {
                Console.Out.WriteLine(waitMutation.Min().GetFitness());
                //排序,便于找出cBest 
                waitMutation.Sort();
                var a = waitMutation.Min();

                for (int i = 1; i < waitMutation.Count - 1; i++)
                {
                    var b = System.Object.ReferenceEquals(a, waitMutation[i]);
                }
                
                
                //对每个染色体执行变异
                waitMutation
                    .Skip(1)// 跳过第一个,即 cBest
                    .ToList()
                    .ForEach(chromosome => chromosome.Mutation());
                
                Console.Out.WriteLine(waitMutation.Min().GetFitness());

            }
            //交叉
            public static void Crossover(List<Chromosome> waitCrossover, List<Chromosome> generation)
            {
                //cBest 不参与交叉
                waitCrossover.Sort();
                var cBest = waitCrossover.First();
                waitCrossover.RemoveAt(0);
            
                //需要交叉的染色体序号
                List<int> needChromosomeIndexs = new List<int>();
                for (int i = 0; i < waitCrossover.Count - 1; i++)
                {
                    double p = MyMaths.NextDouble(0, 1);
                    if (p < pc)
                        needChromosomeIndexs.Add(i);
                }
                
                //交叉数量不为偶数
                if (needChromosomeIndexs.Count % 2 != 0)
                    needChromosomeIndexs.Add(waitCrossover.Count - 1);
                
                //进行交叉,直至全部交叉完成
                while (needChromosomeIndexs.Count != 0)
                {
                    var index1 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                    var index2 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                
                    Chromosome chromosome = waitCrossover[index1];
                    chromosome.Cross(waitCrossover[index2]);
                
                    needChromosomeIndexs.Remove(index1);
                    needChromosomeIndexs.Remove(index2);
                }
                
                //加入cBest
                waitCrossover.Add(cBest);
            }
            public static void Crossover(List<Chromosome> waitCrossover)
            {
                //cBest 不参与交叉
                waitCrossover.Sort();
                var cBest = waitCrossover.First();
                waitCrossover.RemoveAt(0);
            
                //需要交叉的染色体序号
                List<int> needChromosomeIndexs = new List<int>();
                for (int i = 0; i < waitCrossover.Count - 1; i++)
                {
                    double p = MyMaths.NextDouble(0, 1);
                    if (p < pc)
                        needChromosomeIndexs.Add(i);
                }
                
                //交叉数量不为偶数
                if (needChromosomeIndexs.Count % 2 != 0)
                    needChromosomeIndexs.Add(waitCrossover.Count - 1);
                
                //进行交叉,直至全部交叉完成
                while (needChromosomeIndexs.Count != 0)
                {
                    var index1 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                    var index2 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                
                    Chromosome chromosome = waitCrossover[index1];
                    chromosome.Cross(waitCrossover[index2]);
                
                    needChromosomeIndexs.Remove(index1);
                    needChromosomeIndexs.Remove(index2);
                }

                var a = 0;
                //加入cBest
                waitCrossover.Add(cBest);
            }
            
            public static void Crossover1(List<Chromosome> waitCrossover)
            {
                //cBest 不参与交叉
                waitCrossover.Sort();
                var cBest = waitCrossover.First();
                waitCrossover.RemoveAt(0);
            
                //需要交叉的染色体序号
                List<int> needChromosomeIndexs = new List<int>();
                for (int i = 0; i < waitCrossover.Count - 1; i++)
                {
                    double p = MyMaths.NextDouble(0, 1);
                    if (p < pc1)
                        needChromosomeIndexs.Add(i);
                }
                
                //交叉数量不为偶数
                if (needChromosomeIndexs.Count % 2 != 0)
                    needChromosomeIndexs.Add(waitCrossover.Count - 1);
                
                //进行交叉,直至全部交叉完成
                while (needChromosomeIndexs.Count != 0)
                {
                    var index1 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                    var index2 = needChromosomeIndexs[ran.Next(needChromosomeIndexs.Count)];
                
                    Chromosome chromosome = waitCrossover[index1];
                    chromosome.Cross(waitCrossover[index2]);
                
                    needChromosomeIndexs.Remove(index1);
                    needChromosomeIndexs.Remove(index2);
                }
                
                //加入cBest
                waitCrossover.Add(cBest);
            }
            //选择
            public static List<Chromosome> Select(List<Chromosome> chromosomes)
            {    
                //加入两条best
                List<Chromosome> selected = new List<Chromosome>();
                selected.Add(chromosomes.Min().Clone());
                selected.Add(chromosomes.Min().Clone());
                
                //随机进行选择
                while (selected.Count < chromosomes.Count)
                    selected.Add(chromosomes[Math.Min(ran.Next(0, chromosomes.Count - 1), ran.Next(0, chromosomes.Count - 1))].Clone());

                return selected;
            }
            
            //强化
            public static void Intensify(List<Chromosome> wantIntensify, Chromosome cBest)
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
            public double GetFitness()
            {
                return MyMaths.Fitness(GetDecoded());
            }
            
            //解码染色体
            public  int[] GetDecoded()
            {    

                //先将染色体排序 

                double[] ChromosomeSorted = (double[]) encoded.Clone();

                Array.Sort(ChromosomeSorted);

                

                //存放船舶序列

                int[] shipsSeq = new int[V];

                

                //用来存放某个数字上次出现的位置,如果上次已经出现过,就从上次出现过的位置 + 1 开始寻找

                Dictionary<double,int> numLastOccurrenceIndex = new Dictionary<double, int>();

                

                for (int I = 0; I < shipsSeq.Length; I++)

                {    

                    //当前基因片段数字

                    var segmentNum = encoded[I];

                    

                    //默认从0开始寻找

                    int startIndex = 0;

                    //如果字典中已经存有上次的值,将startIndex设为上次的值

                    numLastOccurrenceIndex.TryGetValue(segmentNum, out startIndex);

                      

                    //设定对应基因片段的船舶序列

                    shipsSeq[I] =  Array.IndexOf(ChromosomeSorted, encoded[I], startIndex);

                    //更新这个基因片段数字下次寻找时的开始位置

                    numLastOccurrenceIndex[segmentNum] = Array.IndexOf(ChromosomeSorted, encoded[I], startIndex) + 1;

                }



                return shipsSeq;

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
                        encoded[ran.Next(0, V - 1)] = MyMaths.NextDouble(0, 10);
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