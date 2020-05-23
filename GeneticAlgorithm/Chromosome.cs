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
            {
                this.encoded = (double[]) encoded.Clone();
            }

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
            public static List<Chromosome> Copy(List<Chromosome> chromosomeList)
            {
                return chromosomeList.Select(chromosome => chromosome.Clone()).ToList();
            }
            
            public static void Mutation(List<Chromosome> waitMutation)
            {    //排序,便于找出cBest 
                waitMutation.Sort();
                //对每个染色体执行变异
                waitMutation
                    .Skip(1)// 跳过第一个,即 cBest
                    .ToList()
                    .ForEach(chromosome => chromosome.Mutation());
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

            public static List<Chromosome> Select(List<Chromosome> chromosomes)
        {    
            List<Chromosome> selected = new List<Chromosome>();
            selected.Add(chromosomes.Min().Clone());
            selected.Add(chromosomes.Min().Clone());

            while (selected.Count < chromosomes.Count)
                selected.Add(chromosomes[Math.Min(ran.Next(0, chromosomes.Count - 1), ran.Next(0, chromosomes.Count - 1))]);

            return selected;
        }

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