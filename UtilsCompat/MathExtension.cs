namespace CarbonCore.Utils.Compat
{
    using System;
    using System.Collections.Generic;
    
    public static class MathExtension
    {
        public static readonly double PiOver2 = Math.PI / 2;
        public static readonly double TwoPi = Math.PI * 2;

        public static float DegreesToRadians(float degree)
        {
            return (float)(degree * (Math.PI / 180.0f));
        }

        public static float RadiansToDegrees(float radian)
        {
            return (float)(radian * (180.0f / Math.PI));
        }

        public static float Clamp(this int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        
        public static IList<int> ComputePrimes(int max)
        {
            var primes = new bool[max + 1];
            var sqrt = (int)Math.Sqrt(max);
            for (int x = 1; x < sqrt; x++)
            {
                var squareX = x * x;
                for (int y = 1; y <= sqrt; y++)
                {
                    var squareY = y * y;
                    var n = (4 * squareX) + squareY;
                    if (n <= max && (n % 12 == 1 || n % 12 == 5))
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) + squareY;
                    if (n <= max && n % 12 == 7)
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) - squareY;
                    if (x > y && n <= max && n % 12 == 11)
                    {
                        primes[n] ^= true;
                    }
                }
            }

            var primeList = new List<int> { 2, 3 };
            for (int i = 5; i <= sqrt; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                    int square = i * i;
                    for (int k = square; k < max; k += square)
                    {
                        primes[k] = false;
                    }
                }
            }

            for (int i = sqrt + 1; i <= max; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                }
            }

            return primeList;
        }
    }
}
