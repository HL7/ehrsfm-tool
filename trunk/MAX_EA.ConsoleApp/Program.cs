using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAX_EA
{
    class Program
    {
        static void Main(string[] args)
        {
            MAXDiff diff = new MAXDiff();
            diff.diff_DCM_NLCM(args[0], args[1]);
        }
    }
}
