namespace CarbonCore.Utils.MathUtils
{
    public static class RectUtils
    {
        /*public static IList<Vector2L> GenerateCircleRectOffsets(this RectL rect, ushort distance, bool fill = true)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("GenerateRectAround called with distance 0");
            }

            IList<Vector2L> results = new List<Vector2L>();

            // Project a cone in all directions
            rect.GenerateConeRectOffsets(results, GenerationDirection.Up, distance, fill);
            rect.GenerateConeRectOffsets(results, GenerationDirection.Down, distance, fill);
            rect.GenerateConeRectOffsets(results, GenerationDirection.Left, distance, fill, skipSides: true);
            rect.GenerateConeRectOffsets(results, GenerationDirection.Right, distance, fill, skipSides: true);

            return results;
        }

        public static int GenerateConeRectOffsets(this RectL rect, IList<Vector2L> list, GenerationDirection direction, ushort distance, bool fill = true, bool skipSides = false)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("GenerateRectAround called with distance 0");
            }

            var offset = new Vector2L(rect.LeftTop);
            int count = 0;
            for (int dY = 1; dY <= distance; dY++)
            {
                // Center pieces
                switch (direction)
                {
                    case GenerationDirection.Up:
                        {
                            offset.X = rect.Left;
                            offset.Y -= rect.Bottom;
                            break;
                        }

                    case GenerationDirection.Down:
                        {
                            offset.X = rect.Left;
                            offset.Y += rect.Bottom;
                            break;
                        }

                    case GenerationDirection.Left:
                        {
                            offset.Y = rect.Top;
                            offset.X -= rect.Right;
                            break;
                        }

                    case GenerationDirection.Right:
                        {
                            offset.Y = rect.Top;
                            offset.X += rect.Right;
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                if (fill || dY == distance)
                {
                    list.Add(new Vector2L(offset));
                    count++;
                }

                // Sides
                int maxSides = 1 * dY;
                for (int dX = 1; dX <= maxSides; dX++)
                {
                    if (!fill && dX < maxSides)
                    {
                        continue;
                    }

                    if (skipSides && dX == maxSides)
                    {
                        continue;
                    }

                    switch (direction)
                    {
                        case GenerationDirection.Down:
                        case GenerationDirection.Up:
                            {
                                offset.X = rect.Left - (dX * rect.Right);
                                list.Add(new Vector2L(offset));

                                offset.X = rect.Left + (dX * rect.Right);
                                list.Add(new Vector2L(offset));

                                break;
                            }

                        case GenerationDirection.Left:
                        case GenerationDirection.Right:
                            {
                                offset.Y = rect.Top - (dX * rect.Bottom);
                                list.Add(new Vector2L(offset));

                                offset.Y = rect.Top + (dX * rect.Bottom);
                                list.Add(new Vector2L(offset));
                                break;
                            }

                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }

                    count += 2;
                }
            }

            return count;
        }*/
    }
}
