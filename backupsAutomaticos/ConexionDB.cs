using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;

namespace backupsAutomaticos
{
   public class ConexionDB
    {
        //Cadena de Conexion
        string cadena = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public SqlConnection Conectarbd = new SqlConnection();
        //Constructor
        public ConexionDB()
        {
            Conectarbd.ConnectionString = cadena;
        }
        //Metodo para abrir la conexion
        public void abrir()
        {
            try
            {
                Conectarbd.Open();
               // Console.WriteLine("conexion abierta");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error al abrir BD " + ex.Message);
            }
        }
        //Metodo para cerrar la conexion
        public void cerrar()
        {
            Conectarbd.Close();
        }
    }
}
