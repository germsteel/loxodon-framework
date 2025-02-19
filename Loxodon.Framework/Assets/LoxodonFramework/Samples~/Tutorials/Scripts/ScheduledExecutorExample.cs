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

using UnityEngine;
using System.Collections;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Tutorials {
#pragma warning disable
    public class ScheduledExecutorExample : MonoBehaviour {
        private IScheduledExecutor scheduled;

        void Start() {
            //		this.scheduled = new CoroutineScheduledExecutor (); /* run on the main thread. */
            this.scheduled = new ThreadScheduledExecutor(); /* run on the background thread. */
            this.scheduled.Start();


            IAsyncResult result = this.scheduled.ScheduleAtFixedRate(() => {
                Debug.Log("This is a test.");
            }, 1000, 2000);
        }

        void OnDestroy() {
            if (this.scheduled != null) {
                this.scheduled.Stop();
                this.scheduled = null;
            }
        }

    }
}