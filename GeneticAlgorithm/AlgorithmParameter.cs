using System;

namespace GeneticAlgorithm
{
    public class AlgorithmParameter
    {
        //编译器常量
        public const int V = 20;//船舶数量
        public const int L = 60;//岸线总长度
        public const int T = 300;//时间总长度
        public const int N = 200;//遗传算法种群数量 必须为偶数!
        public const int G = 200;//遗传算法执行代数
        public const int M = 10;//仿真实验次数
        public const double λ = 0.1;//适应度函数参数
        public const double pc = 0.4;//遗传算法交叉概率
        public const double pm = 0.1;//遗传算法变异概率
        public const int Q = 20;//强化算法执行代数
        
        public const int ArrivalTimeUpper = 250;//船舶到达时间上界
        public const int ProductionTimeUpper = 48;//作业时间上界
        public const int ProductionTimeLower = 10;//作业时间下界
        
        public const int RealArrivalTimeUpper = ArrivalTimeUpper + 5;//实际船舶到达时间上界
        public const int RealProductionTimeUpper = ProductionTimeUpper + 5;//实际作业时间上界
        public const int RealProductionTimeLower = ProductionTimeLower -5;//实际作业时间下界
        
        public const int MaxGenerationNoImproved = 10;//
        public const int NoImprovedCriticalCondition = 1;// 小于等于此数值将认为没有改进
        

        //运行期常量
        public static readonly Random ran = new Random();
    }
}