using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public class ResultCommandHandler<TMetadata, TResult>
    {
        public ResultCommandHandler(Type command, Func<object, TMetadata, CancellationToken, Task<TResult>> handler)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Type Command { get; }
        public Func<object, TMetadata, CancellationToken, Task<TResult>> Handler { get; }
    }
}
