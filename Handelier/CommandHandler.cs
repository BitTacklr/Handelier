using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public class CommandHandler
    {
        public CommandHandler(Type command, Func<object, CancellationToken, Task> handler)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Type Command { get; }
        public Func<object, CancellationToken, Task> Handler { get; }

        public CommandHandler Pipe(Func<Func<object, CancellationToken, Task>, Func<object, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new CommandHandler(Command, pipe(Handler));
        }
    }
}
