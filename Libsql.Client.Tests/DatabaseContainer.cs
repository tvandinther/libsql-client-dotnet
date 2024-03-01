using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Libsql.Client.Tests;

public class DatabaseContainer : IDisposable
{
    public DatabaseContainer()
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        Container = new ContainerBuilder()
            .WithImage("tvandinther/turso:v0.87.7")
            .WithCommand("dev", "--db-file", "/data/chinook.db")
            .WithResourceMapping("chinook.db", "/data")
            .WithPortBinding(8080, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                .ForPort(8080)
                .ForPath("/health")
                .ForStatusCode(HttpStatusCode.OK)
            ))
            .Build();
    }

    public IContainer? Container { get; }

    public async void Dispose()
    {
        if (Container is not null)
            await Container.DisposeAsync();
    }

}