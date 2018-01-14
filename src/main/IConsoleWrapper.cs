using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common
{
    public interface IConsoleWrapper
    {
        void Write(string value);

        void WriteLine();

        void WriteLine(string value);

        void Clear();

        ConsoleKeyInfo ReadKey(bool intercept);

        void Beep();
    }
}
