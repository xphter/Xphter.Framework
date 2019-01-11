using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xml;
using System.Xml;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for XhtmlUrlResolverTest and is intended
  ///to contain all XhtmlUrlResolverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class XhtmlUrlResolverTest {


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
    ///A test for ResolveUri
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ResolveUriTest_Null() {
      new XhtmlUrlResolver(null);
    }

    /// <summary>
    ///A test for ResolveUri
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ResolveUriTest_InvalidPath() {
      new XhtmlUrlResolver("*l.2j*&)(");
    }

    /// <summary>
    ///A test for ResolveUri
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ResolveUriTest_FilePath() {
      string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, Path.GetRandomFileName());
      try {
        File.Create(file).Close();
        new XhtmlUrlResolver(file);
      } finally {
        if(File.Exists(file)) {
          File.Delete(file);
        }
      }
    }

    /// <summary>
    ///A test for ResolveUri
    ///</summary>
    [TestMethod()]
    public void ResolveUriTest() {
      XhtmlUrlResolver resolver = new XhtmlUrlResolver(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName);
      XmlDocument document = new XmlDocument();
      document.XmlResolver = resolver;
      document.LoadXml("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><a>&nbsp;&gt;&lt;&amp;&quot;&copy;&reg;&apos;</a>");
      resolver.Delete();
    }
  }
}
