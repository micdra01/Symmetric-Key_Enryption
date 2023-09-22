// See https://aka.ms/new-console-template for more information

using System.Text;

byte[] binary = null;
string enteredText = "No message has been stored yet";
bool isA, isB, isC;
string? password = null;

Login();

void Login()
{
    Console.WriteLine("Passphrase:");
    password = Console.ReadLine();
    
    if (password.Equals("pw"))
    {
        RunProgram();
    }
    else
    {
        Console.WriteLine("Wrong password. (hint, it's 'pw')");
        Login();
    }
}

void RunProgram()
{
    Console.WriteLine("------------------------------------------");
    Console.WriteLine("A: Safely store message");
    Console.WriteLine("B: Read message");
    Console.WriteLine("C: Exit");
    Console.WriteLine("------------------------------------------");
    ReadInput();
}

void ReadInput()
{
    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
    // Check if a specific key was pressed
    isA = key.Key == ConsoleKey.A;
    isB = key.Key == ConsoleKey.B;
    isC = key.Key == ConsoleKey.C;  
    
    if (isA)
    {
        EncryptMessage();
    }
    else if (isB)
    {
        DecryptMessage();
    }
    else if (isC)
    {
        Console.WriteLine("Closing ...");
        Environment.Exit(0);
    }
    else
    {
        Console.WriteLine("Command not found");
        RunProgram();
    }
}

void EncryptMessage()
{
    Console.WriteLine("Enter your message to encrypt");

    binary = Encoding.UTF8.GetBytes(Console.ReadLine());
        
    Console.WriteLine("Message encrypted.");
    RunProgram();
}

void DecryptMessage()
{
    Console.WriteLine("Here is the decrypted message: ");

    if (binary == null)
    {
        Console.WriteLine(enteredText);
    }
    else
    {
        Console.WriteLine(Encoding.UTF8.GetString(binary));
    }
    RunProgram();
}
