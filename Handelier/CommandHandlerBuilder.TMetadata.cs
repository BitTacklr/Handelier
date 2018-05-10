using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    internal class CommandHandlerBuilder<TCommand, TMetadata> : ICommandHandlerBuilder<TCommand, TMetadata>
    {
        private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task>> _build;

        public CommandHandlerBuilder(Action<Func<TCommand, TMetadata, CancellationToken, Task>> build)
        {
            _build = build;
        }

        public ICommandHandlerBuilder<TCommand, TMetadata> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new WithPipeline(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new WithTransformedPipeline<TNext>(_build, next => pipe(next));
        }

        public void Handle(Func<TCommand, TMetadata, CancellationToken, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _build(handler);
        }

        private class WithPipeline : ICommandHandlerBuilder<TCommand, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task>> _build;
            private readonly Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> _pipeline;

            public WithPipeline(Action<Func<TCommand, TMetadata, CancellationToken, Task>> build, Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public ICommandHandlerBuilder<TCommand, TMetadata> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithPipeline(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCommand, TMetadata, CancellationToken, Task> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class WithTransformedPipeline<TCurrent> : ICommandHandlerBuilder<TCurrent, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task>> _build;
            private readonly Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> _pipeline;

            public WithTransformedPipeline(Action<Func<TCommand, TMetadata, CancellationToken, Task>> build, Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public ICommandHandlerBuilder<TCurrent, TMetadata> Pipe(Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new WithTransformedPipeline<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCurrent, TMetadata, CancellationToken, Task> handler)
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
