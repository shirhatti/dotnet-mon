using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_mon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientStream = new NamedPipeClientStream("dotnetmon");
            clientStream.Connect();
            Console.WriteLine("Connected to named pipe");
            while (true)
            {
                var buffer = new byte[100];
                await clientStream.ReadAsync(buffer, 0, buffer.Length);
                Console.WriteLine(Encoding.Unicode.GetString(buffer));
            }
        }
    }
}
