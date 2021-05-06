using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Dekstop.Services.Crypt
{
    public interface IRsaCryptService
    {
        string Test();
    }
    public class RsaCryptService : IRsaCryptService
    {
        public string Test()
        {
            return "test";
        }
    }
}
