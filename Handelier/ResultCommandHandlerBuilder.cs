using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    internal class ResultCommandHandlerBuilder<TCommand, TResult> : IResultCommandHandlerBuilder<TCommand, TResult>
    {
        private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;

        public ResultCommandHandlerBuilder(Action<Func<TCommand, CancellationToken, Task<TResult>>> build)
        {
            _build = build;
        }

        public IResultCommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new BeforeReturn(_build, next => pipe(next));
        }

        public IResultCommandHandlerBuilder<TNext, TResult> Transform<TNext>(Func<Func<TNext, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new TransformedBeforeReturn<TNext>(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TNext> TransformReturn<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new TransformedAtReturn<TNext>(_build, next => pipe(next));
        }

        public ICommandHandlerBuilder<TCommand> Return(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new AtReturn(_build, next => pipe(next));
        }

        public void Handle(Func<TCommand, CancellationToken, Task<TResult>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _build(handler);
        }

        private class BeforeReturn : IResultCommandHandlerBuilder<TCommand, TResult>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public BeforeReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public IResultCommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new BeforeReturn(_build, next => _pipeline(pipe(next)));
            }

            public IResultCommandHandlerBuilder<TNext, TResult> Transform<TNext>(Func<Func<TNext, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> TransformReturn<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TCommand> Return(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new AtReturn(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCommand, CancellationToken, Task<TResult>> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class TransformedBeforeReturn<TCurrent> : IResultCommandHandlerBuilder<TCurrent, TResult>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedBeforeReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCurrent, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
            {
                _build = build;
                _pipeline = pipeline;
            }

            public IResultCommandHandlerBuilder<TCurrent, TResult> Pipe(Func<Func<TCurrent, CancellationToken, Task<TResult>>, Func<TCurrent, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }


            public IResultCommandHandlerBuilder<TNext, TResult> Transform<TNext>(Func<Func<TNext, CancellationToken, Task<TResult>>, Func<TCurrent, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedBeforeReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> TransformReturn<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TNext>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TCurrent> Return(Func<Func<TCurrent, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAtReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public void Handle(Func<TCurrent, CancellationToken, Task<TResult>> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_pipeline(handler));
            }
        }

        private class AtReturn : ICommandHandlerBuilder<TCommand>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public AtReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
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

                return new AfterReturn(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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

        private class TransformedAtReturn<TCurrent> : ICommandHandlerBuilder<TCurrent>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedAtReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
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

                return new TransformedAfterReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build,  next => _pipeline(pipe(next)));
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

        private class AfterReturn : ICommandHandlerBuilder<TCommand>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public AfterReturn(
                Action<Func<TCommand, CancellationToken, Task<TResult>>> build, 
                Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
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

                return new AfterReturn(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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

        private class TransformedAfterReturn<TCurrent> : ICommandHandlerBuilder<TCurrent>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _pipeline;

            public TransformedAfterReturn(
                Action<Func<TCommand, CancellationToken, Task<TResult>>> build, 
                Func<Func<TCurrent, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipeline)
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

                return new TransformedAfterReturn<TCurrent>(_build, next => _pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TNext> Transform<TNext>(Func<Func<TNext, CancellationToken, Task>, Func<TCurrent, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new TransformedAfterReturn<TNext>(_build, next => _pipeline(pipe(next)));
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
