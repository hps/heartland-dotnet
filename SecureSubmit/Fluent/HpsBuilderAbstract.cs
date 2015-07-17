using System;
using System.Collections.Generic;
using System.Reflection;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Fluent {
    public abstract class HpsBuilderAbstract<TSoapGatewayService, TExecutionResult> {
        private List<HpsBuilderValidation> validations;
        protected bool executed;
        protected TSoapGatewayService service;

        public HpsBuilderAbstract(TSoapGatewayService service) {
            this.validations = new List<HpsBuilderValidation>();

            this.service = service;
            this.SetupValidations();
        }

        public virtual TExecutionResult Execute() {
            this.Validate();
            this.executed = true;

            return default(TExecutionResult);
        }

        private void Validate() {
            foreach (var validation in this.validations) {
                if (!validation.Callback()) {
                    Console.WriteLine(validation.ExceptionMessage);
                    throw new HpsArgumentException(validation.ExceptionMessage);
                }
            }
        }

        internal HpsBuilderAbstract<TSoapGatewayService, TExecutionResult> AddValidation(HpsBuilderValidation validation) {
            validations.Add(validation);
            return this;
        }

        internal HpsBuilderAbstract<TSoapGatewayService, TExecutionResult> AddValidation(Func<bool> callback, String message) {
            var validation = new HpsBuilderValidation {
                Callback = callback,
                ExceptionMessage = message
            };
            return AddValidation(validation);
        }

        protected virtual void SetupValidations() { }
    }
}
