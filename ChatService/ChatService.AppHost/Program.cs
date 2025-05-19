var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ChatService>("chatservice");

builder.Build().Run();
