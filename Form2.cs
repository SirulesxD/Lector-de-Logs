using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lector_de_Logs
{
    public partial class Form2 : Form
    {
        //[STAThread]
        public Form2()
        {
            InitializeComponent();
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }

        private void button1_click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(); // Crea una instancia de Form1
            form1.Show(); // Muestra Form1
            this.Hide(); //O this.Close(); cierra form2.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(); // Crea una instancia de Form1
            form3.Show(); // Muestra Form1
            this.Hide(); //O this.Close(); cierra form2.
        }
    }
}
