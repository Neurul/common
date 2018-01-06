using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.neurul.Common.Test
{
    public abstract class TestContext<T>
    {
        protected T sut;

        public TestContext()
        {
            this.Given();
            if (this.InvokeWhenOnConstruct)
                this.When();
        }

        protected virtual void Given() { }

        protected virtual void When() { }

        protected virtual bool InvokeWhenOnConstruct => true;
    }
}
