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
namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class NewBaseType
    {
        public static bool Jabbed(string producent, string dawka_ost)
        {
            // string alias System.String  
            // int    alias System.Int32 

            producent = producent.Trim();
            dawka_ost = dawka_ost.Trim();

            if (producent.ToLower().Contains("brak danych"))
            {

                producent = "";

            }

            if (dawka_ost.ToLower().Contains("brak danych"))
            {

                dawka_ost = "";

            }

            return String.Concat(producent, dawka_ost).Length > 0;
        }
    }

    internal class Tools : NewBaseType
    {
   
    public static int SumInt(string val)
    {
        int number1 = 0;
    
        bool canConvert = Int32.TryParse(val, out number1);

        return number1;
    }

        public static decimal SumDecimal(string val)
    {
        decimal number1 = 0;
    
        bool canConvert = decimal.TryParse(val, out number1);

        // Console.WriteLine( $"{val} {number1}" );

        return number1;
    }


public static void ListArrayListMembers(ArrayList list)
{
    foreach (Object obj in list)
    {
        Type type = obj.GetType();
        Console.WriteLine("{0} -- ", type.Name);
        Console.WriteLine(" Properties: ");
        foreach (var prop in type.GetProperties())  // PropertyInfo
        {
            Console.WriteLine("\t{0} {1} = {2}", prop.PropertyType.Name, 
                prop.Name, prop.GetValue(obj, null));
        }
        Console.WriteLine(" Fields: ");
        foreach (var field in type.GetFields())     // FieldInfo
        {
            Console.WriteLine("\t{0} {1} = {2}", field.FieldType.Name, 
                field.Name, field.GetValue(obj));
        }
    }
}


    }
}

