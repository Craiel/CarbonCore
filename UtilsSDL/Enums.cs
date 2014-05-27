namespace CarbonCore.UtilsSDL
{
    using System;

    [Serializable]
    public enum IntersectionType
    {
        None,
        Intersected,
        Contained
    }

    [Serializable]
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}
