using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lector_de_Logs
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            Escribir("Inicia el proceso");

            if (!File.Exists("G_error.mem"))
            {
                using (StreamWriter escritor = new StreamWriter("G_error.mem", true))
                {
                    escritor.WriteLine("Errores, formatos conocidos de PGReplay");
                    escritor.WriteLine(">ERROR: ");
                    escritor.WriteLine(" ERROR: ");
                    escritor.WriteLine("> ERROR: ");
                }
                Escribir("");
                Escribir(string.Format("El archivo 'G_error.mem' se ha creado y escrito exitosamente."));
            }
            if (!File.Exists("G_statement.mem"))
            {
                using (StreamWriter escritor = new StreamWriter("G_statement.mem", true))
                {
                    escritor.WriteLine("Statement, formatos conocidos de PGReplay");
                    escritor.WriteLine(">STATEMENT: ");
                    escritor.WriteLine(" STATEMENT: ");
                    escritor.WriteLine("> STATEMENT: ");
                }
                Escribir("");
                Escribir(string.Format("El archivo 'G_statement.mem' se ha creado y escrito exitosamente."));
            }
        } //abre la pantala y crea los archivos G_error.mem y G_statement.mem

        private void button1_Click(object sender, EventArgs e)
        {
            String ruta = (String)textBox1.Text;
            string[] archivos;
            string[] resultado = { };
            string txt;
            int i = 0;
            int x = 0;
            int c = 0;
            string[][] memoria = new string[50001][];
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
                }
                else
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
                string nombre = "PG_" + (i + 1) + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                try
                {
                    // Verificar si el archivo existe
                    while (File.Exists(nombre))
                    {
                        i++;
                        nombre = "PG_" + i + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                    }
                }
                catch (Exception ex)
                {
                    Escribir(string.Format("Ocurrió un error: {0}",ex.Message));
                }

                for (i = 0; i < archivos.Length; i++)
                {
                    for (int j = 0; j < memoria.Length; j++)
                    {
                        if (memoria[j] != null)
                        {
                            c++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (c <= x - 1)
                    {
                        Escribir("Abriendo el archivo " + archivos[i]);
                        txt = archivos[i];
                        memoria = Lector(txt, memoria, x);
                    }
                    else
                    {
                        Escribir("Limite de " + x + " registros alcanzado");
                        i = archivos.Length;
                    }
                }
                c = 0;
                for (i = 0; i < memoria.Length; i++)
                {
                    c++;
                    if (memoria[i] == null)
                    {
                        break;
                    }
                }
                string[] lineas = new string[c];
                lineas[0] = "IP¿PUERTO¿BD¿USUARIO¿ID¿ERROR¿SENTENCIA";
                c = 0;
                for (i = 0; i < memoria.Length; i++)
                {
                    c++;
                    if(memoria[i] == null)
                    {
                        break;
                    }

                    for (int j = 0; j < (memoria[0].Length - 1); j++)
                    {
                        lineas[i+1] = lineas[i+1] + memoria[i][j] + "¿";
                    }

                    lineas[i + 1] = lineas[i + 1] + memoria[i][6];
                }
                Escribir("Muestra obtenida de " + (c-1).ToString());
                File.WriteAllLines(nombre, lineas);
            }
            Escribir("Finaliza el programa");
            Escribir("");
        }//iniciar el proceso

        private void atrasToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            this.Close();
        }

        public void Escribir(string texto)
        {
            if (textBox2.Text == "Log")
            {
                textBox2.Text = texto;
                return;
            }

            textBox2.AppendText(Environment.NewLine + texto);
            return;
        }

        public string[] ContadorArchivosTXT(string ruta)
        {
            Escribir("Buscando archivos en la ruta: " + ruta);
            string[] archivosTXT = { };
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

                if (archivosTXT.Length == 0)
                {
                    // Mostrar la cantidad de archivos .txt encontrados
                    Escribir(string.Format("Se encontraron {0} archivos {1} en la carpeta.",archivosTXT.Length,extension));
                    Escribir("Buscando archivos .log");
                    archivosTXT = Directory.GetFiles(ruta, "*" + ".log");
                    Escribir(string.Format("Se encontraron {0} archivos .log en la carpeta.",archivosTXT.Length));
                    return archivosTXT;
                }
                else
                {
                    // Mostrar la cantidad de archivos .txt encontrados
                    Escribir(string.Format("Se encontraron {0} archivos {1} en la carpeta.",archivosTXT.Length,extension));
                    return archivosTXT;
                }

            }
            catch (Exception ex)
            {
                Escribir(string.Format("Ocurrió un error: {0}",ex.Message));
            }

            return archivosTXT;
        }

        private void buscarRutaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Selecciona la carpeta con los logs";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string rutaCarpeta = dialog.SelectedPath;
                this.textBox1.Text = rutaCarpeta;
                //this.BTNIniciar.Enabled = true;
                //this.comboBox1.Enabled = true;
            }
            else
            {
                MessageBox.Show("No se seleccionó ninguna carpeta. /n Por favor, seleccione una carpeta valida.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //this.BTNIniciar.Enabled = false;
                //this.comboBox1.Enabled = false;
            }
        }

        public string[][] Lector(string txt, string[][] memoria, int x)
        {
            int c = 0;
            int p = 0;
            int l = 0;
            int estado=0;
            string ip;
            string sterror="";
            string error = "";
            string statement= "";
            string temp_statement;

            for (int i = 0; i < memoria.Length; i++)
            {
                if (memoria[i] != null)
                {
                    c++;
                }
                else
                {
                    break;
                }
            }

            try
            {
                // Abre el archivo para lectura
                using (StreamReader lector = new StreamReader(txt))
                {
                    // Lee el archivo línea por línea
                    string linea;
                    string linea2;
                    while ((linea = lector.ReadLine()) != null)
                    {
                        l++;
                        if (estado == 5)
                        {
                            if(linea[0] != '<')
                            {

                                string statement2 = linea;
                                char salto = (char)linea[0];
                                while (statement2.IndexOf(salto) > -1)
                                {
                                    statement2 = statement2.Substring(1);
                                } 
                                memoria[p][6] = memoria[p][6] + statement2;
                            }
                            else
                            {
                                estado = 0;
                            }
                        } 

                        if (c < x)
                        {
                            if (estado != 5)
                            {
                                estado = 0;
                            }
                        }
                        else
                        {
                            estado = 6;
                            break;
                        }

                        //Encontrar si la linea es un error
                        if (error == "" && estado == 0)
                        {
                            using (StreamReader lector2 = new StreamReader("G_error.mem"))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    i = 0;
                                    if ((linea2 = lector2.ReadLine()) != null)
                                    {
                                        if ((linea.IndexOf(linea2)) > -1 && estado == 0 && linea2 != null)
                                        {
                                            error = linea2;
                                            estado = 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((linea.IndexOf(error)) > -1 && estado == 0)
                            {
                                estado = 1;
                            }
                        }
                        //Encontrar si la linea es un statement
                        if (statement == "" && estado == 0)
                        {
                            using (StreamReader lector2 = new StreamReader("G_statement.mem"))
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    i = 0;
                                    if ((linea2 = lector2.ReadLine()) != null)
                                    {
                                        if ((linea.IndexOf(linea2)) > -1 && estado == 0 && linea2 != null)
                                        {
                                            statement = linea2;
                                            estado = 4;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((linea.IndexOf(statement)) > -1 && estado == 0)
                            {
                                estado = 4;
                            }
                        }

                        if (estado == 1)
                        {
                            //Verificar si el error ya tiene un id registrado, si no,lo guarda
                            estado = 2;
                            p = 0;
                            while (memoria[p] != null && memoria[p][5] != null && linea != null)
                            {
                                if ((linea.IndexOf(memoria[p][5])) > -1)
                                {
                                    estado = 3;
                                    sterror = Encontrar_error(linea, error);
                                    break;
                                }
                                p++;
                            }

                            //Verificar si el error esta repetido
                            if (estado == 2)
                            {
                                sterror = Encontrar_error(linea, error);
                                p = 0;
                                estado = 2;
                                while (memoria[p] != null && memoria[p][5] != null)
                                {
                                    if (memoria[p][5] == sterror)
                                    {
                                        if(linea.IndexOf(" " + memoria[p][2] + " ") > -1) //Comprobar que sea una BD distinta al error duplicado
                                        {
                                            estado = 3;
                                            break;
                                        }
                                    }
                                    p++;
                                }
                            }

                            //agrega un error nuevo
                            if (estado == 2)
                            {
                                ip = Encontrar_ip(linea) + "|" + sterror + "|";
                                string[] palabras = ip.Split('|');
                                if (palabras.Length == 7)
                                {
                                    memoria[c] = palabras;
                                    c++;
                                }
                            }
                        }
                        //Comparar el id del statement y ver si ya esta registrado con su correspondiente error
                        if (estado == 4)
                        {
                            temp_statement = Encontrar_statement(linea, statement);
                            p = 0;
                            while (memoria[p] != null && memoria[p][4] != null && temp_statement != null)
                            {
                                if (linea.IndexOf(" " + memoria[p][4] + " ") > -1)
                                {
                                    if (memoria[p][5] != null && memoria[p][6] == "")
                                    {
                                        if (temp_statement == "STATEMENT:  ")
                                        {
                                            estado = 5;
                                        }
                                        memoria[p][6] = temp_statement;
                                        break;
                                    }
                                }
                                p++;
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Escribir(string.Format("Error al leer el archivo: {0}",ex.Message));
            }

            return memoria;
        }

        public string Encontrar_ip (string linea)
        {
            int punto;
            int separador;
            int estado = 0;
            string ip = "";
            string puerto = "";
            string[] palabras = linea.Split(' '); // Divide por espacios

            for (int i = 0; i < palabras.Length; i++)
            {
                //encontrar ip y puerto
                if (estado == 0)
                {
                    punto = 0;
                    separador = 0;
                    if ((punto = palabras[i].IndexOf('.')) > 0)
                    {
                        if (punto != palabras[i].LastIndexOf('.'))
                        {
                            if ((separador = palabras[i].IndexOf('(')) > 0)
                            {
                                for (int j = 0; j < separador; j++)
                                {
                                    ip += palabras[i][j];
                                }
                                for (int j = separador + 1; j < palabras[i].Length - 1; j++)
                                {
                                    puerto += palabras[i][j];
                                }
                                ip = ip + "|" + puerto + "|";
                                estado = 1;
                            }
                            else
                            {
                                if ((separador = palabras[i].IndexOf(':')) > 0)
                                {
                                    for (int j = 0; j < separador; j++)
                                    {
                                        ip += palabras[i][j];
                                    }
                                    for (int j = separador + 1; j < palabras[i].Length; j++)
                                    {
                                        puerto += palabras[i][j];
                                    }
                                    ip = ip + "|" + puerto + "|";
                                    estado = 1;
                                }
                            }

                        }
                        else
                        {
                            //nada
                        }
                    }
                    else
                    {
                        if (palabras[i].IndexOf("[local]") > -1)
                        {
                            ip = palabras[i] + "|";
                            estado = 1;
                        }
                    }
                }
                //encontrar usuario y bd
                if (estado == 1)
                {
                    if (palabras[i].Length > 2 && palabras[i][0] == 's')
                    {
                        if (palabras[i][1] == 'y')
                        {
                            if (palabras[i][2] == 's')
                            {
                                ip = ip + "|" + palabras[i - 1];
                                ip = ip + "|" + palabras[i];
                                ip = ip + "|" + palabras[i + 1];

                                estado = 2;
                            }
                        }
                    }
                    else
                    {
                        if (palabras[i] == "postgres" && palabras[i] != "PostgreSQL" && palabras[i + 1].Length > 1)
                        {
                            if (palabras[i + 1] != "postgres" && palabras[i - 1] != "postgres")
                            {
                                if (palabras[i + 1].Length > 0)
                                {
                                    if (palabras[i + 1][0] == 's' && palabras[i].Length > 2)
                                    {
                                        if (palabras[i + 1][1] == 'y')
                                        {
                                            if (palabras[i + 1][2] == 's')
                                            {
                                                ip = ip + "|" + palabras[i];
                                                ip = ip + "|" + palabras[i + 1];
                                                ip = ip + "|" + palabras[i + 2];
                                                estado = 2;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ip = ip + "|" + palabras[i - 1];
                                        ip = ip + "|" + palabras[i];
                                        ip = ip + "|" + palabras[i + 1];
                                        estado = 2;
                                    }
                                }
                            }
                        }
                    }
                    if (palabras[i].IndexOf("postgres") > -1 && i + 1 < palabras.Length)
                    {
                        if (palabras[i + 1].IndexOf("postgres") > -1)
                        {
                            ip = ip + "|" + palabras[i];
                            ip = ip + "|" + palabras[i + 1];
                            ip = ip + "|" + palabras[i + 2];
                            estado = 2;
                        }
                    }
                }
                if (estado == 2)
                {
                    break;
                }
            }
            if (estado != 2)
            {
                ip = "";
            }
            return ip;
        }

        public string Encontrar_error(string linea, string error)
        {
            string tx_error = "";
            int posicion =  linea.IndexOf(error);
            int ierror = error.IndexOf('E');
            tx_error = linea.Substring(posicion + ierror);
            return tx_error;
        }

        public string Encontrar_statement(string linea, string error)
        {
            string tx_error = "";
            int posicion = linea.IndexOf(error);
            int ierror = error.IndexOf('S');
            tx_error = linea.Substring(posicion + ierror);
            return tx_error;
        }

        private void button4_Click(object sender, EventArgs e) //Muestra los formatos de los errores
        {
            textBox2.Text = "Log";
            using (StreamReader lector2 = new StreamReader("G_error.mem"))
            {
                // Lee el archivo línea por línea
                string comprobar;
                while ((comprobar = lector2.ReadLine()) != null)
                {
                    Escribir(comprobar);
                }
            }
            button3.Visible = true;
            button2.Visible = true;
            label4.Text = "error";
        }

        private void button5_Click(object sender, EventArgs e) //Muestra los formatos de los Statement
        {
            textBox2.Text = "Log";
            using (StreamReader lector2 = new StreamReader("G_statement.mem"))
            {
                // Lee el archivo línea por línea
                string comprobar;
                while ((comprobar = lector2.ReadLine()) != null)
                {
                    Escribir(comprobar);
                }
            }
            button3.Visible = true;
            button2.Visible = true;
            label4.Text = "statement";
        }

        private void button3_Click(object sender, EventArgs e) //Cancela la operacion y limpia el log
        {
            button3.Visible = false;
            button2.Visible = false;
            label4.Text = "temp";
            textBox2.Text = "Log";
        }

        private void button2_Click(object sender, EventArgs e) //Guarda el dato de los formatos
        {
            string linea = textBox2.Text;
            textBox2.Text = "Log";
            Escribir(linea);
            if (label4.Text == "error")
            {
                if (File.Exists("G_error.mem"))
                {
                    File.Delete("G_error.mem");
                    using (StreamWriter escritor = new StreamWriter("G_error.mem", true))
                    {
                        escritor.WriteLine(linea);
                    }
                }
            }
            if (label4.Text == "statement")
            {
                if (File.Exists("G_statement.mem"))
                {
                    File.Delete("G_statement.mem");
                    using (StreamWriter escritor = new StreamWriter("G_statement.mem", true))
                    {
                        escritor.WriteLine(linea);
                    }
                }
            }

            label4.Text = "temp";
        }


    }
}
