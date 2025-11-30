using System;
using System.Security.Cryptography;

Console.WriteLine("Generador de Clave Secreta para JWT");
Console.WriteLine("-----------------------------------");

// HMAC-SHA512 necesita una clave de al menos 512 bits.
// 512 bits / 8 bits por byte = 64 bytes.
// Generaremos 64 bytes de datos aleatorios.
var randomNumber = new byte[64];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(randomNumber);
}

// Convertimos los bytes aleatorios a una cadena de texto segura (Base64)
string secretKey = Convert.ToBase64String(randomNumber);

Console.WriteLine("\n¡Clave generada!");
Console.WriteLine("Esta clave tiene la longitud correcta y es criptográficamente segura.");
Console.WriteLine("\nCópiala y pégala en tu appsettings.json:");
Console.WriteLine("==================================================");
Console.WriteLine(secretKey);
Console.WriteLine("==================================================");


Console.WriteLine("\nPresiona cualquier tecla para salir.");
Console.ReadKey();
