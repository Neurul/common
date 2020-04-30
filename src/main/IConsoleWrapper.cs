using System;
using System.Collections.Generic;
using System.Text;

namespace neurUL.Common
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
