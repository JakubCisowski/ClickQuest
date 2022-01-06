using System.IO;
using System.Security.Cryptography;

namespace ClickQuest.ContentManager.GameData;

// Source: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-6.0
public static class DataEncryptionController
{
	private static readonly byte[] AesKey;
	private static readonly byte[] AesIv;

	static DataEncryptionController()
	{
		// These values should not be changed to prevent.
		// They were randomly generated the first time the algorithm was run.
		AesKey = new byte[] { 49, 240, 220, 91, 21, 201, 134, 156, 2, 104, 151, 208, 118, 136, 149, 29, 125, 94, 28, 104, 89, 216, 121, 20, 106, 187, 167, 68, 178, 27, 88, 62 };

		AesIv = new byte[] { 18, 37, 145, 119, 109, 3, 139, 169, 191, 72, 159, 19, 248, 63, 190, 3 };
	}

	public static byte[] EncryptJsonUsingAes(string json)
	{
		byte[] encryptedJson;

		using (var aes = Aes.Create())
		{
			aes.Key = AesKey;
			aes.IV = AesIv;
			aes.Padding = PaddingMode.PKCS7;

			var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (var streamWriter = new StreamWriter(cryptoStream))
					{
						streamWriter.Write(json);
					}

					encryptedJson = memoryStream.ToArray();
				}
			}
		}

		return encryptedJson;
	}

	public static string DecryptJsonUsingAes(byte[] encryptedJson)
	{
		string json;

		using (var aes = Aes.Create())
		{
			aes.Key = AesKey;
			aes.IV = AesIv;
			aes.Padding = PaddingMode.PKCS7;

			var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

			using (var memoryStream = new MemoryStream(encryptedJson))
			using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
			using (var streamReader = new StreamReader(cryptoStream))
			{
				json = streamReader.ReadToEnd();
			}
		}

		return json;
	}
}