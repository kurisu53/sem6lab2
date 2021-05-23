using System;

namespace ClassLibraryV3
{
    [Serializable]
    public struct Grid1D // структура для хранения параметров сетки по одной оси
    {
        public float AxisStep { get; set; }
        public int NodesCount { get; set; }

        public Grid1D(float step, int count)
        {
            AxisStep = step;
            NodesCount = count;
        }

        public override string ToString()
        {
            return $"Step is {AxisStep}. {NodesCount} nodes.";
        }

        public string ToString(string format)
        {
            string AxisStepFormatted = String.Format(format, AxisStep);
            return $"Axis step is {AxisStepFormatted}. There are {NodesCount} nodes on this axis.";
        }
    }
}
