using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AES {

    // 암호 모드
    public enum mode {
        ENCRYPT,
        DECRYPT
    }


    // 키, 벡터값(초기값) 생성
    public static Rfc2898DeriveBytes CreateKeyAndVector(string password) {
        byte[] keyBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);

        Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(keyBytes, saltBytes, 100000);

        return result;
    }

    // 암/복호화 -> 256 키 사이즈 AES 암호화
    public static byte[] Cipher(byte[] origin, string password, mode m) {
        RijndaelManaged aes = new RijndaelManaged();
        Rfc2898DeriveBytes key = CreateKeyAndVector(password);
        Rfc2898DeriveBytes vector = CreateKeyAndVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");

        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key.GetBytes(32);
        aes.IV = vector.GetBytes(16);

        ICryptoTransform cryptoT = m == mode.ENCRYPT ? aes.CreateEncryptor(aes.Key, aes.IV) : aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream ms = new MemoryStream()) {
            using (CryptoStream cs = new CryptoStream(ms, cryptoT, CryptoStreamMode.Write)) {
                cs.Write(origin, 0, origin.Length);
            }
            return ms.ToArray();
        }
    }
}
