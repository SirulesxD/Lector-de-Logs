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

namespace Lector_de_Logs
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void atrasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(); // Crea una instancia de Form1
            form2.Show(); // Muestra Form1
            this.Hide(); //O this.Close(); cierra form2.
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
                    Escribir($"Se encontraron {archivosTXT.Length} archivos .txt {extension} en la carpeta.");
                    Escribir("Buscando archivos .log");
                    archivosTXT = Directory.GetFiles(ruta, "*" + ".log");
                    Escribir($"Se encontraron {archivosTXT.Length} archivos .txt {extension} en la carpeta.");
                    return archivosTXT;
                }
                else
                {
                    // Mostrar la cantidad de archivos .txt encontrados
                    Escribir($"Se encontraron {archivosTXT.Length} archivos {extension} en la carpeta.");
                    return archivosTXT;
                }

            }
            catch (Exception ex)
            {
                Escribir($"Ocurrió un error: {ex.Message}");
            }

            return archivosTXT;
        }
    }
}
