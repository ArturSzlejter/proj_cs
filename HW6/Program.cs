//
// https://basiw.mz.gov.pl/index.html#/visualization?id=3761
// https://dane.gov.pl/pl/dataset/2582,statystyki-zakazen-i-zgonow-z-powodu-covid-19-z-uw
//


using System;
using System.Collections.Generic;
using System.Data;
// using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text; 
// DateTime.Now;
using System.Collections;

// See https://aka.ms/new-console-template for more information
namespace   MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0} Start", DateTime.Now);

            Zgony.use_LINQ_with_CSV_zgony(
                 @"d:\work\2022\proj_cs\HW6\dat\src\ewp_dsh_zgony_po_szczep_202202010941.csv",
                 @"d:\work\2022\proj_cs\HW6\dat\grouped_by_producent_zgony_po_szczep_202202010941.csv"
            );

            Zakazenia.use_LINQ_with_CSV_zakazenia(
                @"d:\work\2022\proj_cs\HW6\dat\src\ewp_dsh_zakazenia_po_szczepieniu_202202010940.csv",
                @"d:\work\2022\proj_cs\HW6\dat\grouped_by_producent_zakazenia_po_szczep_202202010940.csv"
            );

            Console.WriteLine("{0}   End", DateTime.Now);
        }


    }
}

