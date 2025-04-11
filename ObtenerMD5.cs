using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//bibliotecas para obtener MD5
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Lector_de_Logs
{
    internal class ObtenerMD5
    {
        public static bool ObtenerMD5Exe(string rutaDestino)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;   //obtiene la ruta completa del .exe que se esta ejecutando
            string currentMD5 = GetMD5HashFromFile(exePath);

            if (!File.Exists(rutaDestino))  // archivo que contiene el MD5 original
            {
                Console.WriteLine("El archivo no existe.");
                return false;
            }

            string expectedMD5 = File.ReadAllText(rutaDestino).Trim().ToLower();

            if (currentMD5 == expectedMD5)
            {
                Console.WriteLine("✔ El MD5 coincide. El ejecutable es válido.");
                return false;
            }
            else
            {
                Console.WriteLine("✖ El MD5 no coincide. El ejecutable puede haber sido modificado.");
                return true;
            }
        }

        public static string GetMD5HashFromFile(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2"));

                string hash = sb.ToString();
                /*
                // Guardar el hash en un archivo .txt junto al .exe
                string exeDirectory = Path.GetDirectoryName(filePath);
                string exeNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string outputTxtPath = Path.Combine(exeDirectory, exeNameWithoutExtension + ".txt");

                File.WriteAllText(outputTxtPath, hash);
                */
                return hash;
            }
        }
    }
}
