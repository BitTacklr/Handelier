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
    }
}
