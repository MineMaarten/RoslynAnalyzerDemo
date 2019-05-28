using System;
using System.Linq;
using System.Collections.Generic;

namespace FixingProject
{
    public class Foo<Bar> where Bar : class
    {
        public Bar CurrentBar { get; }

        public Foo(Bar bar) 
            => CurrentBar = bar ?? throw new ArgumentNullException(nameof(bar));

        private class SomeNestedClass
        {
            public int Bar { get; set; }
        }
    }
}
