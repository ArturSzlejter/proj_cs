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
    internal class Zgony
    {
   
        public static void use_LINQ_with_CSV_zgony( string inFileName, string outFileName )
        {        
            Console.WriteLine(File.Exists(inFileName)
                    ? $"File exists: {inFileName}"
                    : $"File does not exist: {inFileName}");
            
            // Create the IEnumerable data source  
            // 1. Data source.
            string[] ds_raw_lines = System.IO.File.ReadAllLines(inFileName);  

            Console.WriteLine("lines {0}", ds_raw_lines.Length );

            // remove "
            for(int j = 0; j < ds_raw_lines.Length; j++ ) {

                if( (j < 2) || (j == ds_raw_lines.Length - 1) ) {
                    Console.WriteLine("columns {0} {1}", j, ds_raw_lines[j].Split(';').Length );
                }

                // A verbatim string, embedded " must be escaped as ""
                ds_raw_lines[j] = ds_raw_lines[j].Replace( @"""", "" ); 

            }

    /*  SQL query for ADO

      strSQL = "Select zgony.[producent]," & _
            "   Sum( zgony.[liczba_zaraportowanych_zgonow] ) As ile, " & _
            "   Min( zgony.[wiek] ) as min_wiek," & _
            "   Avg( zgony.[wiek] ) as avg_wiek," & _
            "   Max( zgony.[wiek] ) as max_wiek," & _
            "   Min( zgony.[kat_wiek] ) as min_kat_wiek," & _
            "   Max( zgony.[kat_wiek] ) as max_kat_wiek," & _
            " Count( zgony.[liczba_zaraportowanych_zgonow] ) As cnt, " & _
            "   Min( zgony.[data_rap_zgonu] ) as min_data," & _
            "   Max( zgony.[data_rap_zgonu] ) as max_data " & _
             "  From [zgony_po_szczep$] zgony " & _
          " Group By zgony.[producent] " & _
             " Order By 2 Desc, 3 Desc"
    */
    
            // Create the query.
            // HEADER:
            //  0                   1           2           3   4       5           6                   7              8          9                         10  
            // "data_rap_zgonu";"teryt_woj";"teryt_pow";"plec";"wiek";"kat_wiek";"czy_wspolistniejace";"producent";"dawka_ost";"obniz_odpornosc";"liczba_zaraportowanych_zgonow"
  
            // IEnumerable<<string>,<string>,<string>,<string>,...>            
            // 2. Query creation.            
            // data source with picked columns
            var ds_picked_cols =                     // it is an IEnumerable<...>
                  from line in ds_raw_lines.Skip(1)  // skip header
                  let x = line.Split(';')
                  select (                           // AnonymousType
                       new { data_rap_zgonu = x[0], 
                             wiek           = x[4],
                             kat_wiek       = x[5],
                             producent      = x[7],
                             liczba_zgonow  = x[10],
                             data           = Convert.ToDateTime(x[0])
                            } 
                        );

            // grouped data source
            var ds_grouped = (
                  from rec in ds_picked_cols
                 group rec by rec.producent 
                  into newGroup
            // orderby newGroup.Key
                select new {                // AnonymousType
                    producent = newGroup.Key,                    
                    ile      = newGroup.Sum(     x => Tools.SumInt( x.liczba_zgonow )),
                    min_wiek = newGroup.Min(     x => Tools.SumDecimal( x.wiek )),
                    avg_wiek = newGroup.Average( x => Tools.SumDecimal( x.wiek )),
                    max_wiek = newGroup.Max(     x => Tools.SumDecimal( x.wiek )),
                    min_kat_wiek = newGroup.Min( x => x.kat_wiek ),
                    max_kat_wiek = newGroup.Max( x => x.kat_wiek ),
                    cnt      = newGroup.Count(),
                    min_data = newGroup.Min(     x => x.data_rap_zgonu ),
                    max_data = newGroup.Max(     x => x.data_rap_zgonu ),
                    min_dt   = newGroup.Min(     x => x.data ),
                    max_dt   = newGroup.Max(     x => x.data )
                }
            ).OrderByDescending(obj => obj.ile);

            // compose header
            string head = @" 
                    producent       ,
                    ile             ,
                    min_wiek        ,
                    avg_wiek        ,
                    max_wiek        ,
                    min_kat_wiek    ,
                    max_kat_wiek    ,    
                    cnt             ,    
                    min_data        ,
                    max_data        ,
                    min_dt          ,
                    max_dt          ".Replace(System.Environment.NewLine, "");
                    
            head = head.Replace(" ", "");
            Console.WriteLine( head );
                    
            File.WriteAllText( outFileName, head + System.Environment.NewLine ); // , Encoding.UTF8

            // 3. Query execution.
            int i = 0;
            foreach (var n in ds_grouped)
            {
                string  s = string.Format( "{0}, {1}, {2}, {3:f2}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                    n.producent       ,
                    n.ile             ,
                    n.min_wiek        ,
                    n.avg_wiek        ,
                    n.max_wiek        ,
                    n.min_kat_wiek    ,
                    n.max_kat_wiek    ,    
                    n.cnt             ,    
                    n.min_data        ,
                    n.max_data        ,
                    n.min_dt          ,
                    n.max_dt               
                ) + System.Environment.NewLine;

                // show some top lines in the console
                if (i++ < 10) {     
                    Console.Write("{0} {1}", i, s );
                }
             
                File.AppendAllText( outFileName, s);   // , Encoding.UTF8

            }

            Console.WriteLine( $"output written to: {outFileName}" );

        }


    }
}

