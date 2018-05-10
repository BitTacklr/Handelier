using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    public abstract class ResultCommandHandlerModule<TResult> : IEnumerable<ResultCommandHandler<TResult>>
    {
        private readonly List<ResultCommandHandler<TResult>> _handlers;
        
        protected ResultCommandHandlerModule()
        {
            _handlers = new List<ResultCommandHandler<TResult>>();
        }

        protected IResultCommandHandlerBuilder<TCommand, TResult> For<TCommand>()
        {
            return new ResultCommandHandlerBuilder<TCommand, TResult>(handler => 
            {
                _handlers.Add(new ResultCommandHandler<TResult>(typeof(TCommand), (command, token) => handler((TCommand)command, token)));
            });
        }

        protected void Handle<TCommand>(Func<TCommand, CancellationToken, Task<TResult>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new ResultCommandHandler<TResult>(typeof(TCommand), (command, token) => handler((TCommand)command, token)));
        }

        public ResultCommandHandler<TResult>[] Handlers => _handlers.ToArray();

        public ResultCommandHandlerEnumerator<TResult> GetEnumerator()
        {
            return new ResultCommandHandlerEnumerator<TResult>(Handlers);
        }

        IEnumerator<ResultCommandHandler<TResult>> IEnumerable<ResultCommandHandler<TResult>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
