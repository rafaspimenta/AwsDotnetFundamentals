using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace SqsConsumer;

internal class Program
{
    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();

        var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY", EnvironmentVariableTarget.Machine);
        var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY", EnvironmentVariableTarget.Machine);

        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        var sqsClient = new AmazonSQSClient(credentials, RegionEndpoint.USEast2);

        var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

        var receiveMessageRequest = new ReceiveMessageRequest()
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = ["All"],
            MessageAttributeNames = ["All"]
        };

        while (!cts.IsCancellationRequested)
        {
            var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

            foreach (var message in response.Messages)
            {
                Console.WriteLine($"Message ID: {message.MessageId}");
                Console.WriteLine($"Message Body: {message.Body}");
                await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, cts.Token);
            }

            await Task.Delay(1000);

        }
    }
}
