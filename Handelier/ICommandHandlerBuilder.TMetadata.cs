using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public interface ICommandHandlerBuilder<TCommand, TMetadata>
    {
        ICommandHandlerBuilder<TCommand, TMetadata> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe);
        ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe);
        void Handle(Func<TCommand, TMetadata, CancellationToken, Task> handler);
    }
}
