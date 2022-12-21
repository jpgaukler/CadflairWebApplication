using CadflairForgeAccess.Helpers;
using CadflairForgeAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairForgeAccess
{
    public class ForgeServicesManager
    {

        public AuthorizationService AuthorizationService { get; }
        public ObjectStorageService ObjectStorageService { get; }
        public DesignAutomationService DesignAutomationService { get; }
        public ModelDerivativeService ModelDerivativeService { get; }

        public ForgeServicesManager()
        {
            string clientId = Utils.GetAppSetting("FORGE_CLIENT_ID");
            string clientSecret = Utils.GetAppSetting("FORGE_CLIENT_SECRET");

            AuthorizationService = new(clientId, clientSecret);
            ObjectStorageService = new(AuthorizationService);
            DesignAutomationService = new(AuthorizationService);
            ModelDerivativeService = new();
        }




    }
}
