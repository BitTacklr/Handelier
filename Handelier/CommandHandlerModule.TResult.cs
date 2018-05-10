using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public abstract class CommandHandlerModule<TResult> : IEnumerable<CommandHandler<TResult>>
    {
        private readonly List<CommandHandler<TResult>> _handlers;
        
        protected CommandHandlerModule()
        {
            _handlers = new List<CommandHandler<TResult>>();
        }

        protected ICommandHandlerBuilder<TCommand, TResult> For<TCommand>()
        {
            return new CommandHandlerBuilder<TCommand, TResult>(handler => 
            {
                _handlers.Add(new CommandHandler<TResult>(typeof(TCommand), (command, token) => handler((TCommand)command, token)));
            });
        }

        protected void Handle<TCommand>(Func<TCommand, CancellationToken, Task<TResult>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new CommandHandler<TResult>(typeof(TCommand), (command, token) => handler((TCommand)command, token)));
        }

        public CommandHandler<TResult>[] Handlers => _handlers.ToArray();

        public CommandHandlerEnumerator<TResult> GetEnumerator()
        {
            return new CommandHandlerEnumerator<TResult>(Handlers);
        }

        IEnumerator<CommandHandler<TResult>> IEnumerable<CommandHandler<TResult>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
