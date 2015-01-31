using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionDependencyScanner;

namespace SolutionDependencyScannerTests
{
    [TestClass]
    public class PathExtensionTests
    {
        [TestMethod]
        public void TestNotchanging()
        {
            string path = "c:\\folder1\\folder2\\file.txt";
            string expected = "c:\\folder1\\folder2\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSingleReduction()
        {
            string path = "c:\\folder1\\folder2\\..\\file.txt";
            string expected = "c:\\folder1\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMultipleReduction()
        {
            string path = "c:\\folder1\\folder2\\folder3\\folder4\\..\\..\\..\\file.txt";
            string expected = "c:\\folder1\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void TestMultipleReductionInDifferentPlaces()
        {
            string path = "c:\\folder1\\..\\folder2\\folder3\\..\\folder4\\file.txt";
            string expected = "c:\\folder2\\folder4\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void TestMultipleReductionInDifferentPlacesConflicting()
        {
            string path = "c:\\folder1\\folder2\\folder3\\..\\folder4\\..\\..\\folder5\\file.txt";
            string expected = "c:\\folder1\\folder5\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void TestTooManyReductions()
        {
            string path = "c:\\folder1\\folder2\\..\\..\\..\\file.txt";
            string expected = "c:\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void TestUnixStyleReduction()
        {
            string path = "c:/folder1\\folder2\\folder3/..\\folder4\\../..\\folder5\\file.txt";
            string expected = "c:\\folder1\\folder5\\file.txt";
            string result = PathExtensions.Normalize(path);
            Assert.AreEqual(expected, result);
        }

    }
}
