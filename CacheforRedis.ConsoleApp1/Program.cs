using System.Data;
using System.Net.NetworkInformation;
using StackExchange;
using StackExchange.Redis;

namespace CacheforRedis.ConsoleApp1
{
    internal class Program
    {
        static string connectionString = "DemoRedisACF.redis.cache.windows.net:6380,password=KEY,ssl=True,abortConnect=False";

        static void Main(string[] args)
        {
            Console.Clear();

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(connectionString);
            IDatabase db = connection.GetDatabase();

            // Comprobar conectividad con la base de datos
            RedisResult result = db.Execute("ping");
            Console.WriteLine($"PING {result.ToString()}");

            // Comprobación de la latencia
            Console.WriteLine($"Tiempo de respuesta: {db.Ping()}");

            // Crear una clave con un valor inicial
            Console.WriteLine($"SET demo:nombre 'Alvaro' = {db.StringSet("demo:nombre", "Alvaro")}");
            Console.WriteLine($"SET demo:numero 5 = {db.StringSet("demo:numero", 5)}");
            RedisResult result1 = db.Execute("set demo:nombre 'Alvaro'");

            // Leer una clave
            Console.WriteLine($"GET demo = {db.StringGet("demo")}");
            Console.WriteLine($"GET demo:nombre = {db.StringGet("demo:nombre")}");
            Console.WriteLine($"GET demo:apellidos = {db.StringGet("demo:apellidos")}");

            // Leer una clave e incrementar el valor
            Console.WriteLine($"INCR demo:numero = {db.StringIncrement("demo:numero")}");
            Console.WriteLine($"INCR demo:numero = {db.StringIncrement("demo:numero")}");

            Console.WriteLine($"INCRBY demo:numero -30 = {db.StringIncrement("demo:numero", -30)}");

            // Renombrar una clave
            Console.WriteLine($"REN demo:numero -> demo:nuevo = {db.KeyRename("demo:numero", "demo:nuevo")}");
            Console.WriteLine($"GET demo:numero = {db.StringGet("demo:numero")}");
            Console.WriteLine($"GET demo:nuevo = {db.StringGet("demo:nuevo")}");

            // Buscar claves
            Console.WriteLine($"SET demo2:nombre 'Antonio' = {db.StringSet("demo2:nombre", "Antonio")}");
            Console.WriteLine($"SET demo:numero 5 = {db.StringSet("demo:numero", 5)}");
            Console.WriteLine($"SET demo2:numero 15 = {db.StringSet("demo2:numero", 15)}");
            Console.WriteLine($"SET demo3:nombre 'Ana' = {db.StringSet("demo3:nombre", "Ana")}");

            Console.WriteLine($"KEYS *nombre* = {db.Execute("keys", "*nombre*")}");
            Console.WriteLine($"KEYS *demo2* = {db.Execute("keys", "*demo2*")}");

            var keys = connection.GetServer("DemoRedisACF.redis.cache.windows.net:6380").Keys(pattern: "demo2");
            foreach(var key in keys) 
            { 
                var data = db.StringGet(key);
                Console.WriteLine($"GET {key} = {data}");
            }

            // Borrar una clave
            Console.WriteLine($"DEL demo2:numero = {db.KeyDelete("demo2:numero")}");
            Console.WriteLine($"GET demo2:numero = {db.StringGet("demo2:numero")}");

            // Establecer un TTL para una clave
            Console.WriteLine($"GET demo2:nombre = {db.StringGet("demo2:nombre")}");
            Console.WriteLine($"EXPIRE demo2:nombre 30 = {db.KeyExpire("demo2:nombre", TimeSpan.FromSeconds(10))}");
            Console.WriteLine($"TTL demo2:nombre = {db.KeyTimeToLive("demo2:nombre")}");
            Console.WriteLine($"GET demo2:nombre = {db.StringGet("demo2:nombre")}");
            Thread.Sleep(5000);
            Console.WriteLine($"TTL demo2:nombre = {db.KeyTimeToLive("demo2:nombre")}");
            Thread.Sleep(11000);
            Console.WriteLine($"GET demo2:nombre = {db.StringGet("demo2:nombre")}");

            // Eliminar todas las claves
            Console.WriteLine($"FLUSHDB = {db.Execute("flushdb")}");
        }
    }
}
