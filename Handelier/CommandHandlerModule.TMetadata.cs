using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public abstract class CommandHandlerModule<TMetadata> : IEnumerable<CommandHandler<TMetadata>>
    {
        private readonly List<CommandHandler<TMetadata>> _handlers;
        
        protected CommandHandlerModule()
        {
            _handlers = new List<CommandHandler<TMetadata>>();
        }

        protected ICommandHandlerBuilder<TCommand, TMetadata> For<TCommand>()
        {
            return new CommandHandlerBuilder<TCommand, TMetadata>(handler => 
            {
                _handlers.Add(new CommandHandler<TMetadata>(typeof(TCommand), (command, metadata, token) => handler((TCommand)command, metadata, token)));
            });
        }

        protected void Handle<TCommand>(Func<TCommand, TMetadata, CancellationToken, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new CommandHandler<TMetadata>(typeof(TCommand), (command, metadata, token) => handler((TCommand)command, metadata, token)));
        }

        public CommandHandler<TMetadata>[] Handlers => _handlers.ToArray();

        public CommandHandlerEnumerator<TMetadata> GetEnumerator()
        {
            return new CommandHandlerEnumerator<TMetadata>(Handlers);
        }

        IEnumerator<CommandHandler<TMetadata>> IEnumerable<CommandHandler<TMetadata>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
