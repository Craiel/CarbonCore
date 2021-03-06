﻿namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;

    // http://stackoverflow.com/questions/8659351/2d-perlin-noise
    public static class PerlinNoise
    {
        private static readonly Random Random = new Random();

        private static int[] permutations;
        private static Vector2[] gradiants;

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

        public static float Noise(Point cell)
        {
            var total = 0f;

            var corners = new[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) };
            foreach (var n in corners)
            {
                var ij = cell + n;
                var uv = new Vector2(cell.X - ij.X, cell.Y - ij.Y);

                var index = permutations[ij.X % permutations.Length];
                index = permutations[(index + ij.Y) % permutations.Length];

                var grad = gradiants[index % gradiants.Length];

                total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
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
            gradiants = new Vector2[256];

            for (var i = 0; i < gradiants.Length; i++)
            {
                Vector2 gradient;

                do
                {
                    gradient = new Vector2((float)(Random.NextDouble() * 2) - 1.0f, (float)(Random.NextDouble() * 2) - 1.0f);
                }
                while (gradient.Length() >= 1.0f);

                gradient.Normalize();
                gradiants[i] = gradient;
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
