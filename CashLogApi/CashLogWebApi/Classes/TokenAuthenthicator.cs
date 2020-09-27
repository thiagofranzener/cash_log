using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using CashLogLib.Repositories;

namespace CashLogWebApi.Classes
{
    public partial class TokenAuthenthicator : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization is null || object.ReferenceEquals(actionContext.Request.Headers.Authorization, string.Empty))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                var scheme = actionContext.Request.Headers.Authorization.Scheme;
                var parameter = actionContext.Request.Headers.Authorization.Parameter;
                string auth = "";
                if (scheme is object)
                    auth = scheme;
                if (parameter is object)
                    auth = parameter;
                if (auth.Length != 36 && !(auth.Split('-').Length == 4))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    using (var apiConnection = new ApiConnection())
                    {
                        SqlConnection connection = apiConnection.GetConnection();
                        var userRepository = new CashLogApi.UserRepository(connection);
                        var authenticationToken = new Guid(auth);
                        var command = connection.CreateCommand();
                        command.CommandText = " SELECT username FROM CompanyUser WHERE token = @token ";
                        command.Parameters.AddWithValue("@token", authenticationToken);
                        using (IDataReader dr = command.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            }
                            else
                            {
                                var identity = new GenericIdentity((string)dr["username"]);
                                HttpContext.Current.User = new GenericPrincipal(identity, null);
                            }
                        }

                        connection.Close();
                    }
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}