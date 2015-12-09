namespace CarbonCore.Utils.Unity.Logic.MeshGeneration
{
    using CarbonCore.Utils.Unity.Logic.Enums;

    using UnityEngine;

    public class AnchorUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Vector3 GetAnchorVector(AnchorPoint point, float width, float height)
        {
            switch (point)
            {
                case AnchorPoint.TopLeft:
                    {
                        return new Vector2(-width / 2.0f, height / 2.0f);
                    }

                case AnchorPoint.TopCenter:
                    {
                        return new Vector2(0.0f, height / 2.0f);
                    }

                case AnchorPoint.TopRight:
                    {
                        return new Vector2(width / 2.0f, height / 2.0f);
                    }

                case AnchorPoint.RightCenter:
                    {
                        return new Vector2(width / 2.0f, 0.0f);
                    }

                case AnchorPoint.BottomRight:
                    {
                        return new Vector2(width / 2.0f, -height / 2.0f);
                    }

                case AnchorPoint.BottomCenter:
                    {
                        return new Vector2(0.0f, -height / 2.0f);
                    }

                case AnchorPoint.BottomLeft:
                    {
                        return new Vector2(-width / 2.0f, -height / 2.0f);
                    }

                case AnchorPoint.LeftCenter:
                    {
                        return new Vector2(-width / 2.0f, 0.0f);
                    }

                default:
                    {
                        return Vector2.zero;
                    }
            }
        }
    }
}
