using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Repository
{
    [Serializable]
    public abstract class Entity : INotifyPropertyChanging, INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void SendPropertyChanging(String propertyName)
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void SendPropertyChanged(String propertyName, object newValue)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (_changeList == null)
                return;

            if (_changeList.ContainsKey(propertyName))
            {
                _changeList.Remove(propertyName);
            }
            _changeList.Add(propertyName, newValue);
        }

        protected bool IsPropertyChanged(string name)
        {
            return _changeList != null && _changeList.ContainsKey(name);
        }

       
        private Dictionary<string, object> _changeList;

        internal Dictionary<string, object> GetChanges()
        {
            return _changeList;
        }

        private void StartTrackChanges()
        {
            if (_changeList != null)
            {
                throw new InvalidOperationException("This object is already tracking changes");
            }
            _changeList = new Dictionary<string, object>();
        }

        private bool _isAlreadySaved;

        internal bool IsAlreadySaved()
        {
            return _isAlreadySaved;
        }

        internal void MarkEntitySaved()
        {
            _isAlreadySaved = true;
        }

        protected virtual void OnLoaded()
        {
            StartTrackChanges();
        }

    }
}
