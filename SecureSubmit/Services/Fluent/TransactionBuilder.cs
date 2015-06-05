using System;
using System.Collections.Generic;

namespace SecureSubmit.Services.Fluent
{
    public abstract class TransactionBuilder<TAction, TExecutionResult>
    {
        protected readonly List<Action<TAction>> BuilderActions = new List<Action<TAction>>();
        public abstract TExecutionResult Execute();
    }
}
