using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Lector_de_Logs
{
    public partial class Form1 : Form
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ruta = (String)textBox1.Text;
            string[] archivos;
            string[] resultado = {};
            string txt;
            int i = 0;
            int x = 0;
            int[] t = new int[50001];
            archivos = ContadorArchivosTXT(ruta);

            if (textBox3.Text == "")
            {
                x = 50000;
                textBox3.Text = "50000";
                Escribir("Añadiendo por defecto 50000 lineas de muestra maximas");
            }
            else
            {
                if (int.TryParse(textBox3.Text, out x))
                {
                    if (x > 50000)
                    {
                        x = 50000;
                        Escribir("Numero de muestra maximo 50000");
                        textBox3.Text = "50000";
                    }
                    Escribir("Muestra a obtener de " + x);
                }else
                {
                    x = 50000;
                    Escribir("Caracteras especiales encontrados");
                    Escribir("Añadiendo por defecto 50000 lineas de muestra maximas");
                    textBox3.Text = "50000";
                }
            }

            if (!Directory.Exists(ruta))
            {

                Escribir("Añadir ruta de la carpeta de logs");
            }
                else
            {
                string nombre = "log_" + (i + 1) + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                try
                {
                    // Verificar si el archivo existe
                    while (File.Exists(nombre))
                    {
                        i++;
                        nombre = "log_" + i + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                    }
                }
                catch (Exception ex)
                {
                    Escribir($"Error: {ex.Message}");
                }

                for (i = 0; i < archivos.Length; i++)
                {

                    if (t[50000] <= x - 1)
                    {
                        Escribir("Abriendo el archivo " + archivos[i]);
                        txt = archivos[i];
                        t = Lector(txt, i, nombre, t, x);
                    }
                    else
                    {
                        Escribir("Limite de " + x + " registros alcanzado");
                        i = archivos.Length;
                    }
                }
                string[] lineas = File.ReadAllLines(nombre);
                i = 1;
                Escribir("Añadiendo total de consultas duplicadas");
                while (i != x+1 && i < lineas.Length)
                {
                    lineas[i] = lineas[i] + "|" + (t[i] + 1).ToString();
                    i++;
                }

                Escribir("Muestra obtenida de " + (lineas.Length - 1).ToString());
                File.WriteAllLines(nombre, lineas);
            }
            Escribir("Finaliza el programa");
            Escribir("");


        }
        public string[] ContadorArchivosTXT(string ruta)
        {
            Escribir( "Buscando archivos en la ruta: " + ruta);
            string[] archivosTXT = {};
            string extension;

            if (comboBox1.Text.Length < 1)
            {
                extension = ".txt";
                comboBox1.Text = extension;
            }
            else
            {
                extension = comboBox1.Text;
            }

            try
            {
                // Validar que la carpeta exista
                if (!Directory.Exists(ruta))
                {
                    Escribir("La carpeta no existe.");
                    return archivosTXT;
                }

                // Obtener la lista de archivos .txt en la carpeta y guardarlos en una matriz

                archivosTXT = Directory.GetFiles(ruta, "*" + extension);

                // Mostrar la cantidad de archivos .txt encontrados
                Escribir($"Se encontraron {archivosTXT.Length} archivos {extension} en la carpeta.");
                
            }
            catch (Exception ex)
            {
                Escribir($"Ocurrió un error: {ex.Message}");
            }

            return archivosTXT;
        }

        public void Escribir (string texto)
        { 
            if (textBox2.Text == "Log")
            {
                textBox2.Text = texto;
                return;
            }

            textBox2.AppendText(Environment.NewLine + texto);
            return;
        }

        public int[] Lector(string txt, int libro, string nombre, int[] t , int x)
        {
            int punto;
            int c=0;
            int k;
            int v;
            int porcentaje = 0;
            int estado;
            int separador;
            int archivo;
            int omitir = 0 ;
            string ip;
            string puerto;
            string BD;
            string usuario;
            TimeSpan hora;

            if (libro == 0)
            {
                archivo = 0;
            }
            else
            {
                archivo = 1;
            }

            try
            {
                // Abre el archivo para lectura
                using (StreamReader lector = new StreamReader(txt))
                {
                    // Lee el archivo línea por línea
                    string linea;
                    while ((linea = lector.ReadLine()) != null)
                    {
                        estado = 0;

                        ip = "";
                        puerto = "";
                        BD = "";
                        usuario = "";

                        string[] palabras = linea.Split(' '); // Divide por espacios

                        if (t[50000] + c < x)
                        {
                            estado = 0;
                        }
                        else
                        {
                            estado = 6;
                            break;
                        }

                        for (int i = 0; i < palabras.Length; i++)
                        {
                            if (estado == 3)
                            {
                                break;
                            }
                            if (estado==5)
                            {
                                estado = omitir;
                                omitir = 0;
                            }
                            if (palabras[i] == "")
                            {
                                omitir = estado;
                                estado = 5;
                            }
                            switch (estado)
                            {
                                case 6:
                                    break;
                                case 0:
                                    {
                                        punto = 0;
                                        separador = 0;
                                        if ((punto = palabras[i].IndexOf('.') ) > 0)
                                        {
                                            if (punto != palabras[i].LastIndexOf('.'))
                                            {
                                                if ((separador = palabras[i].IndexOf('(')) > 0)
                                                {
                                                    for (int j = 0; j< separador; j++)
                                                    {
                                                        ip += palabras[i][j];
                                                    }
                                                    for (int j = separador+1; j < palabras[i].Length-1; j++)
                                                    {
                                                        puerto += palabras[i][j];
                                                    }
                                                    estado = 1;
                                                    break;
                                                }
                                                else
                                                {
                                                    if ((separador = palabras[i].IndexOf(':')) > 0)
                                                    {
                                                        for (int j = 0; j < separador; j++)
                                                        {
                                                            ip += palabras[i][j];
                                                        }
                                                        for (int j = separador+1; j < palabras[i].Length; j++)
                                                        {
                                                            puerto += palabras[i][j];
                                                        }
                                                        estado = 1;
                                                        break;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (palabras[i].IndexOf("[local]") > -1)
                                            {
                                                ip = palabras[i];
                                                estado = 1;
                                            }
                                            break;
                                        }

                                        break;
                                    }
                                case 1:
                                    {
                                        if (palabras[i][0] == 's' && palabras[i].Length > 2)
                                        {
                                            if (palabras[i][1] == 'y')
                                            {
                                                if (palabras[i][2] == 's')
                                                {
                                                    BD = palabras[i - 1];
                                                    usuario = palabras[i];
                                                    estado = 2;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (palabras[i] == "postgres" && palabras[i] != "PostgreSQL" && palabras[i+1].Length > 1)
                                            {
                                                if (palabras[i+1] != "postgres" && palabras[i - 1] != "postgres")
                                                {
                                                    if (palabras[i + 1].Length > 0)
                                                    {
                                                        if (palabras[i + 1][0] == 's' && palabras[i].Length > 2)
                                                        {
                                                            if (palabras[i + 1][1] == 'y')
                                                            {
                                                                if (palabras[i + 1][2] == 's')
                                                                {
                                                                    BD = palabras[i];
                                                                    usuario = palabras[i + 1];
                                                                    estado = 2;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            BD = palabras[i - 1];
                                                            usuario = palabras[i];
                                                            estado = 2;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        
                                        if (palabras[i].IndexOf("[unknown]") > -1)
                                        {
                                            if (palabras[i+1].IndexOf("[unknown]") > -1 )
                                            {
                                                BD = palabras[i];
                                                usuario = palabras[i+1];
                                                estado = 2;
                                            }
                                            if (palabras[i + 1][0] == 's')
                                            {
                                                if (palabras[i + 1][1] == 'y')
                                                {
                                                    if (palabras[i + 1][2] == 's')
                                                    {
                                                        BD = palabras[i];
                                                        usuario = palabras[i + 1];
                                                        estado = 2;
                                                    }
                                                }
                                            }
                                        }

                                        if (palabras[i].IndexOf("postgres") > -1 && i + 1 < palabras.Length)
                                        {
                                            if (palabras[i+1].IndexOf("postgres") > -1)
                                            {
                                                BD = palabras[i];
                                                usuario = palabras[i + 1];
                                                estado = 2;
                                            }

                                        }

                                        if (estado == 2)
                                        {
                                            if (archivo == 0)
                                            {
                                                try
                                                {
                                                    using (StreamWriter escritor = new StreamWriter(nombre, true))
                                                    {

                                                        escritor.WriteLine("IP|PUERTO|BD|USUARIO|USOS");
                                                        escritor.WriteLine(ip + "|" + puerto + "|" + BD + "|" + usuario);
                                                        estado = 3;
                                                    }
                                                    Escribir($"El archivo '{nombre}' se ha creado y escrito exitosamente.");
                                                    hora = (DateTime.Now).TimeOfDay;
                                                    Escribir(hora.ToString());

                                                }
                                                catch (Exception ex)
                                                {
                                                    Escribir($"Ocurrió un error: {ex.Message}");
                                                }
                                                archivo = 1;
                                                c++;
                                                //Escribir("conexiones distintas = " + (t[50000] + c).ToString());
                                            }
                                            else
                                            {
                                                // Abre el archivo para lectura
                                                using (StreamReader lector2 = new StreamReader(nombre))
                                                {
                                                    // Lee el archivo línea por línea
                                                    string comprobar;
                                                    k = 0;
                                                    v = 0;
                                                    while ((comprobar = lector2.ReadLine()) != null && k == 0)
                                                    {
                                                        if (comprobar == ip + "|" + puerto + "|" + BD + "|" + usuario)
                                                        {
                                                            t[v] = t[v] + 1;
                                                            k = 1;
                                                            estado = 3;
                                                        }
                                                        v = v + 1;
                                                    }

                                                }
                                                if (k == 0)
                                                {
                                                    try
                                                    {
                                                        using (StreamWriter escritor = new StreamWriter(nombre, true))
                                                        {

                                                            escritor.WriteLine(ip + "|" + puerto + "|" + BD + "|" + usuario);
                                                            estado = 3;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Escribir($"Ocurrió un error: {ex.Message}");
                                                    }
                                                    c++;
                                                    porcentaje++;
                                                    if (porcentaje == 1000)
                                                    {

                                                        hora = (DateTime.Now).TimeOfDay;
                                                        Escribir(((t[50000] + c) * 100 / x).ToString() + "%" + " cantidad " + (t[50000] + c).ToString() + " " + hora.ToString());
                                                        porcentaje = 0;
                                                    }
                                                }
                                            }
                                        }

                                        break;
                                    }
                            }


                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Escribir($"Error al leer el archivo: {ex.Message}");
            }
            t[50000] = t[50000] + c;
            return t;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }  
}
