using HL7_MAX;
using System;

namespace HL7_FM_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("HL7 EHR FM & MAX Diff CLI");
                Console.WriteLine("Usage: dotnet HL7_FM.ConsoleApp.dll <command> <args>");
                // TODO: enumerate commands and args
            }
            else
            {
                switch (args[0])
                {
                    case "diff":
                        new MAXDiff().diff(args[1], args[2]);
                        break;
                    case "diff-zib":
                        new MAXDiff().diff_DCM_NLCM(args[1], args[2]);
                        break;
                    case "diff-fm":
                        new MAXDiff().diff_FM(args[1], args[2]);
                        break;
                    case "validate":
                        new R2Validator().validate(args[1]);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Choose from diff, diff-zib, diff-fm, validate, compile");
                        break;
                }
            }
        }
    }
}
