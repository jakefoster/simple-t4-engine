using System;
using System.Text;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TextTemplating;
using SimpleT4Engine.Api;
using org.ncore.Common;

namespace _unittests.SimpleT4Engine.Api
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        /*
            CustomCmdLineHost host = new CustomCmdLineHost();
            Engine engine = new Engine();
            host.TemplateFileValue = templateFileName;
            //Read the text template.
            string input = File.ReadAllText( templateFileName );
            //Transform the text template.
            string output = engine.ProcessTemplate( input, host );
            //string outputFileName = Path.GetFileNameWithoutExtension( templateFileName );
            outputFileName = Path.Combine( Path.GetDirectoryName( templateFileName ), outputFileName );
            //outputFileName = outputFileName + "1" + host.FileExtension;
            File.WriteAllText( outputFileName, output, host.FileEncoding );

            
            foreach( CompilerError error in host.Errors )
            {
                Console.WriteLine( error.ToString() );
            }
         * */

        [TestMethod]
        public void Simplest()
        {
            TemplatingHost host = new TemplatingHost();
            Engine engine = new Engine();

            string templateFileName = "template1.t4";
            host.DefaultParameters.Add("SomeKey", "SomeValue");
            host.TemplateFile = templateFileName;
            string input = EmbeddedResource.LoadAsString( "Resources.Templates.template1.t4" );
            string output = engine.ProcessTemplate( input, host );


            foreach (CompilerError error in host.Errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ToString());
            }

            System.Diagnostics.Debug.WriteLine("Template produced the following output:");
            System.Diagnostics.Debug.WriteLine(output);

            if (host.Errors != null && host.Errors.Count > 0)
            {
                Assert.Fail("The templating host had errors!");
            }
        }

        [TestMethod]
        public void SimpleTemplateGeneration()
        {
            Engine engine = new Engine();

            TemplatingHost host = new TemplatingHost( System.Text.Encoding.UTF32 );
            host.DefaultParameters.Add( "SomeKey", "SomeValue" );
            host.IncludeFileSearchPaths.Add( @"C:\Includes" );
            host.StandardAssemblyReferences.Add( "System.Web" );
            host.StandardImports.Add( "System.Web" );

            string templateFileName = "some template";
            // NOTE: Doesn't actually seem to care about the name of the template file?  True, but does use the path to search for stuff.
            host.TemplateFile = templateFileName;
            string input = EmbeddedResource.LoadAsString( "Resources.Templates.template1.t4" );
            string output = engine.ProcessTemplate( input, host );

            foreach( CompilerError error in host.Errors )
            {
                System.Diagnostics.Debug.WriteLine( error.ToString() );
            }

            System.Diagnostics.Debug.WriteLine( "Template produced the following output:" );
            System.Diagnostics.Debug.WriteLine( output );

            if( host.Errors != null && host.Errors.Count > 0 )
            {
                Assert.Fail( "The templating host had errors!" );
            }
        }

        [TestMethod]
        public void ComplexTemplateGeneration()
        {
            Engine engine = new Engine();

            TemplatingHost host = new TemplatingHost(System.Text.Encoding.UTF32);
            host.DefaultParameters.Add("SomeKey", "SomeValue");
            host.IncludeFileSearchPaths.Add(@"C:\Includes");
            host.StandardAssemblyReferences.Add("System.Web");
            host.StandardImports.Add("System.Web");

            string templateFileName = "some template";
            // NOTE: Doesn't actually seem to care about the name of the template file?  True, but does use the path to search for stuff.
            host.TemplateFile = templateFileName;
            string input = EmbeddedResource.LoadAsString("Resources.Templates.template2.t4");
            string output = engine.ProcessTemplate(input, host);

            foreach (CompilerError error in host.Errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ToString());
            }

            System.Diagnostics.Debug.WriteLine("Template produced the following output:");
            System.Diagnostics.Debug.WriteLine(output);

            if (host.Errors != null && host.Errors.Count > 0)
            {
                Assert.Fail("The templating host had errors!");
            }
        }
    }
}
