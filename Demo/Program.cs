using LibsqlClient;

var cls = new Class1();
Console.WriteLine(cls.Name);
Console.WriteLine("64-bit: {0}", Environment.Is64BitProcess ? "Yes" : "No");
cls.PrintViaRust("Hello from C#!");