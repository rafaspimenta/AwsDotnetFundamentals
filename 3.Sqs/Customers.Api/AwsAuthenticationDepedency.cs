using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Customers.Api.Messaging;

namespace Customers.Api
{
    public static class AWSAuthenticationDepedency
    {
        public static IServiceCollection AddAWSConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY", EnvironmentVariableTarget.Machine);
            var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY", EnvironmentVariableTarget.Machine);

            var awsConfigSection = configuration.GetSection(QueueSettings.Key);
            services.Configure<QueueSettings>(awsConfigSection);

            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            AWSOptions awsOptions = new()
            {
                Credentials = credentials,
                Region = RegionEndpoint.GetBySystemName(awsConfigSection["region"])
            };
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonSQS>();
            services.AddSingleton<ISqsMessenger, SqsMessenger>();

            return services;
        }
    }
}
