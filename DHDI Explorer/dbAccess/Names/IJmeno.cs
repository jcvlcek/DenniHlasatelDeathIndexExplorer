using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public interface IJmeno
    {
        String Native { get; }

        string Web { get; }

        string PlainText { get; }

        int Count { get; }

        int Rank { get; }
    }
}
