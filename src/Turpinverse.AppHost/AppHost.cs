var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREDOTNETPROJECT001
builder.AddDotnetProject("turpinverse-web", "../Turpinverse.Web/Turpinverse.Web.csproj");
#pragma warning restore ASPIREDOTNETPROJECT001

builder.Build().Run();
