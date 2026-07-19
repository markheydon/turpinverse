var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Turpinverse_Web>("turpinverse-web");

builder.Build().Run();
