var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFastEndpoints();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

var app = builder.Build();

app.UseFastEndpoints();

app.Run();