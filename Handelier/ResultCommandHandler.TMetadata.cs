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

        public ResultCommandHandler<TMetadata, TResult> Pipe(Func<Func<object, TMetadata, CancellationToken, Task<TResult>>, Func<object, TMetadata, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new ResultCommandHandler<TMetadata, TResult>(Command, pipe(Handler));
        }
    }
}
