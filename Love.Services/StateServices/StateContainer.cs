using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Services.StateServices
{
    public class StateContainer
    {
        private static StateContainer stateContainer;

        public static StateContainer GetStateContainer()
        {
            if (stateContainer == null)
            {
                stateContainer = new StateContainer();
                stateContainer.registrationStateService = RegistrationStateService.GetRegistrationStateService();
                stateContainer.sessionStateService = SessionStateService.GetSessionStateService();
            }
            return stateContainer;
        }


        public RegistrationStateService registrationStateService;
        public SessionStateService sessionStateService;
    }
}
