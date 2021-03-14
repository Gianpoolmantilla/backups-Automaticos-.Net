using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace backupsAutomaticos
{
    public class Program
    {
        private static string numeroDeDias = ConfigurationManager.AppSettings["vencimientoDeBks"];
        static void Main(string[] args)
        {           
            Console.WriteLine("Porfavor espere no cierre el proceso, se esta ejecutando los backups automaticos..");            
            ConexionDB con = new ConexionDB();
            con.abrir();
            Console.WriteLine("Conexion exitosa! ==>> ok\nPorfavor espere..");
            backUps bk = new backUps();
            Console.WriteLine("Buscando backups antiguos.. " );
            bk.eliminoBksVencidos(int.Parse(numeroDeDias));
            Console.WriteLine("\nIniciando creacion de backUps en disco.. ");
            bk.creoBackUpsEnDisco(con);              
            con.cerrar();
            Console.WriteLine("Proceso terminado, puede cerrar la consola\nfin");
            Console.ReadLine();
        }
      
    }
}
