using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public interface ICommandHandlerBuilder<TCommand, TResult>
    {
        ICommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand,CancellationToken,Task<TResult>>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        ICommandHandlerBuilder<TNext, TResult> Transform<TNext>(Func<Func<TNext, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe);
        ICommandHandlerBuilder<TNext> TransformReturn<TNext>(Func<Func<TNext,CancellationToken,Task>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        ICommandHandlerBuilder<TCommand> Return(Func<Func<TCommand,CancellationToken,Task>, Func<TCommand,CancellationToken,Task<TResult>>> pipe);
        void Handle(Func<TCommand,CancellationToken,Task<TResult>> handler);
    }
}
