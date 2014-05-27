namespace CarbonCore.UtilsSDL.Math
{
    using System;
    using System.Linq;

    // http://stackoverflow.com/questions/8659351/2d-perlin-noise
    public static class PerlinNoise
    {
        private static readonly Random Random = new Random();

        private static int[] permutations;
        private static Vector2F[] gradiants;

        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        static PerlinNoise()
        {
            CalculatePermutations();
            CalculateGradients();
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public static void Reseed()
        {
            CalculatePermutations();
        }

        public static float Noise(Vector2L cell)
        {
            var total = 0f;

            var corners = new[] { new Vector2L(0, 0), new Vector2L(0, 1), new Vector2L(1, 0), new Vector2L(1, 1) };
            foreach (var n in corners)
            {
                var ij = cell + n;
                var uv = new Vector2F(cell.X - ij.X, cell.Y - ij.Y);

                var index = permutations[ij.X % permutations.Length];
                index = permutations[(index + ij.Y) % permutations.Length];

                var grad = gradiants[index % gradiants.Length];

                total += Q(uv.X, uv.Y) * grad.Dot(uv);
            }

            return Math.Max(Math.Min(total, 1f), -1f);
        }

        // ------------------------------------------------------------------- 
        // Private 
        // ------------------------------------------------------------------- 
        private static void CalculatePermutations()
        {
            // Re-initialize the array with the range of numbers
            permutations = Enumerable.Range(0, byte.MaxValue + 1).ToArray();

            // shuffle the array
            for (var i = 0; i < permutations.Length; i++)
            {
                var source = Random.Next(permutations.Length);

                var t = permutations[i];
                permutations[i] = permutations[source];
                permutations[source] = t;
            }
        }

        private static void CalculateGradients()
        {
            gradiants = new Vector2F[256];

            for (var i = 0; i < gradiants.Length; i++)
            {
                Vector2F gradient;

                do
                {
                    gradient = new Vector2F((float)(Random.NextDouble() * 2) - 1.0f, (float)(Random.NextDouble() * 2) - 1.0f);
                }
                while (gradient.Length >= 1.0f);

                gradiants[i] = gradient.Normalized;
            }
        }

        private static float Drop(float t)
        {
            t = Math.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }
    }
}
