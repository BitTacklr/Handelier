using System;
using System.Collections;
using System.Collections.Generic;

namespace Handelier
{
    public class ResultCommandHandlerEnumerator<TResult> : IEnumerator<ResultCommandHandler<TResult>>
    {
        private readonly ResultCommandHandler<TResult>[] _handlers;
        private int _index;

        public ResultCommandHandlerEnumerator(ResultCommandHandler<TResult>[] handlers)
        {
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
            _index = -1;
        }

        public bool MoveNext()
        {
            return _index < _handlers.Length &&
                   ++_index < _handlers.Length;
        }

        public void Reset()
        {
            _index = -1;
        }

        public ResultCommandHandler<TResult> Current
        {
            get
            {
                if (_index == -1)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                if (_index == _handlers.Length)
                    throw new InvalidOperationException("Enumeration has already ended. Call Reset.");

                return _handlers[_index];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}
