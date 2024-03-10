using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class DesAlgorithm
{
    static void Main()
    {
        Console.WriteLine("Iveskite teksta kuri norite sifruoti arba desifruoti:");
        string inputText = Console.ReadLine();

        Console.WriteLine("Iveskite rakta:");
        string keyInput = Console.ReadLine();

        byte[] key = Encoding.UTF8.GetBytes(keyInput.PadRight(8, ' ').Substring(0, 8));

        Console.WriteLine("Pasirinkite:\n1. Sifruoti\n2. Desifruoti");
        int choice = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Pasirinkite moda (ECB, CBC, CFB):");
        string modeInput = Console.ReadLine().ToUpper();
        CipherMode mode = CipherMode.CBC; 
        switch (modeInput)
        {
            case "ECB":
                mode = CipherMode.ECB;
                break;
            case "CBC":
                mode = CipherMode.CBC;
                break;
            case "CFB":
                mode = CipherMode.CFB;
                break;
         //   case "OFB":
           //     mode = CipherMode.OFB;
            //    break;
           
        }

        string outputText = "";
        switch (choice)
        {
            case 1: 
                outputText = EncryptText(inputText, key, mode);
                Console.WriteLine("Sifruotas tekstas:");
                break;
            case 2: 
                outputText = DecryptText(inputText, key, mode);
                Console.WriteLine("Desifruotas tekstas:");
                break;
        }
        Console.WriteLine(outputText);

       
        if (choice == 1)
        {
            Console.WriteLine("Iveskite failo pavadinima kur norite saugoti sifruota teksta:");
            string filename = Console.ReadLine();
            File.WriteAllText(filename, outputText);
            Console.WriteLine("Sifruotas tekstas issaugotas i faila: " + filename);
        }

       
        if (choice == 2)
        {
            Console.WriteLine("Iveskite failo pavadinimo kur patalpintas tekstas kuri norite desifruoti:");
            string filename = Console.ReadLine();
            string encryptedTextFromFile = File.ReadAllText(filename);
            string decryptedText = DecryptText(encryptedTextFromFile, key, mode);
            Console.WriteLine("Desiftuotas tekstas is failo:");
            Console.WriteLine(decryptedText);
        }

        Console.ReadKey();
    }

    public static string EncryptText(string plainText, byte[] key, CipherMode mode)
    {
        using (DES des = DES.Create())
        {
            des.Key = key;
            des.Mode = mode;
            des.Padding = PaddingMode.PKCS7;

            using (MemoryStream memoryStream = new MemoryStream())
            using (ICryptoTransform encryptor = des.CreateEncryptor(des.Key, des.IV))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (StreamWriter writer = new StreamWriter(cryptoStream))
            {
                writer.Write(plainText);
                writer.Close();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public static string DecryptText(string encryptedText, byte[] key, CipherMode mode)
    {
        using (DES des = DES.Create())
        {
            des.Key = key;
            des.Mode = mode;
            des.Padding = PaddingMode.PKCS7;

            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText)))
            using (ICryptoTransform decryptor = des.CreateDecryptor(des.Key, des.IV))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }


}
