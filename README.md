Holiday Labels and Lists (HLL) is an application I wrote in 2017 for a social services agency where I volunteer. It probably won’t be much help to anyone else without extensive modification, but I’m posting it in case the approaches I took to solving the problems I encountered might be useful.

The agency uses a service called VESTA, a database service that maintains information on the people my client serves. VESTA supplies reports providing data on my client’s holiday programs, detailing which services were provided and who they went to. The reports are used to generate several kinds of label and list documents required by these programs.

The process of generating the label and list documents (referred to hereafter as “output documents”) is not difficult, but doing it manually takes many hours of staff or volunteer time, hence the need for an application such as HLL.

### To build HLL:

HLL is a Winforms app, written in C-Sharp on Visual Studio Community 2017 edition. I’m running it with .NET Framework 4.6.1, but other versions should work as well. The steps required to build it are:

1. Clone the repository to some convenient folder: git <https://github.com/tonyrein/HolidayLabelsAndLists_Public.git> If you don't have git installed, download and extract the ZIP file.
2. Open HolidayLabelsAndLists.sln in Visual Studio. Examine the solution and project properties and change any paths to match the system you’re installing it on. You’ll probably need to change:
    1. OutputFolder in DAO → Properties → Settings. The label and list documents generated by HLL are placed into subfolders of this top-level folder.
    2. InitialVestaFolder in HolidayLabelsAndListsHelper → Properties → Settings. This is the initial location that will be shown in the file selection dialog when the user clicks “Process VESTA Reports.”
3. Edit the documentation html text (stored as a resource in HolidayLabelsAndLists) to match your environment.
4.  Make sure the following NuGet packages are installed (even though VisualStudio tells you they’re already installed, you may have to re-install them):
    1. DocX, by Cathal Coffey (<https://github.com/xceedsoftware/docx>). Used to write MS Word output documents.
    2. EPPlus, by Jan Kallman (<https://github.com/JanKallman/EPPlus>). Used to write MS Excel output documents.
    3. NPOI, by Tony Qu (<https://github.com/tonyqus/npoi>). Used to read the VESTA source reports.
    4. SharpZipLib ([http://www.icsharpcode.net](http://www.icsharpcode.net/)). A dependency of NPOI.
5. Build the solution (Ctrl-Shift-B). The output will be in &lt;solution folder&gt;\\HolidayLabelsAndLists\\bin\\Release. Double-click on the EXE file to run the program. If so desired, you can create an installer for HLL, but I didn’t bother with that – I simply copied the EXE and all the DLL files to a convenient location in the client’s network and created desktop shortcuts to the EXE.
