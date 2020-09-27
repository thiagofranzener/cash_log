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

namespace CashLogWebApi.Classes
{
    public class ApiConnection : System.IDisposable
    {
        private readonly System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection("");
        // Server=localhost;Database=CashLog;User Id=movtech;Password=mvt;Connection Timeout=30
        // 192.168.254.202,1433

        public ApiConnection()
        {
            connection.Open();
        }

        public System.Data.SqlClient.SqlConnection GetConnection()
        {
            return connection;
        }

        public System.Data.SqlClient.SqlCommand GetCommand()
        {
            System.Data.SqlClient.SqlCommand command = connection.CreateCommand();
            command.Connection = connection;
            return command;
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}