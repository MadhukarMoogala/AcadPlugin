# How to Run Javascript Routines in Autocad .Net Plugin Using ClearScript Library

![Static Badge](https://img.shields.io/badge/AutoCAD-2025-blue)
![Static_Badge](https://img.shields.io/badge/NET-8.0-blue)
![Static Badge](https://img.shields.io/badge/build-passing-brightgreen)

The project  demonstrates a well-structured approach to running JavaScript routines in an AutoCAD .NET plugin using the [ClearScript | Add scripting to your .NET applications quickly and easily.](https://microsoft.github.io/ClearScript/)

- **Exposing AutoCAD .NET Classes:** Enables access to AutoCAD functionality from JavaScript scripts.
- **Executing JavaScript Functions:** Allows you to run JavaScript functions within your AutoCAD application.
- **Calling C# Functions from JavaScript:** Facilitates interaction between your JavaScript code and C# functions.



```csharp
public class PrintMessage
{
    private static Editor? _ed
    {
        get
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                return doc.Editor;
            }
            return null;
        }
    }
    public static void Print(string message)
    {
        _ed?.WriteMessage($"{message}\n");
    }
}

public class Commands
{
    private const string _script = @"
                        function square(x) {
                            return x*x;
                        }";
    
    //Execute JS functions in DotNet runtime


    [CommandMethod("JSRoutine")]
    public static void JSRoutine()
    {
        Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        if (doc is null) return;
        Editor ed = doc.Editor;

        // Create aV8ScriptEngine instance, 
        using (var engine = new V8ScriptEngine())
        {
            engine.AccessContext = typeof(Commands);

             //Now expose the PrintMessage class from .NET to the script engine,
            // and then execute a JavaScript statement that writes a message to the AutoCAD Editor.
            engine.AddHostType("PrintMessage", typeof(PrintMessage));              
            engine.Execute("PrintMessage.Print('Hello from JavaScript!');");

            //Call the JS function square and retrieve the result.
            engine.Execute(_script);
            dynamic? result = engine.Script.square(5);
            ed.WriteMessage($"{Convert.ToString(result)}\n");
            

            //Execute the JS code and print the result to the AutoCAD Editor                
            engine.Execute("function print(x) {PrintMessage.Print(x); }");
            engine.Script.print(DateTime.Now.DayOfWeek.ToString());

            //Calling C# Functions from JavaScript:              
            engine.AddHostObject("Greet", new Func<string, string>((name) => $"Hello, {name}!"));              
            engine.Execute("var message = Greet('World'); PrintMessage.Print(message);");

            // examine a script object            
            engine.Execute("var person = { name: 'Fred', age: 5 }");
            ed.WriteMessage($"From Script: \n{Convert.ToString(engine.Script.person.name)}");
        }
    }
}
```

### 

### Requirements

- .NET 8.0 

- AutoCAD 2025 & NuGet package: [AutoCAD.NET](https://www.nuget.org/packages/AutoCAD.NET)

- Visual Studio 2022 17.8

- ClearScript library NuGet package: [Microsoft.ClearScript](https://www.nuget.org/packages/Microsoft.ClearScript)
  
  

### Build

```bash
git clone https://github.com/MadhukarMoogala/AcadPlugin.git
cd AcadPlugin
dotnet build AcadPlugin.csproj -a x64 -c Debug
```

### Usage

- NETLOAD the AcadPlugin.dll

- Execute `JSROUTINE` command in AutoCAD Command Line.

  ![result](https://github.com/MadhukarMoogala/AcadPlugin/assets/6602398/8be874c7-aa9f-4808-b3d3-70040e4199ca)


### **License**

[MIT License](https://github.com/MadhukarMoogala/AcadPlugin/tree/main?tab=MIT-1-ov-file)

### Authors

Madhukar Moogala (@galakar)
