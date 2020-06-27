using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Publisher : IChangeTracking
    {
        public string Id { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        public string ErrorContext { get; set; }

        string _host;
        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    IsChanged = true;
                }
            }
        }

        int _port;
        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    IsChanged = true;
                }
            }
        }

        string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    IsChanged = true;
                }
            }
        }

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    IsChanged = true;
                }
            }
        }

        DesiredPublisherState _desiredState;
        public DesiredPublisherState DesiredState
        {
            get => _desiredState;
            set
            {
                if (_desiredState != value)
                {
                    _desiredState = value;
                    IsChanged = true;
                }
            }
        }

        ActualPublisherState _actualState;
        public ActualPublisherState ActualState
        {
            get => _actualState;
            set
            {
                if (_actualState != value)
                {
                    _actualState = value;
                    IsChanged = true;
                }
            }
        }

        string _subscriptionId;
        public string SubscriptionId
        {
            get => _subscriptionId;
            set
            {
                if (_subscriptionId != value)
                {
                    _subscriptionId = value;
                    IsChanged = true;
                }
            }
        }


        string _healthScheduleId;
        public string HealthScheduleId
        {
            get => _healthScheduleId;
            set
            {
                if (_healthScheduleId != value)
                {
                    _healthScheduleId = value;
                    IsChanged = true;
                }
            }
        }

        public bool IsChanged { get; private set; }
        public void AcceptChanges() => IsChanged = false;
    }
}
