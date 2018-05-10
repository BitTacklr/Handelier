using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handelier
{
    internal class CommandHandlerBuilder<TCommand, TResult> : ICommandHandlerBuilder<TCommand, TResult>
    {
        private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;

        public CommandHandlerBuilder(Action<Func<TCommand, CancellationToken, Task<TResult>>> build)
        {
            _build = build;
        }

        public ICommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

            return new BeforeReturn(_build, next => pipe(next));
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

        private class BeforeReturn : ICommandHandlerBuilder<TCommand, TResult>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> _return_pipeline;

            public BeforeReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> return_pipeline)
            {
                _build = build;
                _return_pipeline = return_pipeline;
            }

            public ICommandHandlerBuilder<TCommand, TResult> Pipe(Func<Func<TCommand, CancellationToken, Task<TResult>>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new BeforeReturn(_build, next => _return_pipeline(pipe(next)));
            }

            public ICommandHandlerBuilder<TCommand> Return(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new AtReturn(_build, next => _return_pipeline(pipe(next)));
            }

            public void Handle(Func<TCommand, CancellationToken, Task<TResult>> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_return_pipeline(handler));
            }
        }

        private class AtReturn : ICommandHandlerBuilder<TCommand>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _return_pipeline;

            public AtReturn(Action<Func<TCommand, CancellationToken, Task<TResult>>> build, Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> return_pipeline)
            {
                _build = build;
                _return_pipeline = return_pipeline;
            }

            public ICommandHandlerBuilder<TCommand> Pipe(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new AfterReturn(_build, _return_pipeline, next => pipe(next));
            }

            public void Handle(Func<TCommand, CancellationToken, Task> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_return_pipeline(handler));
            }
        }

        private class AfterReturn : ICommandHandlerBuilder<TCommand>
        {
            private readonly Action<Func<TCommand, CancellationToken, Task<TResult>>> _build;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> _return_pipeline;
            private readonly Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> _pipeline;

            public AfterReturn(
                Action<Func<TCommand, CancellationToken, Task<TResult>>> build, 
                Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task<TResult>>> return_pipeline,
                Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipeline)
            {
                _build = build;
                _return_pipeline = return_pipeline;
                _pipeline = pipeline;
            }

            public ICommandHandlerBuilder<TCommand> Pipe(Func<Func<TCommand, CancellationToken, Task>, Func<TCommand, CancellationToken, Task>> pipe)
            {
                if (pipe == null)
                {
                    throw new ArgumentNullException(nameof(pipe));
                }

                return new AfterReturn(_build, _return_pipeline, next => pipe(next));
            }

            public void Handle(Func<TCommand, CancellationToken, Task> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                _build(_return_pipeline(_pipeline(handler)));
            }
        }
    }
}
