Console.WriteLine("Generador de Hashes BCrypt");
Console.WriteLine("--------------------------");
Console.WriteLine("Introduce la contraseña a hashear (ejemplo: 123):");

string password = Console.ReadLine();

// Generamos el hash con el factor de trabajo por defecto
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

Console.WriteLine("\n¡Hash generado!");
Console.WriteLine(hashedPassword);

Console.WriteLine("\nCopia este hash y pégalo en tu script de SQL para la columna PasswordHash.");
Console.WriteLine("Presiona cualquier tecla para salir.");
Console.ReadKey();
