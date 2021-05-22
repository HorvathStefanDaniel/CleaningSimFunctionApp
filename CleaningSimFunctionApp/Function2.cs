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
    public static class Function2
    {
        private static int TryParseString(string value)
        {
            int parsed = 0;
            try
            {
                parsed = Int32.Parse(value);
            }
            catch (Exception e)
            {
                throw e;
            }

            return parsed;
        }
       
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string s_u_id = req.Query["u_id"];

            string s_timeSeconds = req.Query["time_seconds"];

            string s_mistakes_count = req.Query["mistakes_count"];

            string s_level_name = req.Query["level_name"];

            int u_id;
            int timeSeconds;
            int mistakes_count;


            ////////SQL CONNECTION
            SqlCommand com = new SqlCommand();
            SqlDataReader dr;
            SqlConnection con = new SqlConnection();
            string conString = "Server=tcp:cleaning-sim-server.database.windows.net,1433;Initial Catalog=cleaning-sim;Persist Security Info=False;User ID=Stefan;Password=12344321Asd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            con.ConnectionString = conString;
            ///////

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            s_u_id = s_u_id ?? data?.name;
            s_timeSeconds = s_timeSeconds ?? data?.name;
            s_mistakes_count = s_mistakes_count ?? data?.name;
            s_level_name = s_level_name ?? data?.name;



            if (string.IsNullOrEmpty(s_u_id) || string.IsNullOrEmpty(s_timeSeconds) || string.IsNullOrEmpty(s_mistakes_count) || string.IsNullOrEmpty(s_level_name))
            {
                return new OkObjectResult("One of the inputs is empty");
            }

            else //must convert to int
            {

                u_id = TryParseString(s_u_id);

                timeSeconds = TryParseString(s_timeSeconds);

                mistakes_count = TryParseString(s_mistakes_count);

            }

            try
            {
                con.Open();

                com.Connection = con;

                com.CommandText = "INSERT INTO [dbo].[Report]([user_id],[time_seconds],[mistakes_count],[level_name]) " +
                    "VALUES("+u_id+", "+timeSeconds+", "+mistakes_count+", '"+s_level_name+"')";

                dr = com.ExecuteReader();
                con.Close();
                return new OkObjectResult("Report has been inserted");
            }
            catch (Exception e)
            {
                throw e;
            }
            

        }
    }
}
