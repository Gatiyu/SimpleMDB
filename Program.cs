namespace SimpleMDB;
using System.Threading.Tasks;
using System;
public class Program
{
    public static async Task Main()
    {
        App app = new App();
        await app.Start();
    }
}