using Kotvis.Examples.Edge.Model;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Scheduler
{
    public class StateManager : IDisposable
    {
        private readonly List<Task> _tasks;
        private readonly IDictionary<string, CancellationTokenSource> _tokenDictionary;

        public StateManager()
        {
            _tasks = new List<Task>();
            _tokenDictionary = new Dictionary<string, CancellationTokenSource>();
        }

        public void AddTask(SchedulerRequest schedulerRequest, ModuleClient moduleClient, Func<SchedulerRequest, ModuleClient, CancellationToken, Task> action)
        {
            var tokenSource = new CancellationTokenSource();
            
            _tokenDictionary.Add(schedulerRequest.Context.ScheduleId, tokenSource);
            _tasks.Add(Task.Run(() => action(schedulerRequest, moduleClient, tokenSource.Token)));
        }

        public void CancelTask(string id)
        {
            _tokenDictionary[id].Cancel();
            _tokenDictionary.Remove(id);

            int numberOfTasks = _tasks.Count - 1;
            for (int i = numberOfTasks; i > -1; i--)
            {
                var task = _tasks[i];
                if (task.IsCompleted)
                {
                    task.Dispose();
                    _tasks.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            foreach(var item in _tokenDictionary)
            {
                item.Value.Cancel();
            }

            foreach(var item in _tasks)
            {   
                item.Dispose();
            }
        }
    }
}
