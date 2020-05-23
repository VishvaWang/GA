using System;

namespace GeneticAlgorithm
{
    public class AlgorithmParameter
    {
        //编译器常量
        public const int V = 20;//船舶数量
        public const int L = 60;//岸线总长度
        public const int T = 2016;//时间总长度
        public const int N = 200;//遗传算法种群数量
        public const int G = 200;//遗传算法执行代数
        public const int M = 10;//仿真实验次数
        public const double λ = 0.1;//适应度函数参数
        public const double pc = 0.4;//遗传算法交叉概率
        public const double pm = 0.1;//遗传算法变异概率
        public const int Q = 20;//强化算法执行代数
        
        //运行期常量
        public static readonly Random ran = new Random();
    }
}