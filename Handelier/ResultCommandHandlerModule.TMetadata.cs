using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public abstract class ResultCommandHandlerModule<TMetadata, TResult> : IEnumerable<ResultCommandHandler<TMetadata, TResult>>
    {
        private readonly List<ResultCommandHandler<TMetadata, TResult>> _handlers;
        
        protected ResultCommandHandlerModule()
        {
            _handlers = new List<ResultCommandHandler<TMetadata, TResult>>();
        }

        protected IResultCommandHandlerBuilder<TCommand, TMetadata, TResult> Build<TCommand>()
        {
            return new ResultCommandHandlerBuilder<TCommand, TMetadata, TResult>(handler => 
            {
                _handlers.Add(new ResultCommandHandler<TMetadata, TResult>(typeof(TCommand), (command, metadata, token) => handler((TCommand)command, metadata, token)));
            });
        }

        protected void Handle<TCommand>(Func<TCommand, TMetadata, CancellationToken, Task<TResult>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new ResultCommandHandler<TMetadata, TResult>(typeof(TCommand), (command, metadata, token) => handler((TCommand)command, metadata, token)));
        }

        public ResultCommandHandler<TMetadata, TResult>[] Handlers => _handlers.ToArray();

        public ResultCommandHandlerEnumerator<TMetadata, TResult> GetEnumerator()
        {
            return new ResultCommandHandlerEnumerator<TMetadata, TResult>(Handlers);
        }

        IEnumerator<ResultCommandHandler<TMetadata, TResult>> IEnumerable<ResultCommandHandler<TMetadata, TResult>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
