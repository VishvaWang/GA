using System;
using System.Collections.Generic;
using static  GeneticAlgorithm.AlgorithmParameter;

namespace GeneticAlgorithm
{
    class Ship
    {    private static readonly Random ran = new Random();

        public readonly int a = ran.Next(ArrivalTimeUpper);//到达时间
        public  int ar;//实际到达时间
        public readonly int p = ran.Next(ProductionTimeLower, ProductionTimeUpper);//作业时间
        public  int pr;//实际作业时间
        public readonly int l = ran.Next(10, 15);//长度

        public int b;//停泊位置
        public int s;//开始作业时间
        public int sr;//实际开始作业时间
        
        public static List<Ship> InitShips()
        {    List<Ship> ships = new List<Ship>();
            //生成V艘船并存入列表
            for (int i = 0; i < V; i++)
                ships.Add(new Ship());

            return ships;
        }
        public int GetD()//离港时间
        {
            return s + p;
        }

        //随机生成船舶实际到达时间ar 和实际作业时间pr ,并计算实际开始作业时间sr
        public void GenR()
        {
            ar = ran.Next(a, RealArrivalTimeUpper);
            pr = ran.Next(RealProductionTimeLower, RealArrivalTimeUpper);

            sr = s;
        }
    }
}