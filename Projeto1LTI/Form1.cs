using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto1LTI
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

            //Ignorar erros de validação do SSL/TLS 
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            this.MouseDown += MainForm_MouseDown;
            this.MouseMove += MainForm_MouseMove;
            this.MouseUp += MainForm_MouseUp;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Variáveis utilizadas para fazer o pedido Get
            string routerOSIpAddress = textBox1.Text;
            string porto = textBox2.Text;

            //Endpoint do pedido Get para verificar se a conexão ao router é sucedida ou não
            string baseUrl = "http://" + routerOSIpAddress + ":" + porto  + "/api";

            //Verificar se as text box foram preenchidas
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Tem de preencher as credenciais", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    //Pedido request para ver se a conexão ao router foi bem sucedida 
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
                    request.Method = "GET";

                
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Conexão sucedida
                        Form2 newForm = new Form2(routerOSIpAddress, porto);
                        newForm.Show();
                        this.Hide();

                        textBox1.Text = "";
                        textBox2.Text = "";
                       
                    }
                    else
                    {
                        // Connection não foi sucedida
                        MessageBox.Show("Credenciais inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (WebException ex)
                {
                    // Erro do pedido
                    MessageBox.Show("Credenciais inválidas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Form
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = this.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int dx = Cursor.Position.X - lastCursor.X;
                int dy = Cursor.Position.Y - lastCursor.Y;
                this.Location = new Point(lastForm.X + dx, lastForm.Y + dy);
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
        #endregion
    }
}
