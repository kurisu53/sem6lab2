using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace ClassLibraryV3
{
    [Serializable]
    public class V3MainCollection : IEnumerable<V3Data>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private List<V3Data> V3DataItems;

        // события

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        // свойства

        public bool WasChanged
        {
            get; set;
        }

        public int Count
        {
            get
            {
                return V3DataItems.Count;
            }
        }

        // конструктор

        public V3MainCollection()
        {
            V3DataItems = new List<V3Data>();
        }

        // обработчики событий

        public void OnCollectionChanged(NotifyCollectionChangedAction act)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void OnPropertyChanged(string str = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(str));
        }

        // индексатор

        public V3Data this[int index]
        {
            get { return V3DataItems[index]; }
            set
            {
                V3DataItems[index] = value;
            }
        }

        // свойства с LINQ-запросами

        public int MinMeasuresCount
        {
            get
            {
                IEnumerable<int> gridMeasuresCounts = from data in (from item in V3DataItems
                                                                    where item is V3DataOnGrid
                                                                    select (V3DataOnGrid)item)
                                                      select data.XGrid.NodesCount * data.YGrid.NodesCount;

                IEnumerable<int> collMeasuresCounts = from data in (from item in V3DataItems
                                                                    where item is V3DataCollection
                                                                    select (V3DataCollection)item)
                                                      select data.DataItems.Count();

                IEnumerable<int> MeasuresCounts = gridMeasuresCounts.Union(collMeasuresCounts);

                return MeasuresCounts.Min();
            }
        }

        public float MaxDistance
        {
            get
            {
                if (V3DataItems.Count != 0)
                {
                    IEnumerable<V3DataCollection> grids = from data in (from item in V3DataItems
                                                                        where item is V3DataOnGrid
                                                                        select (V3DataOnGrid)item)
                                                          select (V3DataCollection)data;

                    IEnumerable<V3DataCollection> collections = from data in (from item in V3DataItems
                                                                              where item is V3DataCollection
                                                                              select (V3DataCollection)item)
                                                                select data;

                    IEnumerable<V3DataCollection> items = grids.Union(collections);

                    IEnumerable<Vector2> Coords = from data in items
                                                  from elem in data
                                                  select elem.Coord;

                    float result = (from coord1 in Coords
                                    select (from coord2 in Coords
                                            select Vector2.Distance(coord1, coord2)).Max()).Max();

                    return result;
                }
                else
                    return 0.0f;
            }
        }

        public IEnumerable<DataItem> Duplicates
        {
            get
            {
                IEnumerable<V3DataCollection> grids = from data in (from item in V3DataItems
                                                                    where item is V3DataOnGrid
                                                                    select (V3DataOnGrid)item)
                                                      select (V3DataCollection)data;

                IEnumerable<V3DataCollection> collections = from data in (from item in V3DataItems
                                                                          where item is V3DataCollection
                                                                          select (V3DataCollection)item)
                                                            select data;

                IEnumerable<V3DataCollection> items = grids.Union(collections);

                var groups = from g in (from data in items
                                        from elem in data
                                        group elem by elem.Coord)
                             where g.Count() > 1
                             select g;

                IEnumerable<DataItem> result = from DataItem item in (from g in groups
                                                                      select g.ToList())
                                               select item;

                return result;
            }
        }

        // реализация интерфейса IEnumerable<V3Data>

        public IEnumerator<V3Data> GetEnumerator()
        {
            return V3DataItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // методы сериализации

        public void Save(string filename)
        {
            FileStream fstream = null;

            try
            {
                if (File.Exists(filename))
                    fstream = File.OpenWrite(filename);
                else
                    fstream = File.Create(filename);
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(fstream, V3DataItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Serialization failed. " + ex.Message);
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
                WasChanged = false;
                OnPropertyChanged("WasChanged");
            }
        }

        public void Load(string filename)
        {
            FileStream fstream = null;

            try
            {
                fstream = File.OpenRead(filename);
                BinaryFormatter binFormatter = new BinaryFormatter();
                V3DataItems = (List<V3Data>)binFormatter.Deserialize(fstream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Deserialization failed. " + ex.Message);
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
                WasChanged = true;
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                OnPropertyChanged("WasChanged");
                OnPropertyChanged("Count");
                OnPropertyChanged("MaxDistance");
            }
        }

        // методы для работы с данными

        public void Add(V3Data item)
        {
            try
            {
                V3DataItems.Add(item);
                WasChanged = true;
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                OnPropertyChanged("WasChanged");
                OnPropertyChanged("Count");
                OnPropertyChanged("MaxDistance");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Adding an item to MC failed. " + ex.Message);
            }
        }

        public bool Remove(string id, DateTime date)
        {
            bool flag = false;
            foreach (V3Data element in V3DataItems.ToList())
            {
                if (element.Measures == id && element.MeasureTime == date)
                {
                    V3DataItems.Remove(element);
                    flag = true;
                    OnPropertyChanged("WasChanged");
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove);
                    OnPropertyChanged("Count");
                }
            }
            return flag;
        }

        public void AddDefaults()
        {
            Grid1D XGrid0 = new Grid1D(0.0f, 0);
            Grid1D YGrid0 = new Grid1D(0.0f, 0);
            Grid1D XGrid1 = new Grid1D(150.0f, 3);
            Grid1D YGrid1 = new Grid1D(180.0f, 3);
            Grid1D XGrid2 = new Grid1D(520.0f, 5);
            Grid1D YGrid2 = new Grid1D(350.0f, 2);

            V3DataOnGrid Grid0 = new V3DataOnGrid("Empty grid", DateTime.Now, XGrid0, YGrid0);
            V3DataOnGrid Grid1 = new V3DataOnGrid("Grid 1", DateTime.Now, XGrid1, YGrid1);
            Grid1.InitRandom(50.0, 150.0);
            V3DataOnGrid Grid2 = new V3DataOnGrid("Grid 2", DateTime.Now, XGrid2, YGrid2);
            Grid2.InitRandom(500.0, 600.0);

            V3DataCollection Collection0 = new V3DataCollection("Empty collection", DateTime.Now);
            V3DataCollection Collection1 = new V3DataCollection("Collection 1", DateTime.Now);
            Collection1.InitRandom(6, 70.0f, 30.0f, 300.0, 450.0);
            V3DataCollection Collection2 = new V3DataCollection("Collection 2", DateTime.Now);
            Collection2.InitRandom(10, 100.0f, 180.0f, 1500.0, 2000.0);

            V3DataItems.Add(Grid0);
            V3DataItems.Add(Grid1);
            V3DataItems.Add(Grid2);
            V3DataItems.Add(Collection0);
            V3DataItems.Add(Collection1);
            V3DataItems.Add(Collection2);

            OnCollectionChanged(NotifyCollectionChangedAction.Add);
            OnPropertyChanged("Count");
            OnPropertyChanged("MaxDistance");
            WasChanged = true;
        }

        public void AddDefaultDataCollection()
        {
            V3DataCollection Collection = new V3DataCollection("Default collection", DateTime.Now);
            Collection.InitRandom(5, 500.0f, 1000.0f, 200.0, 300.0);
            this.Add(Collection);
        }

        public void AddDefaultDataOnGrid()
        {
            Grid1D XGrid = new Grid1D(250.0f, 5);
            Grid1D YGrid = new Grid1D(300.0f, 4);
            V3DataOnGrid Grid = new V3DataOnGrid("Default grid", DateTime.Now, XGrid, YGrid);
            Grid.InitRandom(400.0, 700.0);
            this.Add(Grid);
        }

        public void AddElementFromFile(string filename)
        {
            try
            {
                V3DataCollection Collection = new V3DataCollection(filename);
                this.Add(Collection);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Adding an item from file failed. " + ex.Message);
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (V3Data element in V3DataItems)
            {
                str += element.ToString();
            }
            return str;
        }

        public string ToLongString(string format)
        {
            string str = "";
            foreach (V3Data element in V3DataItems)
            {
                str += element.ToLongString(format);
            }
            return str;
        }
    }
}