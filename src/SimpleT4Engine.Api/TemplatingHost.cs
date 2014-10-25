using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using org.ncore.Diagnostics;

// NOTE: Here's what the call path looks like so far, based on the specified input.

// INPUT:
/*
<#@ template language="C#" debug="true" #>
<#@ import namespace="System" #>
My new guid = <#= Guid.NewGuid() #>
*/

// CALL PATH
/*
TemplatingHost host = new TemplatingHost();
	-> .fileExtensionValue_Get
	-> .fileEncodingValue_Get

string output = engine.ProcessTemplate( input, host );
	-> .TemplateFile_Get
	-> .StandardImports_Get
	-> .StandardAssemblyReferences_Get
	-> .GetHostOption( "CacheAssemblies" )
	-> .ProvideTemplatingAppDomain(
										"namespace Microsoft.VisualStudio.TextTemplating32180CC6B395AA5D6E0DC98503B3A3AF {
											using System;
											
											
											#line 1 "template1.t4"
											public class GeneratedTextTransformation : Microsoft.VisualStudio.TextTemplating.TextTransformation {
												public override string TransformText() {
													try {
														this.Write("My new guid = ");
														
														#line 3 "template1.t4"
														this.Write(Microsoft.VisualStudio.TextTemplating.ToStringHelper.ToStringWithCulture(Guid.NewGuid()));
														
														#line default
														#line hidden
														this.Write("\r\n");
													}
													catch (System.Exception e) {
														System.CodeDom.Compiler.CompilerError error = new System.CodeDom.Compiler.CompilerError();
														error.ErrorText = e.ToString();
														error.FileName = "template1.t4";
														this.Errors.Add(error);
													}
													return this.GenerationEnvironment.ToString();
												}
											}
											
											#line default
											#line hidden
										}"
									)
	-> .ResolveAssemblyReference( "C:\Windows\assembly\GAC_MSIL\System\2.0.0.0__b77a5c561934e089\System.dll" )
	-> .LogErrors( a CompilerErrorCollection )
*/


namespace SimpleT4Engine.Api
{
    // INSPIRED BY 
    //      http://msdn.microsoft.com/en-us/library/bb126519.aspx
    //      http://msdn.microsoft.com/en-us/library/bb126408%28VS.80%29.aspx
    // ORIGINAL SOURCE FROM http://msdn.microsoft.com/en-us/library/bb126579.aspx
    // TEMPLATING REFERENCE: http://msdn.microsoft.com/en-us/library/bb126421.aspx
    // HOW TO BUILD A CUSTOM DIRECTIVE PROCESSOR: http://msdn.microsoft.com/en-us/library/bb126315.aspx

    // IMPORTANT REFERENCES:
    //  DEALING WITH "hostspecific" and the serialization issue:  http://blog.magenic.com/blogs/jons/default.aspx

    public class TemplatingHost : MarshalByRefObject, ITextTemplatingEngineHost
    {
        private const string DEFAULT_FILE_EXTENSION = ".txt";
        private static readonly Encoding DEFAULT_FILE_ENCODING = Encoding.UTF8;

        public TemplatingHost()
            : this( DEFAULT_FILE_EXTENSION, DEFAULT_FILE_ENCODING )
        {
        }

        public TemplatingHost( string fileExtension )
            : this( fileExtension, DEFAULT_FILE_ENCODING)
        {
        }

        public TemplatingHost( Encoding fileEncoding )
            : this( DEFAULT_FILE_EXTENSION, fileEncoding )
        {
        }

        public TemplatingHost( string fileExtension, Encoding fileEncoding )
            : base()
        {
            fileExtensionValue = fileExtension;
            fileEncodingValue = fileEncoding;

            standardAssemblyReferences.Add( "System" );
            standardImports.Add( "System" );
        }

        private string templateFileValue;
        private string fileExtensionValue;
        private Encoding fileEncodingValue;
        private CompilerErrorCollection errorsValue;
        private Dictionary<string, string> defaultParameters = new Dictionary<string, string>();
        private List<string> includeFileSearchPaths = new List<string>();
        private List<string> assemblySearchPaths = new List<string>();
        private List<string> standardAssemblyReferences = new List<string>();
        private List<string> standardImports = new List<string>();

        public string TemplateFile
        {
            get { return templateFileValue; }
            set { templateFileValue = value; }
        }

        // NOTE: Must be R/O because it looks like the private member gets read
        //  in the constructor.  JF
        public string FileExtension
        {
            get { return fileExtensionValue; }
        }

        // NOTE: Must be R/O because it looks like the private member gets read
        //  in the constructor.  JF
        public Encoding FileEncoding
        {
            get { return fileEncodingValue; }
        }

        public CompilerErrorCollection Errors
        {
            get { return errorsValue; }
        }

        public Dictionary<string, string> DefaultParameters
        {
            get { return defaultParameters; }
            set { defaultParameters = value; }
        }

        public List<string> IncludeFileSearchPaths
        {
            get { return includeFileSearchPaths; }
        }

        public List<string> AssemblySearchPaths
        {
            get { return assemblySearchPaths; }
        }

        public IList<string> StandardAssemblyReferences
        {
            get { return standardAssemblyReferences; }
        }

        public IList<string> StandardImports
        {
            get { return standardImports; }
        }

