namespace FileTransfer.App.Services
{
    using System;
    using System.IO;

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

        /// <summary>
        /// Transfers a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The path of the source file.</param>
        /// <param name="destinationFileDirectory">The destination file directory.</param>
        public void Transfer(string sourcePath, string destinationFileDirectory)
        {
            ValidateInputParameters(sourcePath, destinationFileDirectory);
            ProcessTransfer(sourcePath, destinationFileDirectory);
        }

        /// <summary>
        /// Processes the file transfer from source file path to destination.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationFileDirectory"></param>
        private TransferResult ProcessTransfer(
        string sourcePath,
        string destinationFileDirectory)
        {
            string destinationFilePath = Path.Combine(
                destinationFileDirectory,
                Path.GetFileName(sourcePath));

            List<FileChunk> chunks = new List<FileChunk>();
            FileHashCalculator hashCalculator = new FileHashCalculator();
            long offset = 0;

            using (FileStream sourceStream = File.OpenRead(sourcePath))
            using (FileStream destinationStream = new FileStream(
                destinationFilePath,
                FileMode.Create,
                FileAccess.ReadWrite))
            {
                byte[] sourceBuffer =
                    new byte[_transferOptions.ChunkSizeBytes];

                byte[] verificationBuffer =
                    new byte[_transferOptions.ChunkSizeBytes];

                int bytesRead;

                while ((bytesRead = sourceStream.Read(
                    sourceBuffer,
                    0,
                    sourceBuffer.Length)) > 0)
                {
                    string sourceChunkHash =
                        hashCalculator.ComputeChunkMd5(
                            sourceBuffer,
                            bytesRead);

                    destinationStream.Write(
                        sourceBuffer,
                        0,
                        bytesRead);

                    destinationStream.Flush();

                    destinationStream.Seek(
                        offset,
                        SeekOrigin.Begin);

                    int totalBytesRead = 0;

                    while (totalBytesRead < bytesRead)
                    {
                        int readCount = destinationStream.Read(
                            verificationBuffer,
                            totalBytesRead,
                            bytesRead - totalBytesRead);

                        if (readCount == 0)
                        {
                            throw new EndOfStreamException(
                                "The destination ended before the chunk was fully read.");
                        }

                        totalBytesRead += readCount;
                    }

                    string destinationChunkHash =
                        hashCalculator.ComputeChunkMd5(
                            verificationBuffer,
                            bytesRead);

                    if (!string.Equals(
                            sourceChunkHash,
                            destinationChunkHash,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        RetryChunk(destinationStream, sourceBuffer, verificationBuffer, bytesRead, offset, sourceChunkHash, hashCalculator);
                    }

                    chunks.Add(new FileChunk(
                        offset,
                        bytesRead,
                        sourceChunkHash));

                    offset += bytesRead;

                    destinationStream.Seek(
                        offset,
                        SeekOrigin.Begin);
                }
            }

            string sourceHash =
                hashCalculator.ComputeFileSha256(sourcePath);

            string destinationHash =
                hashCalculator.ComputeFileSha256(destinationFilePath);

            return new TransferResult(
                chunks,
                sourceHash,
                destinationHash);
        }

        #region "Private validation methods"  

        /// <summary>
        /// Writes a chunk to the destination and retries it when MD5 verification fails.
        /// </summary>
        /// <param name="destinationStream">The destination file stream.</param>
        /// <param name="sourceBuffer">The source chunk data.</param>
        /// <param name="verificationBuffer">The buffer used to read the destination chunk.</param>
        /// <param name="bytesRead">The number of valid bytes in the chunk.</param>
        /// <param name="offset">The chunk position in the destination file.</param>
        /// <param name="sourceChunkHash">The expected MD5 hash.</param>
        /// <param name="hashCalculator">The calculator used to hash the chunk.</param>
        /// <exception cref="InvalidDataException">
        /// Thrown when the chunk cannot be verified after all attempts.
        /// </exception>
        /// <exception cref="EndOfStreamException">
        /// Thrown when the destination ends before the chunk is fully read.
        /// </exception>
        private void RetryChunk(
        FileStream destinationStream,
        byte[] sourceBuffer,
        byte[] verificationBuffer,
        int bytesRead,
        long offset,
        string sourceChunkHash,
        FileHashCalculator hashCalculator)
        {
            for (int attempt = 0;
                 attempt <= _transferOptions.MaxRetries;
                 attempt++)
            {
                destinationStream.Seek(
                    offset,
                    SeekOrigin.Begin);

                destinationStream.Write(
                    sourceBuffer,
                    0,
                    bytesRead);

                destinationStream.Flush();

                destinationStream.Seek(
                    offset,
                    SeekOrigin.Begin);

                int totalBytesRead = 0;
                bool chunkReadCompletely = true;

                while (totalBytesRead < bytesRead)
                {
                    int readCount = destinationStream.Read(
                        verificationBuffer,
                        totalBytesRead,
                        bytesRead - totalBytesRead);

                    if (readCount == 0)
                    {
                        chunkReadCompletely = false;
                        break;
                    }

                    totalBytesRead += readCount;
                }

                if (!chunkReadCompletely)
                {
                    continue;
                }

                string destinationChunkHash =
                    hashCalculator.ComputeChunkMd5(
                        verificationBuffer,
                        bytesRead);

                if (string.Equals(
                        sourceChunkHash,
                        destinationChunkHash,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            throw new InvalidDataException(
                $"Chunk verification failed at offset {offset} after " +
                $"{_transferOptions.MaxRetries + 1} attempts.");
        }

        /// <summary>
        /// Validates that the input strings for the file destination and source are not null or empty.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationFileDirectory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void ValidateInputParameters(string sourcePath, string destinationFileDirectory)
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));

            if (string.IsNullOrWhiteSpace(destinationFileDirectory)) throw new ArgumentNullException(nameof(destinationFileDirectory));

            ValidateSourceFileExists(sourcePath);
            ValidateDestinationDirectoryExists(destinationFileDirectory);
            ValidateSourceAndDestinationAreNotSameFile(sourcePath, destinationFileDirectory);
        }

        /// <summary>
        /// Validates that source and destination directories are not the same. Throws exception when they are.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationFileDirectory"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateSourceAndDestinationAreNotSameFile(string sourcePath, string destinationFileDirectory)
        {
            var sourceFilePath = Path.GetFullPath(sourcePath);
            var destinationFilePath = Path.GetFullPath(
                Path.Combine(destinationFileDirectory, Path.GetFileName(sourcePath)));

            if (string.Equals(
                    sourceFilePath,
                    destinationFilePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "The destination file cannot be the same as the source file.");
            }
        }

        /// <summary>
        /// Validates whether the source file exists. Throws file not found exception if not.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        private void ValidateSourceFileExists(string sourcePath)
        {
            if (!File.Exists(sourcePath)) { throw new FileNotFoundException("The source file was not found", sourcePath); }
        }

        /// <summary>
        /// Validates the destinationDirectory. If it doesn't exist, it creates it.
        /// </summary>
        /// <param name="destinationFileDirectory"></param>
        private void ValidateDestinationDirectoryExists(string destinationFileDirectory)
        {
            if (!Directory.Exists(destinationFileDirectory))
            {
                Directory.CreateDirectory(destinationFileDirectory);
            }
        }
        #endregion
    }
}
