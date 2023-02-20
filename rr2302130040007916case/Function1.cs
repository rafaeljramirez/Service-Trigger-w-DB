using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace rr2302130040007916case
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        //private ServiceBusProcessor processor;

        public Function1(ILogger<Function1> log)
        {
            _logger = log;
        }

        [FunctionName("Function1")]
        public static async Task Run([ServiceBusTrigger("rr-topic", "rr-sub", Connection = "SBconnectionstring")] string mySbMsg, string messageId, ILogger _logger)
        {
            {
                try
                {
                    var messageBody = mySbMsg;

                    // Perform some processing on the message data
                    // ...

                    _logger.LogInformation($"Processed message {mySbMsg}");
                    _logger.LogInformation($"MessageId={messageId}");

                    // Insert the message data into a SQL database
                    using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                    {
                        await connection.OpenAsync();

                        using (var command = new SqlCommand("INSERT INTO dbo.Message (messagestring) VALUES (@messagestring)", connection))
                        {
                            command.Parameters.AddWithValue("@messagestring", messageBody);

                            await command.ExecuteNonQueryAsync();
                        }
                        _logger.LogInformation("Processed message into SQL database");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            }

        }
    }
}