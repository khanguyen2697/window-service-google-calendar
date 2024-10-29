using System;
using System.Security.Cryptography;
using System.Text;

namespace GoogleCanlendarService.Utils
{
    class DumpData
    {
        public static string GenerateCustomUuid(int length = 26)
        {
            char[] Base32HexChars = "0123456789abcdefghijklmnopqrstuv".ToCharArray();
            if (length < 5 || length > 1024)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 5 and 1024 characters.");
            }

            // Calculate how many bytes we need (5 bits per base32hex character)
            int numBits = length * 5; // Each base32hex character represents 5 bits
            int numBytes = (numBits + 7) / 8; // Convert bits to bytes, rounding up

            // Generate random bytes
            byte[] randomBytes = new byte[numBytes];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert the bytes to base32hex string
            StringBuilder uuidBuilder = new StringBuilder(length);

            // Process each byte and convert it to base32hex characters
            int bitBuffer = 0;
            int bitsInBuffer = 0;

            foreach (byte b in randomBytes)
            {
                bitBuffer = (bitBuffer << 8) | b;
                bitsInBuffer += 8;

                while (bitsInBuffer >= 5)
                {
                    bitsInBuffer -= 5;
                    int index = (bitBuffer >> bitsInBuffer) & 31; // Extract 5 bits at a time
                    uuidBuilder.Append(Base32HexChars[index]);
                }
            }

            // If there are remaining bits, pad them with zeros
            if (bitsInBuffer > 0)
            {
                int index = (bitBuffer << (5 - bitsInBuffer)) & 31;
                uuidBuilder.Append(Base32HexChars[index]);
            }

            // Trim the result to the desired length
            return uuidBuilder.ToString().Substring(0, length);
        }
    }
}
