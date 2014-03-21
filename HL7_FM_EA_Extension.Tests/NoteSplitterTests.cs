using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class NoteSplitterTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            string[] notes = {
                                 "$ST$",
                                 "$DE$",
                                 "$EX$",
                                 "$ST$$EX$",
                                 "$DE$$EX$",
                                 "$ST$$DE$$EX$",
                                 "xyz",
                                 "$ST$xyz",
                                 "$DE$abc",
                                 "$EX$klm",
                                 "$ST$abc$EX$klm",
                                 "$DE$abc$EX$klm",
                                 "$ST$abc$DE$klm$EX$xyz"
                             };

            Regex regex2 = new Regex(@"\$([A-Z]{2})\$");
            foreach (string note in notes)
            {
                Console.WriteLine("note: {0}", note);

                string[] parts = regex2.Split(note, 10);
                Dictionary<string,string> dict = new Dictionary<string, string>();
                for (int i = 1; i < parts.Length; i+=2)
                {
                    dict[parts[i]] = (parts.Length > i+1)?parts[i + 1]:"";
                }

                Console.WriteLine();
            }
        }
    }
}
