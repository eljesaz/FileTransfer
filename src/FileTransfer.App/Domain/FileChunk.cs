namespace FileTransfer.App.Domain
{
    using System;

    /// <summary>
    /// The public class for the <see cref="FileChunk"/> model.
    /// </summary>
    public class FileChunk
    {
        /// <summary>
        /// The public constructer for <see cref="FileChunk"/> model.
        /// </summary>
        /// <param name="offset">The chunk position in the source file.</param>
        /// <param name="length">The number of bytes in the chunk.</param>
        /// <param name="md5hash">The MD5 checksum of the chunk.</param>
        public FileChunk(long offset, int length, string md5hash)
        {
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }

            if (length <= 0) { throw new ArgumentOutOfRangeException(nameof(length)); }

            if (string.IsNullOrWhiteSpace(md5hash))
            {
                throw new ArgumentException("MD5 hash is required.", nameof(md5hash));
            }

            Offset = offset;
            Length = length;
            Md5Hash = md5hash;
        }
        /// <summary>
        /// Position in the source file.
        /// </summary>
        public long Offset { get; }

        /// <summary>
        /// Number of bytes in the chunk.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Checksum of the chunk.
        /// </summary>
        public string Md5Hash { get; }
    }
}
