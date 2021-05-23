using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace ClassLibraryV3
{
    [Serializable]
    public struct DataItem : ISerializable // структура для хранения значения поля в точке
    {
        public Vector2 Coord { get; set; }
        public double EMField { get; set; }

        public DataItem(Vector2 coord, double field)
        {
            Coord = coord;
            EMField = field;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Coord_X", Coord.X);
            info.AddValue("Coord_Y", Coord.Y);
            info.AddValue("EMField", EMField);
        }

        public DataItem(SerializationInfo info, StreamingContext context)
        {
            float x = info.GetSingle("Coord_X");
            float y = info.GetSingle("Coord_Y");
            Coord = new Vector2(x, y);
            EMField = (double)info.GetValue("EMField", typeof(double));
        }

        public override string ToString()
        {
            return $"EM field at the point ({Coord.X}, {Coord.Y}) is {EMField}.\n";
        }

        public string ToString(string format)
        {
            string CoordXFormatted = String.Format(format, Coord.X);
            string CoordYFormatted = String.Format(format, Coord.Y);
            string EMFieldFormatted = String.Format(format, EMField);
            return $"EM field at the point ({CoordXFormatted}; {CoordYFormatted}) is {EMFieldFormatted}.\n";
        }
    }
}
