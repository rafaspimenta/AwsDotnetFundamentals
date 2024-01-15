using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace _3.Sqs;

internal class Program
{
    static async Task Main(string[] args)
    {
        var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY", EnvironmentVariableTarget.Machine);
        var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY", EnvironmentVariableTarget.Machine);

        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

        var sqsClient = new AmazonSQSClient(credentials, RegionEndpoint.USEast2);

        var customer = new CustomerCreated()
        {
            Id = Guid.NewGuid(),
            Email = "rafa.spimenta@gmail.com",
            FullName = "Rafael Pimenta",
            DateOfBirth = new DateTime(1986, 6, 12),
            GitHubUsername = "rafaspimenta"
        };

        var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

        var sendMessageRequest = new SendMessageRequest()
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            MessageBody = JsonSerializer.Serialize(customer),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue()
                    {
                        DataType = "String",
                        StringValue = nameof(CustomerCreated)
                    }

                }
            }
        };

        var response = await sqsClient.SendMessageAsync(sendMessageRequest);

        Console.WriteLine(response.ToString());

    }
}
