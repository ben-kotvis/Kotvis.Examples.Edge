using DotNetty.Codecs;
using Kotvis.Examples.Edge.Model;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Kotvis.Examples.Edge.Scheduler
{
    public class MessageScheduler
    {
        public static async Task Create(Module module, ModuleClient moduleClient, CancellationToken cancellation)
        {
            do
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellation);

                var currentDateTime = DateTimeOffset.Now;

                var completedScheduleIds = new List<string>();
                try
                {
                    foreach (var schedule in module.Schedules.Where(i => i.NextRunTime.Subtract(currentDateTime) < TimeSpan.FromSeconds(1) || i.NextRunTime < currentDateTime))
                    {
                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schedule.Request.Context));
                        var message = new Message(bytes);
                        Console.WriteLine($"Sending message to output for schedule id: {schedule.Request.Context.ScheduleId}.");
                        var messageTime = DateTimeOffset.Now;
                        await moduleClient.SendEventAsync(schedule.Request.OutputName, message, cancellation);

                        if (schedule.Request.Repeat)
                        {
                            Monitor.Enter(module);
                            schedule.Advance(messageTime);
                            Monitor.Exit(module);
                        }
                        else
                        {
                            completedScheduleIds.Add(schedule.Request.Context.ScheduleId);
                        }
                    }

                    if (completedScheduleIds.Any())
                    {
                        Monitor.Enter(module);
                        module.Schedules.RemoveAll(i => completedScheduleIds.Contains(i.Request.Context.ScheduleId));
                        Monitor.Exit(module);
                    }
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Scheduler cancelled {ex}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Scheduler error {ex}");
                }

            }
            while (!cancellation.IsCancellationRequested);
        }
    }
}
