namespace otel;

using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

public class DynamoRepo
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly ILogger<DynamoRepo> _logger;

    public DynamoRepo(IAmazonDynamoDB dynamoDb, ILogger<DynamoRepo> logger)
    {
        _dynamoDb = dynamoDb;
        _logger = logger;
    }
    
    public async Task StoreDiceRoll(string? player, int result, CancellationToken cancellationToken)
    {
        PutItemRequest request = new()
        {
            TableName = "RollHistory",
            Item = new Dictionary<string, AttributeValue>()
            {
                { "Date", new AttributeValue {S = DateTime.UtcNow.ToString("O")}},
                { "Player", new AttributeValue {S = player}},
                { "Result", new AttributeValue {N = result.ToString(CultureInfo.InvariantCulture)}}
            }
        };

        try
        {
            var response = await _dynamoDb.PutItemAsync(request, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to store dice roll");
        }
    }
}
