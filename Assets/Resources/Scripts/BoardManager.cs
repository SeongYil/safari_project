using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//보드판 상태 관리함
namespace Assets.Resources.Scripts
{
    public class BoardManager
    {
        public static int[,] InitAnimalBoard = null;
        public static int[,] InitStocks = null;

        static public void InitializeBoard()
        {
            //추후 보드매니저에서 생성할 수 있게 수정 
            InitAnimalBoard = new int[Environment.Y, Environment.X]
            {
                { Environment.G2,   Environment.L2,     Environment.E2},
                { 0,                Environment.P2,     0 },
                { 0,                Environment.P1,     0 },
                { Environment.E1,   Environment.L1,     Environment.G1}
            };


            InitStocks = new int[(int)SharedDataType.EColor.Count, (int)SharedDataType.EStockType.Count];
            //흰색 코끼리 
            InitStocks[0, 0] = 0;

            //흰색 기린
            InitStocks[0, 1] = 0;

            //흰색 병아리 
            InitStocks[0, 2] = 0;

            //검은색 코끼리
            InitStocks[1, 0] = 0;

            //검은색 기린 
            InitStocks[1, 1] = 0;

            //검은색 병아리
            InitStocks[1, 2] = 0;




        }
    }
}
