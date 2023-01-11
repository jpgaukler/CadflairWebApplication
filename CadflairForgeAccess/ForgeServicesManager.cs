using CadflairForgeAccess.Helpers;
using CadflairForgeAccess.Services;
using Microsoft.Extensions.Configuration;
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

        public ForgeServicesManager(IConfiguration configuration)
        {
            //string clientId = Utils.GetAppSetting("FORGE_CLIENT_ID");
            //string clientSecret = Utils.GetAppSetting("FORGE_CLIENT_SECRET");
            string clientId = configuration["ForgeCredentials:ClientId"];
            string clientSecret = configuration["ForgeCredentials:Secret"];

            AuthorizationService = new(clientId, clientSecret);
            ObjectStorageService = new(AuthorizationService);
            DesignAutomationService = new(AuthorizationService, ObjectStorageService);
            ModelDerivativeService = new(AuthorizationService, ObjectStorageService);
        }




    }
}
