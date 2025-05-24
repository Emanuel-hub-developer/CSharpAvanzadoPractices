using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TareaAPI.Utilities
{
    public class TaskQueueManager
    {
        private readonly Queue<Func<Task>> _taskQueue = new();
        private bool _isProcessing = false;
        private readonly object _lock = new();

        private readonly Subject<string> _taskEvents = new(); 

        public IObservable<string> TaskEvents => _taskEvents.AsObservable(); 

        public void Enqueue(Func<Task> taskFunc)
        {
            lock (_lock)
            {
                _taskQueue.Enqueue(taskFunc);
                if (!_isProcessing)
                {
                    _isProcessing = true;
                    _ = ProcessQueue();
                }
            }
        }

        private async Task ProcessQueue()
        {
            while (true)
            {
                Func<Task> taskFunc;
                lock (_lock)
                {
                    if (_taskQueue.Count == 0)
                    {
                        _isProcessing = false;
                        return;
                    }
                    taskFunc = _taskQueue.Dequeue();
                }

                try
                {
                    await taskFunc();
                    _taskEvents.OnNext("Tarea procesada exitosamente");
                }
                catch (Exception ex)
                {
                    _taskEvents.OnNext($"Error procesando tarea: {ex.Message}");
                }
            }
        }
    }
}
