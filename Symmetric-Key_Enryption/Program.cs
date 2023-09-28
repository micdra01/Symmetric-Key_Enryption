// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;

bool isA, isB, isC;
string? msgToEncrypt = null;

byte[] key = new byte[32];
byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
byte[] plaintextBytes = new byte[32];
byte[] ciphertext = new byte[plaintextBytes.Length];
byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

//Attempt to include passphrase to read from file in a later session
string? passphrase = "dummy";
byte[] Salt = new byte[10];


RunProgram();

//Unused attempt to include passphrase in order to read from the file in a later session
void Login()
{
    Console.WriteLine("Enter your passphrase:");
    passphrase = Console.ReadLine();
    
    RunProgram();
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
        if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/EncryptedMsg.txt"))
        {
            DecryptMessage();
        }
        else
        {
            Console.WriteLine("No message to decrypt found");
            RunProgram();
        }
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
    msgToEncrypt = Console.ReadLine();

    //Key generation using passphrase
    key = CreateKey(passphrase);
    using var aes = new AesGcm(key);
    
    //'Number used once'/IV generation
    RandomNumberGenerator.Fill(nonce);
    
    //Plaintext bytes and ciphertext preparation
    plaintextBytes = Encoding.UTF8.GetBytes(msgToEncrypt);
    ciphertext = new byte[plaintextBytes.Length];
    
    //Encrypting
    aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
    Console.WriteLine("Message encrypted.");
    
    // Write to a file
    byte[] combined = new byte[nonce.Length + ciphertext.Length + tag.Length];
    System.Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
    System.Buffer.BlockCopy(ciphertext, 0, combined, nonce.Length, ciphertext.Length);
    System.Buffer.BlockCopy(tag, 0, combined, nonce.Length + ciphertext.Length, tag.Length);
    File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/EncryptedMsg.txt", combined);
    
    RunProgram();
}

void DecryptMessage()
{
    //Show encrypted message from file
    using (var sr = new StreamReader(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/EncryptedMsg.txt"))
    {
        Console.WriteLine("Here is the message as read from the file: ");
        Console.WriteLine(sr.ReadToEnd());
    }

    //Decrypt the message from memory, can't work it out from the file :(
    try
    {
        key = CreateKey(passphrase);
        using (var aes = new AesGcm(key))
        {
            Console.WriteLine("Here is the decrypted message: ");
            plaintextBytes = new byte[ciphertext.Length];
        
            aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

            Console.WriteLine(Encoding.UTF8.GetString(plaintextBytes));;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        RunProgram();
    }
    RunProgram();
}

byte[] CreateKey(string passphrase, int keyBytes = 32)
{
    const int Iterations = 300;
    var keyGenerator = new Rfc2898DeriveBytes(passphrase, Salt, Iterations);
    return keyGenerator.GetBytes(keyBytes);
}


// ReSharper disable once InvalidXmlDocComment
/**
 * Notes:
 *
 * SecretMessage.cs med 4 x byte[], Nonce, Tag, Salt, Cipher
 * JSON serialize det i en JsonStorage.cs ?
 * Så kan json tingen skrives til fil og læses igen
*/