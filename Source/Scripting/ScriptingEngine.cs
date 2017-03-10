namespace CarbonCore.Scripting
{
    using System.Dynamic;

    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    public class ScriptingEngine
    {
        private ExpandoObject globals;

        public void Execute()
        {
            var scriptOptions = ScriptOptions.Default.AddReferences(typeof(ScriptingEngine).Assembly).AddImports("CarbonCore.Scripting", "CarbonCore.Scripting");

            this.globals = new ExpandoObject();
            dynamic foo = this.globals;
            foo.test = "abc";

            CSharpScript.EvaluateAsync("System.Console.WriteLine(\"Test\" + test);", scriptOptions, this.globals);

            //this.state.ContinueWithAsync()
        }
    }
}
