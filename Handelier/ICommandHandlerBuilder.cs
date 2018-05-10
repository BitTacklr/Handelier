using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public interface ICommandHandlerBuilder<TCommand>
    {
        ICommandHandlerBuilder<TCommand> Pipe(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe);
        ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe);
        void Handle(Func<TCommand, CancellationToken, Task> handler);
    }
}
