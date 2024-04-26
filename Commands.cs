using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Microsoft.ClearScript.V8;
using System.Diagnostics;

[assembly: Autodesk.AutoCAD.Runtime.CommandClass(typeof(AcadPlugin.Commands))]

namespace AcadPlugin
{
    //Exposing AutoCAD .NET classes to JavaScript
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
}

