﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Utils
{
    public static class Urls
    {
        public static string AuthUrl = "Account/signin";
        public static string LongSessionUpdateUrl = "Account/longsessionupdate";
        public static string RegisterUrl = "Account/signup";
        public static string ConfirmPhoneTestUrl = "Tester/confirmPhone";

        public static string GetAuthUserInfoUrl = "Account/getauthuserinfo";
    }
}
