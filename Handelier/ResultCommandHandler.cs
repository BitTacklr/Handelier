using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public class ResultCommandHandler<TResult>
    {
        public ResultCommandHandler(Type command, Func<object, CancellationToken, Task<TResult>> handler)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Type Command { get; }
        public Func<object, CancellationToken, Task<TResult>> Handler { get; }

        public ResultCommandHandler<TResult> Pipe(Func<Func<object, CancellationToken, Task<TResult>>, Func<object, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new ResultCommandHandler<TResult>(Command, pipe(Handler));
        }
    }
}
