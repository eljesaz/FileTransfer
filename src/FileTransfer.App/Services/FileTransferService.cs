namespace FileTransfer.App.Services
{
    using System;

    using FileTransfer.App.Domain;

    /// <summary>
    /// Provides chunked file transfer operations with integrity verification.
    /// </summary>
    public class FileTransferService
    {
        private readonly TransferOptions _transferOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTransferService"/> class.
        /// </summary>
        /// <param name="transferOptions">
        /// Configuration used to control the file transfer.
        /// </param>
        public FileTransferService(TransferOptions transferOptions) 
        {
            if (transferOptions == null) throw new ArgumentNullException(nameof(transferOptions));
            _transferOptions = transferOptions;
        }

    }
}
