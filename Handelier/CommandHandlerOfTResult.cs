using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public class CommandHandler<TResult>
    {
        public CommandHandler(Type command, Func<object, CancellationToken, Task<TResult>> handler)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Type Command { get; }
        public Func<object, CancellationToken, Task<TResult>> Handler { get; }
    }
}
