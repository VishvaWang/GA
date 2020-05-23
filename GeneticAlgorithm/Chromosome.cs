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
            public double GetFitness()
            {
                return MainAlgorithm.Fitness(GetDecoded());
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