using Love.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.Base
{
    public class BaseService
    {
        protected UserProvider userProvider = new UserProvider();
    }
}
