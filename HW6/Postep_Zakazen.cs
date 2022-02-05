//
// https://basiw.mz.gov.pl/index.html#/visualization?id=3761
// https://dane.gov.pl/pl/dataset/2582,statystyki-zakazen-i-zgonow-z-powodu-covid-19-z-uw?page=1&per_page=50&q=&sort=-data_date


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
    internal class Postep_Zakazen
    {
  

        public static void use_LINQ_with_CSV( string inFileName, string outFileName )
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

                // test number of columns
                if( (j < 2) || 
                    (j == ds_raw_lines.Length - 1) ||
                    (j == ds_raw_lines.Length / 2)
                ) {
                    Console.WriteLine("columns {0} {1}", j, ds_raw_lines[j].Split(';').Length );
                }

                // A verbatim string, embedded " must be escaped as ""
                ds_raw_lines[j] = ds_raw_lines[j].Replace( @"""", "" ); 

            }

    
            // Create the query.
            // HEADER:
            //           0                   1           2           3        4       5           6                   7          8          9                         10            
            // zgony "data_rap_zgonu";    "teryt_woj";"teryt_pow";"plec";"wiek";"kat_wiek";"czy_wspolistniejace";"producent";"dawka_ost";"obniz_odpornosc";"liczba_zaraportowanych_zgonow"
            // zakaz "data_rap_zakazenia";"teryt_woj";"teryt_pow";"plec";"wiek";"kat_wiek";                      "producent";"dawka_ost";"obniz_odpornosc";"liczba_zaraportowanych_zakazonych"
            //           0                   1           2           3        4       5                                6         7          8                          9 
  
            // IEnumerable<<string>,<string>,<string>,<string>,...>            
            // 2. Query creation.            
            // data source with picked columns
            var ds_picked_cols =                     // it is an IEnumerable<...>
                  from line in ds_raw_lines.Skip(1) // .Take(111100)   // skip header
                  let x = line.Split(';')
              //   where Convert.ToDateTime(x[0]).Year == 2022
                  select (                           // AnonymousType
                       new { data_rap_zakazenia = x[0], 
                             wiek           = x[4],
                             kat_wiek       = x[5],
                             producent      = x[6],
                             liczba_zakazonych = x[9],
                             data           = Convert.ToDateTime(x[0]),
                             dawka_ost      = x[7],
                             rok            = Convert.ToDateTime(x[0]).Year,
                             szczepiony     = Tools.Jabbed( x[6], x[7] )
                            } 
                        );
            
            var sum2 = ds_picked_cols.Sum(x => Tools.SumInt( x.liczba_zakazonych ));    // decimal (money)
            var sum_jabbed2 = ds_picked_cols.Where(x => x.szczepiony ).Sum(x => Tools.SumInt( x.liczba_zakazonych ));   
            var sum_normal2 = ds_picked_cols.Where(x => !x.szczepiony ).Sum(x => Tools.SumInt( x.liczba_zakazonych ));   

            Console.WriteLine( $"{sum2} = jabbed {sum_jabbed2} + normal {sum_normal2} = {sum_jabbed2 + sum_normal2}" );

            // grouped data source
            var ds_grouped = (                                       //  Since C# 7 you can also use value tuples:   
                  from rec in ds_picked_cols                         //  group x by (x.Column1, x.Column2)   
                 group rec by rec.data  // .GroupBy(x => (x.Column1, x.Column2))
                  into newGroup
            // orderby newGroup.Key
                select new {                // AnonymousType
                    data     = newGroup.Key,
                    ile      = newGroup.Sum(     x => Tools.SumInt( x.liczba_zakazonych )),
                    min_wiek = newGroup.Min(     x => Tools.SumDecimal( x.wiek )),
                    avg_wiek = newGroup.Average( x => Tools.SumDecimal( x.wiek )),
                    max_wiek = newGroup.Max(     x => Tools.SumDecimal( x.wiek )),
                    min_kat_wiek = newGroup.Min( x => x.kat_wiek ),
                    max_kat_wiek = newGroup.Max( x => x.kat_wiek ),
                    cnt      = newGroup.Count(),
                    // min_data = newGroup.Min(     x => x.data_rap_zakazenia ),
                    // max_data = newGroup.Max(     x => x.data_rap_zakazenia ),
                    // min_dt   = newGroup.Min(     x => x.data ),
                    // max_dt   = newGroup.Max(     x => x.data ),
                    dawka_ost = String.Concat( newGroup.Min( x => x.dawka_ost ), "..", newGroup.Max( x => x.dawka_ost )),
                    rok       = String.Concat( newGroup.Min( x => x.rok ),       "..", newGroup.Max( x => x.rok )),
                    jabbed_cnt = newGroup.Sum( x => ( x.szczepiony ? Tools.SumInt( x.liczba_zakazonych) : 0 )),
                    normal_cnt = newGroup.Sum( x => (!x.szczepiony ? Tools.SumInt( x.liczba_zakazonych) : 0 )),
                    producent  = String.Concat( newGroup.Min( x => x.producent ), "..", newGroup.Max( x => x.producent ))
                }
            ).OrderBy( obj => obj.data );

            var sum3 = ds_grouped.Sum(x => x.ile );    // decimal (money)
            var sum_jabbed3 = ds_picked_cols.Where(x => x.szczepiony ).Sum(x => Tools.SumInt( x.liczba_zakazonych ));   
            var sum_normal3 = ds_picked_cols.Where(x => !x.szczepiony ).Sum(x => Tools.SumInt( x.liczba_zakazonych ));   

            Console.WriteLine( $"{sum3} = jabbed {sum_jabbed3} + normal {sum_normal3} = {sum_jabbed2 + sum_normal3}" );

            // compose header
            string head = @" 
                    data_rap        ,
                    ile             ,
                    min_wiek        ,
                    avg_wiek        ,
                    max_wiek        ,
                    min_kat_wiek    ,
                    max_kat_wiek    ,    
                    cnt             ,                          
                    dawka_ost       ,
                    rok             ,
                    jabbed_cnt      ,
                    normal_cnt      ,
                    producent       ,
                    procent_jabbed  ,
                    procent_normal   ,
                    total".Replace(System.Environment.NewLine, "");

                    /*
                    // min_data        ,
                    // max_data        ,
                    // min_dt          ,
                    // max_dt          ,
                    */
                    
            head = head.Replace(" ", "");
            Console.WriteLine( head );
                    
            File.WriteAllText( outFileName, head + System.Environment.NewLine ); // , Encoding.UTF8

            // 3. Query execution.
            int i = 0;
            foreach (var n in ds_grouped)
            {
                string  s = string.Format( "{0}, {1}, {2}, {3:f2}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}",
                    n.data            ,
                    n.ile             ,
                    n.min_wiek        ,
                    n.avg_wiek        ,
                    n.max_wiek        ,
                    n.min_kat_wiek    ,
                    n.max_kat_wiek    ,    
                    n.cnt             ,    
                    // n.min_data        ,
                    // n.max_data        ,
                    // n.min_dt          ,
                    // n.max_dt          ,
                    n.dawka_ost       ,
                    n.rok             ,
                    n.jabbed_cnt      ,
                    n.normal_cnt      ,
                    n.producent       ,
                    n.ile > 0 ? (100.0 * n.jabbed_cnt / n.ile) : null,
                    n.ile > 0 ? (100.0 * n.normal_cnt / n.ile) : null,
                    n.jabbed_cnt + n.normal_cnt,
                    null,null,null,null
                ) + System.Environment.NewLine;

                // show some top lines in the console
                if (i++ < 10) {     
                    Console.Write("{0} {1}", i, s );
                }
             
                File.AppendAllText( outFileName, s);   // , Encoding.UTF8

            }

            Console.WriteLine( $"{sum2} sum test {sum3}" );

            Console.WriteLine( $"output written to: {outFileName}" );

        }


    }
}

