using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDataMask.Console.Models;
using OpenDataMask.Console.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args);

        var built = config.Build();
        if (built.GetValue<bool>("AzureKeyVault:Enabled") && !string.IsNullOrWhiteSpace(built["AzureKeyVault:VaultUri"]))
        {
            config.AddAzureKeyVault(new Uri(built["AzureKeyVault:VaultUri"]!), new DefaultAzureCredential());
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<MongoSettings>(context.Configuration.GetSection("MongoSettings"));
        services.AddSingleton<IMaskService, MaskService>();
        services.AddSingleton<IMaskingConfigReader, XmlMaskingConfigReader>();
        services.AddSingleton<IMongoRepository, MongoRepository>();
        services.AddSingleton<IMaskingEngine, MaskingEngine>();
        services.AddLogging(logging => logging.AddConsole());
    })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
var configuration = services.GetRequiredService<IConfiguration>();

try
{
    var inputFile = configuration["InputFile"] ?? configuration["MaskingConfig:InputFile"];
    if (string.IsNullOrWhiteSpace(inputFile))
    {
        throw new InvalidOperationException("InputFile is required. Provide --input-file or add InputFile to appsettings.");
    }

    var reader = services.GetRequiredService<IMaskingConfigReader>();
    var engine = services.GetRequiredService<IMaskingEngine>();
    var maskingConfig = reader.Read(inputFile);

    logger.LogInformation("Starting MongoDB masking workflow with input file {InputFile}.", inputFile);
    await engine.ExecuteAsync(maskingConfig, CancellationToken.None).ConfigureAwait(false);
    logger.LogInformation("MongoDB masking workflow completed successfully.");
}
catch (Exception ex)
{
    logger.LogCritical(ex, "An unhandled error occurred during execution.");
    return 1;
}

return 0;
