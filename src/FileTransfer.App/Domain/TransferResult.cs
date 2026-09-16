namespace FileTransfer.App.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The class model that represents the response from file transfer.
    /// </summary>
    public class TransferResult
    {
        public TransferResult(IReadOnlyList<FileChunk> chunks, string sourceSha256, string destinationSha256) 
        {
            ValidateInputParameters(chunks, sourceSha256, destinationSha256);

            this.Chunks = chunks;
            this.SourceSha256 = sourceSha256;
            this.DestinationSha256 = destinationSha256;
            HashesMatch = string.Equals(sourceSha256, destinationSha256, StringComparison.OrdinalIgnoreCase);
        }

        private void ValidateInputParameters(IReadOnlyList<FileChunk> chunks, string sourceSha256, string destinationSha256)
        {
            ArgumentNullException.ThrowIfNull(chunks);
            if (string.IsNullOrWhiteSpace(sourceSha256)) 
            {
                throw new ArgumentException("Source file hash is empty", nameof(sourceSha256));
            }
            if(string.IsNullOrWhiteSpace(destinationSha256))
            {
                throw new ArgumentException("Destination file hash is empty", nameof(destinationSha256));
            }
        }

        public IReadOnlyList<FileChunk> Chunks { get; }
        public string SourceSha256 { get; }
        public string DestinationSha256  { get; }
        public bool HashesMatch { get; }
    }
}
