using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public interface IResultCommandHandlerBuilder<TCommand, TMetadata, TResult>
    {
        IResultCommandHandlerBuilder<TCommand, TMetadata, TResult> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe);
        IResultCommandHandlerBuilder<TNext, TMetadata, TResult> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe);
        ICommandHandlerBuilder<TNext, TMetadata> TransformReturn<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe);
        ICommandHandlerBuilder<TCommand, TMetadata> Return(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe);
        void Handle(Func<TCommand, TMetadata, CancellationToken, Task<TResult>> handler);
    }
}
