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

using Loxodon.Log;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Loxodon.Framework.ILScript {
    [Serializable]
    public abstract class ObservableObjectBase : INotifyPropertyChanged {
        private static readonly ILog log = LogManager.GetLogger(typeof(ObservableObjectBase));

        private readonly object _lock = new object();
        private PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged {
            add { lock (_lock) { this.propertyChanged += value; } }
            remove { lock (_lock) { this.propertyChanged -= value; } }
        }


        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void RaisePropertyChanged(string propertyName = null) {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs">Property changed event.</param>
        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs) {
            try {
                if (propertyChanged != null)
                    propertyChanged(this, eventArgs);
            }
            catch (Exception e) {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", eventArgs.PropertyName, e);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void RaisePropertyChanged(params PropertyChangedEventArgs[] eventArgs) {
            foreach (var args in eventArgs) {
                try {
                    if (propertyChanged != null)
                        propertyChanged(this, args);
                }
                catch (Exception e) {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", args.PropertyName, e);
                }
            }
        }

        /// <summary>
        ///  Set the specified propertyName, field, newValue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null) {
            if (object.Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///  Set the specified propertyName, field, newValue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, PropertyChangedEventArgs eventArgs) {
            if (object.Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(eventArgs);
            return true;
        }
    }
}
