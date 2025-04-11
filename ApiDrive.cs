using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//apis de google
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace Lector_de_Logs
{
    class ApiDrive
    {
        public static async Task DriveApi()
        {
            // Ruta al JSON de la cuenta de servicio
            string serviceAccountFile = "service-account.json";

            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                              .CreateScoped(DriveService.ScopeConstants.Drive); // O DriveReadonly
            }

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MiAppDriveBackend",
            });

            // Listar archivos
            /*
            var request = service.Files.List();
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync();

            Console.WriteLine("Archivos visibles:");
            foreach (var file in result.Files)
            {
                Console.WriteLine($"{file.Name} ({file.Id})");
            }
            */
            var rutaBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string exeNameWithoutExtension = Path.GetFileNameWithoutExtension(exePath);
            string archivoEXE = Path.Combine(exeNameWithoutExtension + ".exe");
            string archivoTXT = Path.Combine(exeNameWithoutExtension + ".txt");

            var rutaDestino = Path.Combine(rutaBase, archivoTXT);

            await DescargarArchivoPorNombreAsync(service, archivoTXT, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            //await DescargarArchivoPorExtensionAsync(service, ".txt");  // o ".exe"

            bool existeActualizacion = ObtenerMD5.ObtenerMD5Exe(rutaDestino);

            if (existeActualizacion)
            {
                DialogResult actualizar = MessageBox.Show("Existe una actualización nueva! ¿Actualizar ahora?", "Actualización", buttons: MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (actualizar == DialogResult.Yes)
                {
                    await DescargarArchivoPorNombreAsync(service, archivoEXE, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\"));
                    MessageBox.Show("Exe descargardo en ruta Descargas!", "Descargado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }

        }


        public static async Task DescargarArchivoPorNombreAsync(DriveService service, string nombreArchivo, string rutaBase)
        {
            // Paso 1: Buscar el archivo por su nombre (primera coincidencia exacta)
            var listRequest = service.Files.List();
            listRequest.Q = string.Format("name = '{0}' and trashed = false", nombreArchivo);
            listRequest.Fields = "files(id, name)";
            var fileList = await listRequest.ExecuteAsync();

            var archivo = fileList.Files.FirstOrDefault();
            if (archivo == null)
            {
                Console.WriteLine(string.Format("Archivo '{}' no encontrado en Drive.",nombreArchivo));
                return;
            }

            // Paso 2: Obtener la ruta del ejecutable actual
            //var rutaBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rutaDestino = Path.Combine(rutaBase, archivo.Name);

            // Paso 3: Descargar el archivo
            var request = service.Files.Get(archivo.Id);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);

            using (var file = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(file);
            }

            Console.WriteLine(string.Format("Archivo descargado: {0}",rutaDestino));
        }


        public static async Task DescargarArchivoPorExtensionAsync(DriveService service, string extensionDeseada)
        {
            // Asegurarse de que la extensión empiece con punto (".")
            if (!extensionDeseada.StartsWith("."))
                extensionDeseada = "." + extensionDeseada;

            // Paso 1: Buscar archivos que terminen en esa extensión
            var listRequest = service.Files.List();
            //listRequest.Q = $"name contains '{extensionDeseada}' and trashed = false";
            listRequest.Fields = "files(id, name)";
            var fileList = await listRequest.ExecuteAsync();

            // Buscar el primero que realmente tenga esa extensión
            var archivo = fileList.Files.FirstOrDefault(f => Path.GetExtension(f.Name).Equals(extensionDeseada, StringComparison.OrdinalIgnoreCase));

            if (archivo == null)
            {
                //Console.WriteLine($"No se encontró ningún archivo con extensión '{extensionDeseada}' en Drive.");
                return;
            }

            // Paso 2: Ruta donde guardar el archivo
            var rutaBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rutaDestino = Path.Combine(rutaBase, archivo.Name);

            // Paso 3: Descargar el archivo
            var request = service.Files.Get(archivo.Id);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);

            using (var file = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(file);
            }

            //Console.WriteLine($"Archivo '{archivo.Name}' descargado en: {rutaDestino}");

            ObtenerMD5.ObtenerMD5Exe(rutaDestino);
        }
    }
}
