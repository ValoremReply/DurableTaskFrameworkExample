using DurableTask.AzureStorage;
using DurableTask.Core;
using DurableTaskFramework.Example;

var storageConnectionString = "UseDevelopmentStorage=true";
var taskHubName = "TaskHub1";

var orchestrationServiceAndClient = new AzureStorageOrchestrationService(new AzureStorageOrchestrationServiceSettings()
{
    TaskHubName = taskHubName,
    StorageConnectionString = storageConnectionString,
});

await orchestrationServiceAndClient.CreateIfNotExistsAsync();

var taskHubClient = new TaskHubClient(orchestrationServiceAndClient);

var taskHub = new TaskHubWorker(orchestrationServiceAndClient)
    .AddTaskOrchestrations(typeof(SumOfSquaresOrchestration))
    .AddTaskActivities(typeof(SumOfSquaresTask));
await taskHub.StartAsync();

var instanceId = Guid.NewGuid().ToString();
var instanceTask = await taskHubClient.CreateOrchestrationInstanceAsync(
                typeof(SumOfSquaresOrchestration), 
                instanceId, 
                File.ReadAllText("BagOfNumbers.json"));

Console.WriteLine("Waiting up to 60 seconds for completion.");

var taskResult = await taskHubClient.WaitForOrchestrationAsync(instanceTask, TimeSpan.FromSeconds(60));

Console.WriteLine($"Task done: {taskResult?.OrchestrationStatus}");