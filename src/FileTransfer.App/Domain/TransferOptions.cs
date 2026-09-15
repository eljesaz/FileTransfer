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
        /// <param name="chunkSizeBytes">The max bytes a chunk can contain.</param>
        /// <param name="maxRetries">The maximum number of retries per chunk.</param>
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
        /// The maximum number of retries per chunk.
        /// </summary>
        public int MaxRetries { get; }
    }
}
