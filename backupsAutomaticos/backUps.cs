using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Configuration;


namespace backupsAutomaticos
{
    public class backUps
    {
        #region Constantes
        private static string _rutaDeBackUps = ConfigurationManager.AppSettings["rutaDeBackUps"];
        private static string _ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];  // nombre de la plicacion que se creo en https://console.developers.google.com/apis/credentials (cuenta para entrar: eldonbackups@gmail.com /pass: Luxsys2010)
        private static string _FolderId = ConfigurationManager.AppSettings["FolderId"]; // codigo de direccion url a la carpeta donde se subira         
        private static string _contentType = "aplication/zip"; //extension del archivo
        private static string _pathCredencial = ConfigurationManager.AppSettings["pathCredencial"]; // ruta donde esta el archivo client_secret.json y el archivo token Google.Apis.Auth.OAuth2.Responses.TokenResponse-User
        private static string _permitirSubidaAlDrive = ConfigurationManager.AppSettings["permitirSubidaAlDrive"];
        #endregion

        public void creoBackUpsEnDisco(ConexionDB con)
        {
            List<string> nombreBase = new List<string>();
            var section = (BaseConfigSection)ConfigurationManager.GetSection("bases");
            if (section != null)
            {
                foreach (baseElement item in section.BaseItems)
                {
                    nombreBase.Add(item.nombreBase);
                }
            }
            string fecha = DateTime.Today.ToString("yyyyMMdd").Trim() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            try
            {
                List<string> nombreDelZip = new List<string>();
                foreach (var nombre in nombreBase)
                {
                    string nombreArchivo = nombre + "_" + fecha;
                    nombreDelZip.Add(nombreArchivo);
                    string ruta = _rutaDeBackUps + nombreArchivo + @"\";
                    Directory.CreateDirectory(ruta);
                    string querybk = @"BACKUP DATABASE " + nombre + @" TO DISK = '" + ruta + nombreArchivo + @".bak'";
                    SqlCommand command = new SqlCommand(querybk, con.Conectarbd);
                    command.ExecuteNonQuery();
                    comprimirEnZip(_rutaDeBackUps, nombreArchivo, nombreArchivo + ".zip");
                    eliminoDirectorio(nombreArchivo);
                    Console.WriteLine(nombreArchivo + " creado en disco..");
                }
                Console.WriteLine("\nlos bks en disco se encuntran en " + _rutaDeBackUps);
                Console.WriteLine("\n**********************************************************\n");
                googleDrive(nombreDelZip, _permitirSubidaAlDrive);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error al realizar backups: " + ex.Message);
            }
        }
        public static void comprimirEnZip(string ruta, string nombreCarpeta, string destinoZip)
        {
            try
            {
                string rutaInico = ruta + nombreCarpeta;
                string rutaFinal = ruta + destinoZip;
                ZipFile.CreateFromDirectory(rutaInico, rutaFinal);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error al comprimir: " + ex.Message);
            }
        }
        /// <summary>
        /// Elimino archivo de extension bak dejando solo el zip
        /// </summary>
        /// <param name="nombreArchivo"></param>
        public void eliminoDirectorio(string nombreArchivo)
        {
            string rutaeliminacion = _rutaDeBackUps + nombreArchivo;
            Directory.Delete(rutaeliminacion, true);
        }
        public void googleDrive(List<string> nombreDelZip, string valido)
        {
            if (bool.Parse(valido) == true)
            {
                Console.WriteLine(" Iniciando subida de archivos al google drive");
                foreach (var item in nombreDelZip)
                {
                    Console.WriteLine(item + " subiendo al drive..");
                    SubidaAlDrive(item + ".zip", _rutaDeBackUps + item + ".zip");
                }
            }
        }
        public void SubidaAlDrive(string nombreZip, string rutaDestino)
        {
            try
            {
                GoogleService.GoogleDriver.UploadFile(_pathCredencial, _ApplicationName, nombreZip, rutaDestino, _contentType, _FolderId);
                Console.WriteLine("subio exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error al Subir driver: " + ex.Message);
            }
        }
        public void eliminoBksVencidos(int diasAVencer)
        {
            bool vencido = false;
            try
            {
                DirectoryInfo info = new DirectoryInfo(_rutaDeBackUps);
                foreach (FileInfo item in info.GetFiles("*.zip"))
                {
                    DateTime fecha = DateTime.Now;
                    DateTime Fecha_Archivo = item.LastWriteTime;
                    var dias = (fecha - Fecha_Archivo).TotalDays;
                    if (dias > diasAVencer)
                    {
                        vencido = true;
                        File.Delete(_rutaDeBackUps + item);
                    }
                }
                if (vencido == true)
                {
                    Console.WriteLine("Se eliminaron bks mayores a " + diasAVencer + " dias");
                }
                else
                {
                    Console.WriteLine("No se encontraron backups mayores a " + diasAVencer + " dias");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error al eliminar bks antiguos: " + ex.Message);
            }
        }
    }
}
