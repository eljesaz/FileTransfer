namespace FileTransfer.App.Domain
{
    using System;

    /// <summary>
    /// The public class for the <see cref="TransferOptions"/> model.
    /// </summary>
    public class TransferOptions
    {
        /// <summary>
        /// The public contructor for the <see cref="TransferOptions"/>.
        /// </summary>
        public TransferOptions(int chunkSizeBytes, int maxRetries) 
        { 
            if(chunkSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSizeBytes));
            if(maxRetries < 0)throw new ArgumentOutOfRangeException(nameof(maxRetries));
            this.ChunkSizeBytes = chunkSizeBytes;
            this.MaxRetries = maxRetries;
        }
        /// <summary>
        /// The max bytes a chunk can contain.
        /// </summary>
        public int ChunkSizeBytes { get; }

        /// <summary>
        /// The max retries a chunk can try to transfer.
        /// </summary>
        public int MaxRetries { get; }
    }
}
