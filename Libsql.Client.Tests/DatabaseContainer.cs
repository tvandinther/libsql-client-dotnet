using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Libsql.Client.Tests;

public class DatabaseContainer : IDisposable
{
    public DatabaseContainer()
    {
        Container = new ContainerBuilder()
            .WithImage("tvandinther/turso:v0.87.7")
            .WithCommand("dev", "--db-file", "/data/chinook.db")
            .WithResourceMapping("chinook.db", "/data")
            .WithPortBinding(8080, true)
            .Build();
    }

    public IContainer Container { get; }

    public async void Dispose()
    {
        await Container.DisposeAsync();
    }

}