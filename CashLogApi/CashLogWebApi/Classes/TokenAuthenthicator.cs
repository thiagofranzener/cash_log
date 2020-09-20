using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using System.Security.Principal;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Data;

namespace CashLogWebApi.Classes
{
    public class TokenAuthenthicator : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization == string.Empty)
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            else
            {
                var scheme = actionContext.Request.Headers.Authorization.Scheme;
                var parameter = actionContext.Request.Headers.Authorization.Parameter;
                string auth = "";

                if (scheme != null)
                    auth = scheme;
                if (parameter != null)
                    auth = parameter;
                if (auth.Length != 36 && !auth.Split("-").Length == 4)
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                else
                    using (ApiConnection apiConnection = new ApiConnection())
                    {
                        SqlConnection connection = apiConnection.GetConnection();
                        CashLogLib.UserRepository userRepository = new CashLogLib.UserRepository(connection);
                        Guid authenticationToken = new Guid(auth);
                        SqlCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT username FROM CompanyUser WHERE token=@token";
                        command.Parameters.AddWithValue("@token", authenticationToken);
                        using (IDataReader dr = command.ExecuteReader())
                        {
                            if (!dr.Read)
                                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            else
                            {
                                GenericIdentity identity = new GenericIdentity(dr("username"));
                                HttpContext.Current.User = new GenericPrincipal(identity, null);
                            }
                        }
                        connection.Close();
                    }
            }
            base.OnAuthorization(actionContext);
        }
    }

}