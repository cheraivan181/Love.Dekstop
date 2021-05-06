using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Models.Responses
{
    public class AuthUserInfo
    {
        public string Login { get; set; }
        public List<string> Roles { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
