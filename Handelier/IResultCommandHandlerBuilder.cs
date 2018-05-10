using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public interface IResultCommandHandlerBuilder<TCommand, TResult>
    {
        IResultCommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand,CancellationToken,Task<TResult>>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        IResultCommandHandlerBuilder<TNext, TResult> Transform<TNext>(Func<Func<TNext, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe);
        ICommandHandlerBuilder<TNext> TransformReturn<TNext>(Func<Func<TNext,CancellationToken,Task>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        ICommandHandlerBuilder<TCommand> Return(Func<Func<TCommand,CancellationToken,Task>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        void Handle(Func<TCommand,CancellationToken,Task<TResult>> handler);
    }
}
