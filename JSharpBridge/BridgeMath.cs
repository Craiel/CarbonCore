namespace CarbonCore.JSharpBridge
{
    using System;

    public static class BridgeMath
    {
        private static readonly Random StaticRandom = new Random((int)DateTime.Now.Ticks);

        public static float PI
        {
            get
            {
                return (float)Math.PI;
            }
        }

        public static float Random()
        {
            return (float)StaticRandom.NextDouble();
        }

        public static int NextInt(this Random random, int maxValue = int.MaxValue)
        {
            return random.Next(maxValue);
        }

        public static bool NextBoolean(this Random random)
        {
            return random.Next(0, 1) == 1;
        }

        // http://stackoverflow.com/questions/218060/random-gaussian-variables
        public static double NextGaussian(this Random random, double mu = 0, double sigma = 1)
        {
            double u1 = random.NextDouble(); // these are uniform(0,1) random doubles
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
            return mu + sigma * randStdNormal; // random normal(mean,stdDev^2)
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static long NextLong(this Random random)
        {
            return random.Next();
        }

        public static bool IsFinite(double value)
        {
            throw new NotImplementedException();
        }

        public static int Round(float value)
        {
            throw new NotImplementedException();
        }

        public static int Round(double value)
        {
            throw new NotImplementedException();
        }

        public static double Signum(double var1)
        {
            throw new NotImplementedException();
        }

        public static double Cos(double var12)
        {
            throw new NotImplementedException();
        }

        public static double Sin(double var12)
        {
            throw new NotImplementedException();
        }

        public static double Atan2(double var11, double var22)
        {
            throw new NotImplementedException();
        }

        public static double Sqrt(double p0)
        {
            throw new NotImplementedException();
        }

        public static double Abs(double dotProduct)
        {
            throw new NotImplementedException();
        }

        public static int Abs(int dotProduct)
        {
            throw new NotImplementedException();
        }

        public static float Atan(double d)
        {
            throw new NotImplementedException();
        }

        public static T Max<T>(T f, T f1)
        {
            throw new NotImplementedException();
        }

        public static double Ceiling(double getHealth)
        {
            throw new NotImplementedException();
        }

        public static T Min<T>(T var26, T i)
        {
            throw new NotImplementedException();
        }

        public static float Pow(double d, double d1)
        {
            throw new NotImplementedException();
        }

        public static bool IsInfinite(double posX)
        {
            throw new NotImplementedException();
        }

        public static int FloatToRawIntBits(float textureV)
        {
            throw new NotImplementedException();
        }

        public static short ReverseBytes(short par1)
        {
            throw new NotImplementedException();
        }

        public static long DoubleToLongBits(double data)
        {
            throw new NotImplementedException();
        }

        public static int FloatToIntBits(float data)
        {
            throw new NotImplementedException();
        }

        public static string IntToString(int var18)
        {
            throw new NotImplementedException();
        }

        public static string LongToString(long nextLong, int i)
        {
            throw new NotImplementedException();
        }

        public static string ToHexString(int rarityColor)
        {
            throw new NotImplementedException();
        }

        public static int ReverseBytes(int par1)
        {
            throw new NotImplementedException();
        }

        public static bool boolValueOf(bool p0)
        {
            throw new NotImplementedException();
        }
    }
}
