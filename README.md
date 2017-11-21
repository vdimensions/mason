# Welcome to the **mason** project #


## What is mason? ##

This project was born out of my day-to-day struggle in managing the versioning and packaging process for my own projects. It answered the need for a consistent and easy-to-manage way to increase versions, create nuget packages and keep project metadata (like assembly title, project id, copyright, license and etc.) aligned between the package description and the project build artefact (AssemblyInfo). 

At the current state *mason* consists of 3 separate tools:

 - preprocessor
 - verman
 - packager

The tools are bundled each into a respective MSBuild target, which means one just needs to install the target's relevant nuget package in order for the given tool start working on the given project. 

There is no dependency among the tools themselves, you are free to use only one or all of the above. Continue reading to discover each tool purpose and specific configuration. 

Before dealing with details about the separate tools, there is one more important concept for mason's workings -- the `mason.properties` file.

### Mason properties ###

The `mason.properties` file is a standard text file, containing key-value pairs. Java developers should be familiar with this format. It roughly has the following structure:

    # the `#` sign denotes a comment
    key = "value"

    # the quotes are optional

The purpose of mason properties is to contain your project settings. For example you might want to have this kind of `mason.properties` in your project:

    version.build = 0
    version = 1.0.0.${version.build}
    id = "YourGreatProject"
    description = "Your great project's description"
    author = "Your name or your company name"
    copyright = "Copyright (c) ${author} YEAR"


## Mason Preprocessor ##

The mason preprocessor tool allows you to reuse project specific settings on more that one place in the project where the same setting is needed. Good candidates for such properties are the ones in the above example -- for instance, one may need to use the project's version during the build as well as during the packaging process.

You may have noticed some *expressions* between `${` and `}` sumbols in the above example. These expressions get substituted by a property value or environment variable having the name equal to the contents of the expression. For instance `${USER}` will return the current operating system's user name, because most OS-es define an environment variable `USER` holding the current operating system's account name.

In order to benefit from the different values we specify in the `mason.properties`, we need to understand mason templates.

### Mason templates ###

Mason templates are regular text files which can contain whaterver you want. 
What matters here, is that in the template files one may include expressions -- text defined betweeen `${` and `}`. This is the same expression syntax as in the mason property files.

In order to be recognized by mason, the template file must end with the `.template` extension.

During the `BeforeBuild` phase of your project, mason will locate all template files contained inside the project directory and its subdirectories, and will *expand* the expressions they may contain. This produces an **expanded file** which is placed beside its corresponding template. The expanded file has *exactly* the same name as that part of the template file which preceeds the `.template` extension. If a file that has the same name as the expaned file exists, it will be overwritten (which is actually the desired goal).

So, if we take the example `mason.properties` file from the above section, we could now use it to keep our nuget spec file and our AssemblyInfo file in sync.

To do this, we would have to create a `YourGreatProjectName.nuspec.template` file and use the expression syntax to populate the relevant properties. Below is an exceprt of the respective rows (**not** the entire nuspec template):

    <id>${id}</id>
    <version>${version}</version>
    <author>${author}</author>
    <copyright>${copyright}</copyright>

This will produce the expanded file `YourGreatProjectName.nuspec` having the respective values populated by mason: 

    <id>YourGreatProject</id>
    <version>1.0.0.0</version>
    <author>Your name or your company name</author>
    <copyright>Copyright (c) Your name or your company name YEAR</copyright>

If for instance, this is a C# project, you may want to keep this information in sync with the `AssemblyInfo.cs` file as well. So, the approach would be to copy it and rename it to `AssemblyInfo.cs.template`, then place it beside the original `AssemblyInfo.cs` file.

Below is an excerpt form the template with the relevant items reflected:

    [assembly: AssemblyTitle("${id}")]    
    [assembly: AssemblyCompany("${author}")]
    [assembly: AssemblyCopyright("${copyright}")]
    [assembly: AssemblyVersion("${verison}")]
    [assembly: AssemblyFileVersion("${verison")]
    [assembly: AssemblyInformationalVersion("${verison")]

And here is the produced result:

    [assembly: AssemblyTitle("YourGreatProject")]    
    [assembly: AssemblyCompany("Your name or your company name")]
    [assembly: AssemblyCopyright("Copyright (c) Your name or your company name YEAR")]
    [assembly: AssemblyVersion("1.0.0.0")]
    [assembly: AssemblyFileVersion("1.0.0.0")]
    [assembly: AssemblyInformationalVersion("1.0.0.0")]

Because the substitution occurs before the actual build process takes place, the expanded files will already be prepared for MSBuild to take them. 

> Beware! Instead of editing the expanded file, update its corresponding template file. Otherwise the changes will be overwritten when the file is expaned again. A good approach is to include a short notice in the beginning of the template file saying the file is automatically regenerated and user changes could be lost.


## Mason Version Manager ##

The version manager tool is triggered during the `AfterBuild` phase. Its purpose is to increase the project's version by updating the relevant `mason.properties` file. 

At the current state, mason supports only incremental version updaes. In order to tell mason how to change your version, you need to:

 - define a custom property in the `mason.properties` reflecting the version component to be increased. In the above example, we have explicitly defined a `versiion.build` property.

 - make mason aware of the version property. This is done by adding the following line to your `mason.properties` file: 

       mason-verman.version-property-to-update = "version.build"

   Now, mason will look for the `version.build` property and will increase its value after a successful build.


## Mason Packager ##

TODO
