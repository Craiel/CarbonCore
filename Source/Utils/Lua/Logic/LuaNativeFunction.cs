namespace CarbonCore.Utils.Lua.Logic
{
    using CarbonCore.Utils.Lua.Contracts;

    using NLua;

    public class LuaNativeFunction : ILuaRuntimeFunction
    {
        private readonly string contents;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaNativeFunction(string contents)
        {
            this.Name = null;
            this.contents = contents;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; }

        public void Register(Lua target)
        {
            target.DoString(this.contents);
        }
    }
}
