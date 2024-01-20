using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging
{
    public class SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings) : ISqsMessenger
    {
        private readonly IAmazonSQS _sqs = sqs;
        private readonly IOptions<QueueSettings> _queueSettings = queueSettings;

        private string? _queueUrl;

        public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = await GetQueueUrlAsync(),
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue> {
                    {
                        "MessageType", new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = typeof(T).Name
                        }
                    }
                }
            };

            return await _sqs.SendMessageAsync(sendMessageRequest);
        }

        private async Task<string> GetQueueUrlAsync()
        {
            if (!string.IsNullOrWhiteSpace(_queueUrl))
            {
                return _queueUrl;
            }

            var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name);
            _queueUrl = queueUrlResponse.QueueUrl;
            return _queueUrl;
        }
    }
}
