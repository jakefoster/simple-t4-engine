
# SimpleT4Engine.Api

TODO: Write up craziness with T4 referencing in .NET 4.0.

Add references to the following .dll files (search paths on my machine in brackets):  

	Microsoft.VisualStudio.TextTemplating.10.0.dll (C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.TextTemplating.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.10.0.dll)  
	Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll (C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll)  

Make sure the following namespaces are both imported in T4MVC.tt (just follow the syntax that's already in the file).  

	Microsoft.VisualStudio.TextTemplating  
	Microsoft.VisualStudio.TextTemplating.Interfaces  

From Microsoft's "Visual Studio 2010 Release Candidate Readme" documentation (http://download.microsoft.com/download/A/F/F/AFFE9A0D-E43C-4402-99C1-DD4E0E58AB60/VS2010RCReadme.htm):  

	2.4.11.4 Change: Text templates do not look for assemblies in the current project

		Text templates fail to find assemblies referenced by the current project.

		The current project's references are no longer used to find assemblies.

		To resolve this issue:

		In an assembly directive, state the location of the required assembly explicitly.

		You can use Visual Studio macros such as $(ProjectDir) or Windows environment variables such as %ProgramFiles% as part of the assembly location.

	2.4.11.5 Change in text template API: assemblies and namespaces have changed

		Code that uses the text templating API and that worked in previous editions fails to compile.

		To resolve this issue:

		The following assemblies, which were used in Visual Studio 2008, are not used in Visual Studio 2010:
		microsoft.visualstudio.texttemplating.vshost.dll

		microsoft.visualstudio.texttemplating.dll

		Your project might require references to both of the following assemblies:
		Microsoft.VisualStudio.TextTemplating.10.0

		Microsoft.VisualStudio.TextTemplating.Interfaces.10.0

		Microsoft.VisualStudio.TextTemplating.Interfaces

		Replace it with:

		Microsoft.VisualStudio.TexMicrosoft.VisualStudio.TextTemplating.VSHost as appropriate.

	2.4.11.6 Change: When a text template is being debugged, Break() does not enter the debugger. Use Launch() instead.

		When you want to step through execution of a text template, a call to System.Diagnostics.Debugger.Break() does not result in entering a debugging instance of Visual Studio. Instead, a message appears that reports "User defined breakpoint - Windows is checking for a solution to the problem". This happens in Windows 7 an

		To resolve this issue:

		Additionally to the call to Break(), insert the following call at the point where you want to start stepping through execution:

		System.Diagnostics.Debugger.Launch();

	2.4.11.7 Change: Text templates can no longer specify a version of a template language

		In Visual Studio 2008, you could specify a particular version of the template programming language. For example: <#@ template language="VBv3.5" #>.

		In Visual Studio 2010, the valid values for the language parameter are "VB" and "C#" and the version 4 compiler will always be used. If you specify "VBv3.5" or "C#v3.5", you will get a warning message that the version 4 compiler will be used instead.

		To resolve this issue:

		In Visual Studio 2008, the default compiler for text templates is 2.0. If you want to use language features that are not available in 2.0, you must specify v3.5 explicitly.

		If you want to use the same template in both versions of Visual Studio, then you can just ignore the warning message in Visual Studio 2010.

Also see "How To: Migrate T4 Text Templates from VS2008 to VS2010 Beta 2":
http://blogs.msdn.com/b/timfis/archive/2009/10/20/how-to-migrate-t4-text-templates-from-vs2008-to-vs2010-beta-2.aspx

## GAC Related Stuff
See ".NET 4 New GAC Locations/GacUtil":
http://davidyardy.com/blog/post/2011/03/02/NET-4-New-GAC-LocationsGacUtil.aspx

See "Gacutil.exe (Global Assembly Cache Tool)":
http://msdn.microsoft.com/en-us/library/ex0ss12c(v=vs.100).aspx
