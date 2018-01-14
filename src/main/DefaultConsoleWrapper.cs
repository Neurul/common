using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common
{
    public class DefaultConsoleWrapper : IConsoleWrapper
    {
        public void Beep()
        {
            Console.Beep();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
