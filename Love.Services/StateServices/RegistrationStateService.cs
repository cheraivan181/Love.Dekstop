using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.StateServices
{
    public class RegistrationStateService
    {
        private static RegistrationStateService registrationStateService;
        public static RegistrationStateService GetRegistrationStateService()
        {
            if (registrationStateService == null)
                registrationStateService = new RegistrationStateService();

            return registrationStateService;
        }

        public bool IsLogin = true; 

        public void ChangeState()
        {
            IsLogin = IsLogin ? false : true;
        }
    }
}
