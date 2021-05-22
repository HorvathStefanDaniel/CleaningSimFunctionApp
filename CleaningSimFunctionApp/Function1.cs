using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CleaningSimFunctionApp
{
    public static class Function1
    {
       
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string s_id= req.Query["id"];
            int id = 0;

            ////////SQL CONNECTION
            SqlCommand com = new SqlCommand();
            SqlDataReader dr;
            SqlConnection con = new SqlConnection();
            string conString = "Server=tcp:cleaning-sim-server.database.windows.net,1433;Initial Catalog=cleaning-sim;Persist Security Info=False;User ID=Stefan;Password=12344321Asd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            con.ConnectionString = conString;
            ///////

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            s_id = s_id ?? data?.name;

            if (string.IsNullOrEmpty(s_id))
            {
                return new OkObjectResult("No id has been sent");
            }
            else
            {
                try
                {
                    id = Int32.Parse(s_id);
                }
                catch (FormatException)
                {
                    return new OkObjectResult("Id contains invalid characters");
                }
            }

            //connect to db and try to insert new id if it doesn't already exist.
            try
            {
                con.Open();

                com.Connection = con;

                com.CommandText = "SELECT TOP(1)[user_id] FROM[dbo].[User_Table] WHERE[user_id] =" + id + ";";
                dr = com.ExecuteReader();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (dr.Read()) //this id is already present in the db so we don't need to add it
            {
                con.Close();
                return new OkObjectResult("Id is good");
            }
            try
            {
                com.CommandText = "INSERT INTO dbo.User_Table (user_id) VALUES (" + id + "); ";
                dr.Close();
                dr = com.ExecuteReader();
                con.Close();
                return new OkObjectResult("Id has been inserted");
            }
            catch (Exception e)
            {
                throw e;
            }
            

        }
    }
}
