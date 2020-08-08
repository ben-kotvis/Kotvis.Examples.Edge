using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Module : IChangeTracking
    {
        public static object Locker;
        static Module()
        {
            Locker = new object();
        }
        public Module()
        {
            Publishers = new List<Publisher>();
        }
        public List<Publisher> Publishers { get; set; }


        ModuleState _state;
        public ModuleState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    IsChanged = true;
                }
            }
        }

        public bool IsChanged { get; private set; }
        public void AcceptChanges() => IsChanged = false;
    }
}
