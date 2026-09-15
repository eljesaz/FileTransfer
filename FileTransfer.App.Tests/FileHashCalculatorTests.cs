using System;
using System.Security.Cryptography;
using System.Text;

using FileTransfer.App.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileTransfer.App.Tests;

[TestClass]
public class FileHashCalculatorTests
{
    [TestMethod]
    public void ComputeChunkMd5_UsesOnlyBytesRead()
    {
        const string input = "hello";
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        string expectedHash = Convert.ToHexString(
            MD5.HashData(inputBytes));

        byte[] buffer = Encoding.UTF8.GetBytes(
            input + "-extra-data");

        var calculator = new FileHashCalculator();

        string actualHash = calculator.ComputeChunkMd5(
            buffer,
            inputBytes.Length);

        Assert.AreEqual(expectedHash, actualHash);
    }
}