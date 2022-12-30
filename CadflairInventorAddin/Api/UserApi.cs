using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Api
{
    internal static class UserApi
    {
        public static async Task<User> GetUserByObjectIdentifier(string objectIdentifier)
        {
            try
            {
                string endPoint = $"/api/user/get/{objectIdentifier}";
                string result = await Client.Get(endPoint);
                User user  = JsonConvert.DeserializeObject<User>(result);
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetUserByObjectIdentifier", objectIdentifier);
                return null;
            }
        }

    }
}
