using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public class CommandHandler<TMetadata>
    {
        public CommandHandler(Type command, Func<object, TMetadata, CancellationToken, Task> handler)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Type Command { get; }
        public Func<object, TMetadata, CancellationToken, Task> Handler { get; }

        public CommandHandler<TMetadata> Pipe(Func<Func<object, TMetadata, CancellationToken, Task>, Func<object, TMetadata, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new CommandHandler<TMetadata>(Command, pipe(Handler));
        }
    }
}
