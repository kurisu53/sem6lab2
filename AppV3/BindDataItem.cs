using System.ComponentModel;
using System.Numerics;
using ClassLibraryV3;

namespace AppV3
{
    class BindDataItem : IDataErrorInfo, INotifyPropertyChanged
    {
        private float xCoord, yCoord;
        private double field;

        public V3DataCollection VCollection;
        public event PropertyChangedEventHandler PropertyChanged;

        // свойства

        public float XCoord
        {
            get
            {
                return xCoord;
            }
            set
            {
                xCoord = value;
                OnPropertyChanged("XCoord");
                OnPropertyChanged("YCoord");
            }
        }

        public float YCoord
        {
            get
            {
                return yCoord;
            }
            set
            {
                yCoord = value;
                OnPropertyChanged("XCoord");
                OnPropertyChanged("YCoord");
            }
        }

        public double Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged("Field");
            }
        }

        public BindDataItem(ref V3DataCollection Coll)
        {
            VCollection = Coll;
        }

        // реализация интерфейса INotifyPropertyChanged

        public void OnPropertyChanged(string str = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(str)); 
        }

        // реализация интерфейса IDataErrorInfo

        public string this[string property]
        {
            get
            {
                string msg = null;
                
                switch(property)
                {
                    case "XCoord":
                        Vector2 NewCoordX = new Vector2(XCoord, YCoord);
                        foreach (DataItem item in VCollection)
                        {
                            if (item.Coord.Equals(NewCoordX))
                                msg = "Duplicate coordinates!";
                        }
                        break;
                    case "YCoord":
                        Vector2 NewCoordY = new Vector2(XCoord, YCoord);
                        foreach (DataItem item in VCollection)
                        {
                            if (item.Coord.Equals(NewCoordY))
                                msg = "Duplicate coordinates!";
                        }
                        break;
                    case "Field":
                        if (Field <= 0)
                            msg = "Field value is 0 or negative";
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }

        public string Error
        {
            get
            {
                return "Error text";
            }
        }

        // добавление элемента
        public void AddDataItem()
        {
            Vector2 NewCoord = new Vector2(XCoord, YCoord);
            DataItem item = new DataItem(NewCoord, Field);
            VCollection.Add(item);
            OnPropertyChanged("XCoord");
            OnPropertyChanged("YCoord");
            OnPropertyChanged("Field");
        }
    }
}