        public bool LoadIncludeText( string requestFileName, out string content, out string location )
        {
            content = System.String.Empty;
            location = System.String.Empty;

            if( File.Exists( requestFileName ) )
            {
                content = File.ReadAllText( requestFileName );
                return true;
            }

            // NOTE: Check the same directory as the running template file.  JF
            string candidate = Path.Combine( Path.GetDirectoryName( this.TemplateFile ), requestFileName );
            if( File.Exists( candidate ) )
            {
                content = File.ReadAllText( candidate );
                location = candidate;
                return true;
            }

            // NOTE: Finally check our list of custom specified paths.  JF
            foreach( string path in includeFileSearchPaths )
            {
                string fullFilePath = Path.Combine( path, requestFileName );
                if( File.Exists( fullFilePath ) )
                {
                    content = File.ReadAllText( fullFilePath );
                    location = fullFilePath;
                    return true;
                }
            }

            return false;
        }

        public object GetHostOption( string optionName )
        {
            object returnObject;
            switch( optionName )
            {
                case "CacheAssemblies":
                    returnObject = true;
                    break;
                default:
                    returnObject = null;
                    break;
            }
            return returnObject;
        }

        public string ResolveAssemblyReference( string assemblyReference )
        {
            Spy.Trap( () => assemblyReference );

            if( File.Exists( assemblyReference ) )
            {
                Spy.Trace( "Found at absolute path: {0}", assemblyReference );
                return assemblyReference;
            }

            // NOTE: Check the same directory as the running template file.  JF
            string candidate = Path.Combine( Path.GetDirectoryName( this.TemplateFile ), assemblyReference );
            if( File.Exists( candidate ) )
            {
                Spy.Trace("Found in template file directory: {0}", candidate);
                return candidate;
            }

            // NOTE: Check the AppDomain.CurrentDomain.BaseDirectory (path of the running app).  JF
            candidate = Path.Combine( Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory ),
                                             assemblyReference );
            if (File.Exists(candidate))
            {
                Spy.Trace("Found in AppDomain.CurrentDomain.BaseDirectory directory: {0}", candidate);
                return candidate;
            }

            // NOTE: Check our list of custom specified paths.  JF
            foreach( string path in assemblySearchPaths )
            {
                string fullFilePath = Path.Combine( path, assemblyReference );
                if( File.Exists( fullFilePath ) )
                {
                    Spy.Trace("Found in custom specified directory: {0}", fullFilePath);
                    return fullFilePath;
                }
            }

            // NOTE: Finally, check the GAC.  JF
            candidate = AssemblyCache.QueryAssemblyInfo( assemblyReference );
            if( File.Exists( candidate ) )
            {
                Spy.Trace("Found in the GAC: {0}", candidate);
                return candidate;
            }

            // NOTE: If don't come up with anything then we must return string.Empty.  JF
            return string.Empty;
        }

        //The engine calls this method based on the directives the user has 
        //specified in the text template.
        //This method can be called 0, 1, or more times.
        //---------------------------------------------------------------------
        public Type ResolveDirectiveProcessor( string processorName )
        {
            //This host will not resolve any specific processors.
            //Check the processor name, and if it is the name of a processor the 
            //host wants to support, return the type of the processor.
            //---------------------------------------------------------------------
            if( string.Compare( processorName, "XYZ", StringComparison.OrdinalIgnoreCase ) == 0 )
            {
                //return typeof();
            }
            //This can be customized to search specific paths for the file
            //or to search the GAC
            //If the directive processor cannot be found, throw an error.
            throw new Exception( "Directive Processor not found" );
        }

        //A directive processor can call this method if a file name does not 
        //have a path.
        //The host can attempt to provide path information by searching 
        //specific paths for the file and returning the file and path if found.
        //This method can be called 0, 1, or more times.
        //---------------------------------------------------------------------
        public string ResolvePath( string fileName )
        {
            if( fileName == null )
            {
                throw new ArgumentNullException( "The file name cannot be null" );
            }

            //If the argument is the fully qualified path of an existing file,
            //then we are done
            //----------------------------------------------------------------
            if( File.Exists( fileName ) )
            {
                return fileName;
            }

            //Maybe the file is in the same folder as the text template that 
            //called the directive.
            //----------------------------------------------------------------
            string candidate = Path.Combine( Path.GetDirectoryName( this.TemplateFile ), fileName );
            if( File.Exists( candidate ) )
            {
                return candidate;
            }

            //Look more places.
            //----------------------------------------------------------------
            //More code can go here...
            //If we cannot do better, return the original file name.
            return fileName;
        }

        public string ResolveParameterValue( string directiveId, string processorName, string parameterName )
        {
            if( directiveId == null && processorName == null && parameterName != null )
            {
                if( defaultParameters.Keys.Contains( parameterName ) )
                {
                    return defaultParameters[ parameterName ];
                }
            }

            if( directiveId == null )
            {
                throw new ArgumentNullException( "The directiveId cannot be null." );
            }
            if( processorName == null )
            {
                throw new ArgumentNullException( "The processorName cannot be null." );
            }
            if( parameterName == null )
            {
                throw new ArgumentNullException( "The parameterName cannot be null." );
            }
            
            // TODO: Default directive suppport goes here.  JF

            return String.Empty;
        }

        public void SetFileExtension( string extension )
        {
            fileExtensionValue = extension;
        }

        public void SetOutputEncoding( System.Text.Encoding encoding, bool fromOutputDirective )
        {
            fileEncodingValue = encoding;
        }

        public void LogErrors( CompilerErrorCollection errors )
        {
            errorsValue = errors;
        }

        public AppDomain ProvideTemplatingAppDomain( string content )
        {
            return AppDomain.CreateDomain( "SimpleT4Engine.Api.TemplatingHost App Domain" );
        }
    }
}
