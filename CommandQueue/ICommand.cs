using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandQueue
{
    public interface ICommand
    {
        void Execute();
    }
}
