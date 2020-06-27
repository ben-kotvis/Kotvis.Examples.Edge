using DotNetty.Codecs;
using Kotvis.Examples.Edge.Model;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Scheduler
{
    public class MessageScheduler
    {
        public static async Task Create(SchedulerRequest request, ModuleClient moduleClient, CancellationToken cancellation)
        {
            do
            {
                try
                {
                    await Task.Delay(request.RunTime, cancellation);

                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request.Context));

                    var message = new Message(bytes);


                    Console.WriteLine($"Sending message to output for schedule id: {request.Context.ScheduleId}.");
                    await moduleClient.SendEventAsync(request.OutputName, message, cancellation);
                }
                catch(TaskCanceledException)
                {
                    Console.WriteLine($"Schedule Id: {request.Context.ScheduleId} was cancelled.");
                }
            }
            while (!cancellation.IsCancellationRequested && request.Repeat);
        }
    }
}
