﻿/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;

namespace Loxodon.Framework.Binding.Parameters {
    public class ParameterWrapInvoker : IInvoker {
        protected readonly IInvoker invoker;
        protected readonly ICommandParameter commandParameter;
        public ParameterWrapInvoker(IInvoker invoker, ICommandParameter commandParameter) {
            if (invoker == null)
                throw new ArgumentNullException("invoker");

            if (commandParameter == null)
                throw new ArgumentNullException("commandParameter");

            this.invoker = invoker;
            this.commandParameter = commandParameter;
        }

        public object Invoke(params object[] args) {
            return this.invoker.Invoke(commandParameter.GetValue());
        }
    }

    public class ParameterWrapInvoker<T> : IInvoker {
        protected readonly IInvoker<T> invoker;
        protected readonly ICommandParameter<T> commandParameter;
        public ParameterWrapInvoker(IInvoker<T> invoker, ICommandParameter<T> commandParameter) {
            if (invoker == null)
                throw new ArgumentNullException("invoker");

            if (commandParameter == null)
                throw new ArgumentNullException("commandParameter");

            this.invoker = invoker;
            this.commandParameter = commandParameter;
        }

        public object Invoke(params object[] args) {
            return this.invoker.Invoke(commandParameter.GetValue());
        }
    }
}
