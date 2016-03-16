namespace CarbonCore.Utils.Diagnostics
{
    public class ProfileStatistics
    {
        private double count;
        private double time;

        private long countPoints;
        private long timePoints;

        public ProfileStatistics(string name)
        {
            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public double Count
        {
            get
            {
                return this.count;
            }
        }

        public double Time
        {
            get
            {
                return this.time;
            }
        }

        public double AverageTime
        {
            get
            {
                if (this.timePoints > 0)
                {
                    return this.time / this.timePoints;
                }

                return 0;
            }
        }

        public double AverageCount
        {
            get
            {
                if (this.countPoints > 0)
                {
                    return this.count / this.countPoints;
                }

                return 0;
            }
        }

        public void AddCount(float number)
        {
            this.count += number;
            this.countPoints++;
        }

        public void AddTime(double ticks)
        {
            this.time += ticks;
            this.timePoints++;
        }

        public void Reset()
        {
            this.count = 0;
            this.countPoints = 0;

            this.time = 0;
            this.timePoints = 0;
        }
    }
}
