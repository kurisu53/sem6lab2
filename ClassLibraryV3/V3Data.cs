using System;
using System.Numerics;

namespace ClassLibraryV3
{
    [Serializable]
    public abstract class V3Data
    {
        private string measures;
        public string Measures
        {
            get
            {
                return measures;
            }
            set
            {
                measures = value;
            }
        }

        private DateTime measureTime;
        public DateTime MeasureTime
        {
            get
            {
                return measureTime;
            }
            set
            {
                measureTime = value;
            }
        }

        public V3Data(string measures, DateTime time)
        {
            Measures = measures;
            MeasureTime = time;
        }

        public V3Data()
        {
            Measures = "Default measures";
            MeasureTime = DateTime.Now;
        }

        public abstract Vector2[] Nearest(Vector2 v);
        public abstract string ToLongString();
        public abstract string ToLongString(string format);

        public override string ToString()
        {
            return $"Measures: {Measures}. Measurement time: {MeasureTime}.";
        }

    }
}