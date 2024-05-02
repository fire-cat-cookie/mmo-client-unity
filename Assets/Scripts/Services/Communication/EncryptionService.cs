using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptionService : MonoBehaviour{
    private string serverPublicKey =
           "<RSAKeyValue><Modulus>4MEU8lNX7YqPKvwg9muarH3OnVbvFbQKjXFnvFMVShOnpY4LR6/Ykd2RaZaYwPoBifblngPfBc3STnGABK8fYpeYY7/8bjiiW+p5e/HyDvAkxh90fp+NLF+TsOOXFkIuYPv1PaDTIE9MfRkcvZc7UaWJerSXIFIBQfXNy1d1sGU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private byte[] Encrypt(byte[] data) {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(serverPublicKey);
        return rsa.Encrypt(data, true);
    }

    public byte[] Encrypt(byte[] message, int startIndex) {

        if (startIndex == 0) {
            return Encrypt(message);
        }

        byte[] toEncrypt = new byte[message.Length - startIndex];
        Array.Copy(message, startIndex, toEncrypt, 0, toEncrypt.Length);

        byte[] encrypted = Encrypt(toEncrypt);

        byte[] combined = new byte[encrypted.Length + startIndex];
        Array.Copy(message, 0, combined, 0, startIndex);
        Array.Copy(encrypted, 0, combined, startIndex, encrypted.Length);

        return combined;
    }
}
