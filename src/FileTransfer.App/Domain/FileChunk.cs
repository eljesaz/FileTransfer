namespace FileTransfer.App.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileChunk
    {
        public FileChunk(long offset, int length, string md5hash)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

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
