using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Text;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for EncodingUtilityTest and is intended
  ///to contain all EncodingUtilityTest Unit Tests
  ///</summary>
  [TestClass()]
  public class EncodingUtilityTest {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for IsLittleEndian
    ///</summary>
    [TestMethod()]
    public void IsLittleEndianTest() {
      Assert.IsTrue(EncodingUtility.IsLittleEndian);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_UTF8() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\UTF-8.txt");
      Encoding expected = Encoding.UTF8;
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_UTF16_Little() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\UTF-16-Little.txt");
      Encoding expected = Encoding.Unicode;
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_UTF16_Big() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\UTF-16-Big.txt");
      Encoding expected = new UnicodeEncoding(true, true);
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_UTF32_Little() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\UTF-32-Little.txt");
      Encoding expected = Encoding.UTF32;
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_UTF32_Big() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\UTF-32-Big.txt");
      Encoding expected = new UTF32Encoding(true, true);
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for DetectUnicodeEncoding
    ///</summary>
    [TestMethod()]
    public void DetectUnicodeEncodingTest_ANSI() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\ANSI.txt");
      Encoding actual = EncodingUtility.DetectUnicodeEncoding(file);
      Assert.IsNull(actual);
    }
  }
}
