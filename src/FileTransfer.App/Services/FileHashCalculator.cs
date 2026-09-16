namespace FileTransfer.App.Services
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Creates hashes from file chunks and whole files sequentialy.
    /// </summary>
    public class FileHashCalculator
    {
        public FileHashCalculator() { }
        public string ComputeChunkMd5(byte[] buffer, int bytesRead )
        {
            ArgumentNullException.ThrowIfNull(buffer);

            if (bytesRead < 0 || bytesRead > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesRead));
            }

            byte[] hashBytes;

            using (MD5 md5Hash = MD5.Create())
            {
                hashBytes = md5Hash.ComputeHash(
                    buffer,
                    0,
                    bytesRead);
            }

            return Convert.ToHexString(hashBytes);
        }
        
        public string ComputeFileSha256(string filePath)
        {

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    "A file path is required.",
                    filePath);
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The file was not found.",
                    filePath);
            }
            byte[] hashBytes;

            using (FileStream stream = File.OpenRead(filePath))
            using (SHA256 sha256Hash = SHA256.Create())
            {
                hashBytes = sha256Hash.ComputeHash(stream);
            }

            StringBuilder hashBuilder = new(hashBytes.Length * 2);

            foreach (byte hashByte in hashBytes)
            {
                hashBuilder.Append(hashByte.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }

}
