namespace CarbonCore.Utils.Lua.Logic
{
    using CarbonCore.Utils.Lua.Contracts;

    public class LuaObject : ILuaObject
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaObject(string name, object reference)
        {
            this.Name = name;
            this.Object = reference;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public object Object { get; private set; }
    }
}
