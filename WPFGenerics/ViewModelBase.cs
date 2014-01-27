using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WPFGenerics
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Handle the different types of events
        /// </summary>
        public enum EvenTypes
        {
            None = 0,
            MouseLeftButtonUp = 1,
            MouseLeftButtonDown = 2,
            MouseDoubleClick = 3,
            Enter = 4,
            Drop = 5,
            Delete = 6,
            MouseRightButtonUp = 7,
            MouseRightButtonDown = 8,
            AltLeftUp = 10
        }
        /// <summary>
        /// Type of the event triggered
        /// </summary>
        [XmlIgnore]
        public EvenTypes EventType { get; set; }
        /// <summary>
        /// On proper change event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Each time the property changes raise the event
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Set the changed property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            if (propertyName != null)
                OnPropertyChanged(propertyName);

            return true;
        }
        /// <summary>
        /// Invoke property name using our trigger
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="sender"></param>
        public void InvokePropertyChanged(string propertyName, object sender = null)
        {
            if (sender == null) sender = this;
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));

            // is a method? invoke it
            var method = GetType().GetMethod(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
                method.Invoke(this, null);
        }

        public ViewModelBase()
        {
            Initialize();
            var parseable = this as IManageable;
            if (parseable != null)
            {
                parseable.InstanceID = Guid.NewGuid();
                ViewModelManager.AddViewModel(this);
            }
        }

        /// <summary>
        /// Initiaize procedure asynchronous
        /// </summary>
        public virtual async void Initialize()
        {
            // Override
        }
    }
}
