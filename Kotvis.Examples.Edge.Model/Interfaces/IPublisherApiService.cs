﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IPublisherApiService
    {
        Task Subscribe(DesiredPublisher desiredPublisher);
    }
}
