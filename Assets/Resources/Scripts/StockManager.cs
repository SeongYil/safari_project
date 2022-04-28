using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//stock 이미지 리소스 관리 


namespace Assets.Resources.Scripts
{
    public class StockManager
    {

        static public StockManager instance;

        private StockManager()
        {

        }

        static public void CreateManager()
        {
            instance = new StockManager();
        }



    }
}
