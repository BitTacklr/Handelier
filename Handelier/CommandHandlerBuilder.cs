using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    internal class CommandHandlerBuilder<TCommand> : ICommandHandlerBuilder<TCommand>
    {
        private readonly Action<Func<TCommand, CancellationToken, Task>> _build;

        public CommandHandlerBuilder(Action<Func<TCommand, CancellationToken, Task>> build)
        {
            _build = build;
        }

        public ICommandHandlerBuilder<TCommand> Pipe(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new WithPipeline(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new WithTransformedPipeline<TNext>(_build, next => pipe(next));
        }

        public void Handle(Func<TCommand, CancellationToken, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _build(handler);
        }

        private class WithPipeline : ICommandHandlerBuilder<TCommand>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> _pipeline;

            public WithPipeline(Action<Func<TCommand, CancellationToken, Task>> build, Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public ICommandHandlerBuilder<TCommand> Pipe(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithPipeline(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCommand, CancellationToken, Task> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class WithTransformedPipeline<TCurrent> : ICommandHandlerBuilder<TCurrent>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task>> _build;
            private readonly Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> _pipeline;

            public WithTransformedPipeline(Action<Func<TCommand, CancellationToken, Task>> build, Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public ICommandHandlerBuilder<TCurrent> Pipe(Func<Func<TCurrent, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCurrent, CancellationToken, Task> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }
    }
}
