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
using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object {
    [Serializable]
    public class ObjectSourceDescription : SourceDescription {
        private Path path;

        public ObjectSourceDescription() {
            this.IsStatic = false;
        }

        public ObjectSourceDescription(Path path) {
            this.Path = path;
        }

        public virtual Path Path {
            get { return this.path; }
            set {
                this.path = value;
                if (this.path != null)
                    this.IsStatic = this.path.IsStatic;
            }
        }

        public override string ToString() {
            return this.path == null ? "Path:null" : "Path:" + this.path.ToString();
        }
    }
}
