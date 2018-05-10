using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    internal class ResultCommandHandlerBuilder<TCommand, TMetadata, TResult> : IResultCommandHandlerBuilder<TCommand, TMetadata, TResult>
    {
        private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;

        public ResultCommandHandlerBuilder(Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build)
        {
            _build = build;
        }

        public IResultCommandHandlerBuilder<TCommand, TMetadata, TResult> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new BeforeReturn(_build, next => pipe(next));
        }

        public IResultCommandHandlerBuilder<TNext, TMetadata, TResult> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new TransformedBeforeReturn<TNext>(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TNext, TMetadata> TransformReturn<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new TransformedAtReturn<TNext>(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TCommand, TMetadata> Return(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new AtReturn(_build, next => pipe(next));
        }

        public void Handle(Func<TCommand, TMetadata, CancellationToken, Task<TResult>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _build(handler);
        }

        private class BeforeReturn : IResultCommandHandlerBuilder<TCommand, TMetadata, TResult>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public BeforeReturn(Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public IResultCommandHandlerBuilder<TCommand, TMetadata, TResult> Pipe(Func<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new BeforeReturn(_build, next => _pipeline(pipe(next)));
            }

            public IResultCommandHandlerBuilder<TNext, TMetadata, TResult> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> TransformReturn<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TCommand, TMetadata> Return(Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new AtReturn(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCommand, TMetadata, CancellationToken, Task<TResult>> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class TransformedBeforeReturn<TCurrent> : IResultCommandHandlerBuilder<TCurrent, TMetadata, TResult>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedBeforeReturn(Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, Func<Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public IResultCommandHandlerBuilder<TCurrent, TMetadata, TResult> Pipe(Func<Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>, Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }


            public IResultCommandHandlerBuilder<TNext, TMetadata, TResult> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task<TResult>>, Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> TransformReturn<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TCurrent, TMetadata> Return(Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCurrent, TMetadata, CancellationToken, Task<TResult>> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class AtReturn : ICommandHandlerBuilder<TCommand, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public AtReturn(Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
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

                return new AfterReturn(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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

        private class TransformedAtReturn<TCurrent> : ICommandHandlerBuilder<TCurrent, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedAtReturn(Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
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

                return new TransformedAfterReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build,  next => _pipeline(pipe(next)));
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

        private class AfterReturn : ICommandHandlerBuilder<TCommand, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public AfterReturn(
                Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, 
                Func<Func<TCommand, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
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

                return new AfterReturn(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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

        private class TransformedAfterReturn<TCurrent> : ICommandHandlerBuilder<TCurrent, TMetadata>
        {
            private readonly Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedAfterReturn(
                Action<Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> build, 
                Func<Func<TCurrent, TMetadata, CancellationToken, Task>, Func<TCommand, TMetadata, CancellationToken, Task<TResult>>> pipeline)
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

                return new TransformedAfterReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext, TMetadata> Transform<TNext>(Func<Func<TNext, TMetadata, CancellationToken, Task>, Func<TCurrent, TMetadata, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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
