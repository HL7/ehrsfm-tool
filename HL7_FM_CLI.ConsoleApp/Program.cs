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
                Console.WriteLine("usage: dotnet HL7_FM.ConsoleApp.dll <command> <args>");
                Console.WriteLine();
                Console.WriteLine("These are the commands:");
                Console.WriteLine();
                Console.WriteLine("EHR-S FM / FP commands");
                Console.WriteLine("    validate    validate a fm/fp/fpdef exported as max file [max file]");
                Console.WriteLine("    compile     compile a Functional Model [base fm file, profile def file, output file]");
                Console.WriteLine("MAX comamnds");
                Console.WriteLine("    diff        a pure max diff");
                Console.WriteLine("    diff-zib    diff zibs exported as max file");
                Console.WriteLine("    diff-fm     diff fm/fp/fpdef exported as max file");
                Console.WriteLine();
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
                    case "compile":
                        new R2ProfileCompiler().Compile(args[1], args[2], args[3]);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Choose from diff, diff-zib, diff-fm, validate, compile");
                        break;
                }
            }
        }
    }
}
