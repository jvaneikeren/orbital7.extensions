﻿using System.Security.Cryptography;

namespace Orbital7.Extensions.Encryption;

public class TripleDESEncryptionEngine : 
    IEncryptionEngine
{
    public byte[] Encrypt(
        byte[] data, 
        string passphrase, 
        Encoding encoding)
    {
        // Source: http://www.dijksterhuis.org/encrypting-decrypting-string/

        byte[] results;

        // Step 1. We hash the passphrase using MD5
        // We use the MD5 hash generator as the result is a 128 bit byte array
        // which is a valid length for the TripleDES encoder we use below

        var HashProvider = MD5.Create();
        byte[] TDESKey = HashProvider.ComputeHash(encoding.GetBytes(passphrase));

        // Step 2. Create the algorithm
        var TDESAlgorithm = TripleDES.Create();

        // Step 3. Setup the encoder
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;

        // Step 5. Attempt to encrypt the string
        try
        {
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            results = Encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        finally
        {
            // Clear the TripleDes and Hashprovider services of any sensitive information
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }

        // Step 6. Return the encrypted bytes.
        return results;
    }

    public byte[] Decrypt(
        byte[] data, 
        string passphrase, 
        Encoding encoding)
    {
        byte[] results;

        // Step 1. We hash the passphrase using MD5
        // We use the MD5 hash generator as the result is a 128 bit byte array
        // which is a valid length for the TripleDES encoder we use below

        var HashProvider = MD5.Create();
        byte[] TDESKey = HashProvider.ComputeHash(encoding.GetBytes(passphrase));

        // Step 2. Create the algorithm
        var TDESAlgorithm = TripleDES.Create();

        // Step 3. Setup the decoder
        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;

        // Step 4. Attempt to decrypt the string
        try
        {
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            results = Decryptor.TransformFinalBlock(data, 0, data.Length);
        }
        finally
        {
            // Clear the TripleDes and Hashprovider services of any sensitive information
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }

        // Step 5. Return the decrypted data
        return results;
    }
}
