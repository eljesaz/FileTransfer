namespace FileTransfer.App.Services
{
    using System.Security.Cryptography;

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
            return String.Empty;
        }
    }

}
