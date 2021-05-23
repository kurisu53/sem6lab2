using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;

namespace ClassLibraryV3
{
    [Serializable]
    public class V3DataOnGrid : V3Data, IEnumerable<DataItem>, ISerializable // класс для значений поля на равномерной сетке
    {
        public Grid1D XGrid { get; set; }
        public Grid1D YGrid { get; set; }
        public double[,] EMValues { get; set; }

        public V3DataOnGrid(string measures, DateTime time, Grid1D x, Grid1D y) : base(measures, time)
        {
            XGrid = x;
            YGrid = y;
            EMValues = new double[XGrid.NodesCount, YGrid.NodesCount];
        }

        // реализация интерфейса IEnumerable<DataItem>
        public IEnumerator<DataItem> GetEnumerator()
        {
            Vector2 coord;
            double field;

            for (int i = 0; i < XGrid.NodesCount; i++)
            {
                for (int j = 0; j < YGrid.NodesCount; j++)
                {
                    coord.X = XGrid.AxisStep * i;
                    coord.Y = YGrid.AxisStep * j;
                    field = EMValues[i, j];
                    yield return new DataItem(coord, field);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // реализация интерфейса ISerializable
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Measures", Measures);
            info.AddValue("MeasureTime", MeasureTime);
            info.AddValue("XStep", XGrid.AxisStep);
            info.AddValue("XNodes", XGrid.NodesCount);
            info.AddValue("YStep", YGrid.AxisStep);
            info.AddValue("YNodes", YGrid.NodesCount);
            info.AddValue("EMValues", EMValues);
        }

        public V3DataOnGrid(SerializationInfo info, StreamingContext context) :
            base((string)info.GetValue("Measures", typeof(string)), (DateTime)info.GetValue("MeasureTime", typeof(DateTime)))
        {
            float XStep = info.GetSingle("XStep");
            int XNodes = (int)info.GetValue("XNodes", typeof(int));
            float YStep = info.GetSingle("YStep");
            int YNodes = (int)info.GetValue("YNodes", typeof(int));
            XGrid = new Grid1D(XStep, XNodes);
            YGrid = new Grid1D(YStep, YNodes);
            EMValues = new double[XNodes, YNodes];
            EMValues = (double[,])info.GetValue("EMValues", typeof(double[,]));
        }

        // инициализация значений поля в узлах сетки случайными числами
        public void InitRandom(double minValue, double maxValue)
        {
            Random rand = new Random();
            for (int i = 0; i < XGrid.NodesCount; i++)
                for (int j = 0; j < YGrid.NodesCount; j++)
                    EMValues[i, j] = rand.NextDouble() * (maxValue - minValue) + minValue;
        }

        // преобразование к типу V3DataCollection
        public static explicit operator V3DataCollection(V3DataOnGrid data)
        {
            V3DataCollection vcollection = new V3DataCollection(data.Measures, data.MeasureTime);
            Vector2 coord;
            double field;
            for (int i = 0; i < data.XGrid.NodesCount; i++)
                for (int j = 0; j < data.YGrid.NodesCount; j++)
                {
                    coord.X = data.XGrid.AxisStep * i;
                    coord.Y = data.YGrid.AxisStep * j;
                    field = data.EMValues[i, j];
                    vcollection.DataItems.Add(new DataItem(coord, field));
                }
            return vcollection;
        }

        public override Vector2[] Nearest(Vector2 v)
        {
            List<Vector2> NodesList = new List<Vector2>();
            float longside;
            if (XGrid.AxisStep < YGrid.AxisStep) // за первоначальный минимум принимаем большую из сторон ячейки сетки
            {
                longside = YGrid.AxisStep;
            }
            else
            {
                longside = XGrid.AxisStep;
            }
            float min = longside;

            Vector2 currentNode;
            for (int i = 0; i < XGrid.NodesCount; i++)
                for (int j = 0; j < YGrid.NodesCount; j++)
                {
                    currentNode.X = XGrid.AxisStep * i;
                    currentNode.Y = YGrid.AxisStep * j;
                    if (Vector2.Distance(currentNode, v) < min)
                    // если нашли более близкий узел, перезаполняем список
                    {
                        min = Vector2.Distance(currentNode, v);
                        NodesList.Clear();
                        NodesList.Add(currentNode);
                    }
                    else if (Math.Abs(Vector2.Distance(currentNode, v) - min) <= Math.Abs(min * 0.000001))
                    // проверка на равенство чисел с плавающей точкой
                    // Если есть еще один узел на минимальном расстоянии, добавляем его
                    {
                        NodesList.Add(currentNode);
                    }
                }

            return NodesList.ToArray();
        }

        public override string ToString()
        {
            return $"\n{base.ToString()} XGrid: {XGrid} YGrid: {YGrid}\n";
        }

        public override string ToLongString()
        {
            string str = "";
            double xCoord, yCoord;
            for (int i = 0; i < XGrid.NodesCount; i++)
                for (int j = 0; j < YGrid.NodesCount; j++)
                {
                    xCoord = XGrid.AxisStep * i;
                    yCoord = YGrid.AxisStep * j;
                    str += $"EM field at the point ({xCoord}, {yCoord}) is {EMValues[i, j]}.\n";
                }
            return $"{this}\n{str}";
        }

        public override string ToLongString(string format)
        {
            string str = "";
            double xCoord, yCoord;
            string xCoordFormatted, yCoordFormatted, EMValuesFormatted;
            for (int i = 0; i < XGrid.NodesCount; i++)
                for (int j = 0; j < YGrid.NodesCount; j++)
                {
                    xCoord = XGrid.AxisStep * i;
                    yCoord = YGrid.AxisStep * j;
                    xCoordFormatted = String.Format(format, xCoord);
                    yCoordFormatted = String.Format(format, yCoord);
                    EMValuesFormatted = String.Format(format, EMValues[i, j]);
                    str += $"EM field at the point ({xCoordFormatted}, {yCoordFormatted}) is {EMValuesFormatted}.\n";
                }
            return $"\nV3DataOnGrid. {base.ToString()} XGrid: {XGrid.ToString(format)} YGrid: {YGrid.ToString(format)}\n{str}";
        }
    }
}