using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Service
{
    public class DataService : IDataService
    {
        public DataService()
        {

        }
        public List<int> GetData()
        {
            return new List<int>
            {
                1,2,3
            };
        }
    }
}