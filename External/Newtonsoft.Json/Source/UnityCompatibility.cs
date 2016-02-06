namespace Newtonsoft.Json
{
    public static class UnityCompatibility
    {
        public static string nameof(object other)
        {
            return other.GetType().ToString();
        }
    }
}
