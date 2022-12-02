using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Service
{
    public interface IDataService
    {
        List<int> GetData();
    }
}
