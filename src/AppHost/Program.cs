var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
               .WithDataVolume();
var chat = ollama.AddModel("chat", "phi3");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(chat)
    .WaitFor(chat);

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
