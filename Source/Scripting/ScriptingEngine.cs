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
            this.globals = new ExpandoObject();
            dynamic foo = this.globals;
            foo.test = "abc";
            
            CSharpScript.EvaluateAsync("var foo = 0;", null, this.globals);

            //this.state.ContinueWithAsync()
        }
    }
}
