using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Net.Http;
using System.Xml.Linq;
using System.Web;
using System.Security.Claims;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Data.Common;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Net.NetworkInformation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Projeto1LTI
{

    public partial class Form2 : Form
    {
        
        //Definição das variáveis utilizadas globalmente
        string routerOSIpAddress;
        string porto;
        string baseUrl;

        int numeroNodes;
        int numeroNamespaces;
        int numeroPods;
        int numeroDeployments;
        int numeroServices;

        public Form2(string Ip, string Porto)
        {
            InitializeComponent();

            this.MouseDown += MainForm_MouseDown;
            this.MouseMove += MainForm_MouseMove;
            this.MouseUp += MainForm_MouseUp;


            //Ignorar erros de validação do SSL/TLS 
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //Preencher variáveis passadas do Form 1
            routerOSIpAddress = Ip;
            porto = Porto;
            baseUrl = "http://" + routerOSIpAddress + ":" + porto + "/api/v1";

        }


        //Cria todos os elemetos que necessitamos para executar todas as funções quando é iniciado o programa
        private void Form1_Load(object sender, EventArgs e)
        {
            //Começar com tudo invisível
            comboBox1.Visible = false;
            comboBox3.Visible = false;
            textBox4.Visible = false;
            textBox3.Visible = false;
            textBox2.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            comboBox3.Visible = false;
            label5.Visible = false;

            numeroNodes = 0;
            numeroNamespaces = 0;
            numeroPods = 0;
            numeroDeployments = 0;
            numeroServices= 0;
            contarNodes();
            contarNamespaces();
            contarPods();
            contarDeployments();
            contarServices();
            listarDashboard();
            label8.Text = numeroNodes.ToString();
            label10.Text = numeroNamespaces.ToString();
            label12.Text = numeroPods.ToString();
            label14.Text = numeroDeployments.ToString();
            label16.Text = numeroServices.ToString();

        }

        //Chama a função cluster quando se clica na pictureBox
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            numeroNodes = 0;
            numeroNamespaces = 0;
            numeroPods = 0;
            numeroDeployments = 0;
            numeroServices = 0;
            panel6.Visible = true;
            panel7.Visible = true;
            panel8.Visible = true;
            panel9.Visible = true;
            panel10.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label13.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            label16.Visible = true;
            contarNodes();
            contarNamespaces();
            contarPods();
            contarDeployments();
            contarServices();
            listarDashboard();
            label8.Text = numeroNodes.ToString();
            label10.Text = numeroNamespaces.ToString();
            label12.Text = numeroPods.ToString();
            label14.Text = numeroDeployments.ToString();
            label16.Text = numeroServices.ToString();
            dataGridView1.Columns[3].HeaderText = "IP";
            dataGridView1.Columns[4].HeaderText = "Up-Time CPU";
            dataGridView1.Columns[5].HeaderText = "Memória Usada";
            dataGridView1.Columns[6].HeaderText = "Memória por Alocar";
            dataGridView1.Columns[3].Width = 75;
            dataGridView1.Columns[4].Width = 125;
            dataGridView1.Columns[5].Width = 150;
            dataGridView1.Columns[6].Width = 175;
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[2].Visible = true;
            dataGridView1.Columns[3].Visible = true;
            dataGridView1.Columns[4].Visible = true;
            dataGridView1.Columns[5].Visible = true;
            dataGridView1.Columns[6].Visible = true;
        }

        #region Dashboard
        //Listar Dashboard
        private void listarDashboard()
        {
            limparForm();
            try
            {
                // Criar um novo pedido GET Http
                string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "metrics.k8s.io/v1beta1/nodes");
                request.Method = "GET";

                // Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        versao();
                        int rowIndex = 0;
                        string memoriaPorAlocar;
                       
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();

                            JObject usage = node["usage"] as JObject; // Try to get the "usage" object

                            if (usage != null) // If "usage" is present
                            {
                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                string cpuUsage = usage["cpu"].Value<string>();
                                string cpuString = Regex.Replace(cpuUsage, "[^0-9]", "");
                                float resultCpu;
                                float.TryParse(cpuString, out resultCpu);
                                resultCpu = resultCpu / 1000000;
                                int IntCPU = (int)Math.Round(resultCpu);


                                string memoryUsage = usage["memory"].Value<string>();
                                string cpuMemoryUsage = Regex.Replace(memoryUsage, "[^0-9]", "");
                                float resultMemoryUsage;
                                float.TryParse(cpuMemoryUsage, out resultMemoryUsage);
                                resultMemoryUsage = resultMemoryUsage / 1024;
                                int IntMemoryUsage = (int)Math.Round(resultMemoryUsage);

                                memoriaPorAlocar = memoria(node["metadata"]["name"].Value<string>());
                                string cpuMemoriaPorAlocar = Regex.Replace(memoriaPorAlocar, "[^0-9]", "");
                                float resultMemoriaPorAlocar;
                                float.TryParse(cpuMemoriaPorAlocar, out resultMemoriaPorAlocar);
                                resultMemoriaPorAlocar = resultMemoriaPorAlocar / 1024;
                                int IntMemoriaPorAlocar = (int)Math.Round(resultMemoriaPorAlocar);

                                IntMemoriaPorAlocar = IntMemoriaPorAlocar - IntMemoryUsage;

                                // Create a new DataGridViewRow and set its values
                                row.Cells[0].Value = name;
                                row.Cells[2].Value = timestamp;
                                row.Cells[4].Value = IntCPU.ToString() + " milisegundos";
                                row.Cells[5].Value = IntMemoryUsage.ToString() + " MegaBytes";
                                row.Cells[6].Value = IntMemoriaPorAlocar.ToString() + " MegaBytes";
                                rowIndex++;
                            }
                        }
                    }

                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        //Memoria por alocar
        private string memoria(string nome)
        {
           
            string memoria = "";
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/nodes/"+nome);
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());

                    if (responseJson != null) // If "items" is an array of nodes
                    {
                        memoria = responseJson["status"]["capacity"]["memory"].Value<string>();
                        return memoria;                     
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
               
            }
            return memoria;
        }

        //Versao Nodes
        private void versao()
        {
            limparForm();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/nodes");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string ip = node["status"]["addresses"]
                            .Where(x => x["type"].Value<string>() == "InternalIP")
                            .Select(x => x["address"].Value<string>())
                            .FirstOrDefault();

                            // Create a new DataGridViewRow and set its values
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[1].Value = version;
                            row.Cells[3].Value = ip;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void contarNodes()
        {
            limparForm();
            label5.Visible = false;

            numeroNodes = 0;

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/nodes");
                request.Method = "GET";


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            numeroNodes++;
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //contarNamespaces
        private void contarNamespaces()
        {
            limparForm();
            label5.Visible = false;

            numeroNamespaces = 0;

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/namespaces");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            numeroNamespaces++;
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Contar Pods
        private void contarPods()
        {
            limparForm();
            label5.Visible = false;

            numeroPods = 0;

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/pods");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            numeroPods++;
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Contar Deployments
        private void contarDeployments()
        {
            limparForm();
            label5.Visible = false;

            string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/apps/v1/deployments";

            numeroDeployments = 0;

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            numeroDeployments++;
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //Contar Services
        private void contarServices()
        {
            limparForm();
            label5.Visible = false;

            numeroServices = 0;

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/services");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            numeroServices++;
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Listar

        //Listar os nodes
        private void nodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Listar";
            
            
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/nodes");
                request.Method = "GET";


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();
                            string OS = node["metadata"]["labels"]["beta.kubernetes.io/os"].Value<string>();
                            string ImagemOS = node["status"]["nodeInfo"]["osImage"].Value<string>();
                            string Arquitetura = node["status"]["nodeInfo"]["architecture"].Value<string>();

                            string memoriaPorAlocar = memoria(name);
                            string cpuMemoriaPorAlocar = Regex.Replace(memoriaPorAlocar, "[^0-9]", "");
                            float resultMemoriaPorAlocar;
                            float.TryParse(cpuMemoriaPorAlocar, out resultMemoriaPorAlocar);
                            resultMemoriaPorAlocar = resultMemoriaPorAlocar / 1024;
                            int IntMemoriaPorAlocar = (int)Math.Round(resultMemoriaPorAlocar);

                            // Create a new DataGridViewRow and set its values
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[0].Value = name;
                            row.Cells[1].Value = version;
                            row.Cells[2].Value = timestamp;
                            row.Cells[3].Value = OS;
                            row.Cells[4].Value = ImagemOS;
                            row.Cells[5].Value = Arquitetura;
                            row.Cells[6].Value = IntMemoriaPorAlocar.ToString() + " MegaBytes";
                            dataGridView1.Columns[3].HeaderText = "Sistema Operativo";
                            dataGridView1.Columns[4].HeaderText = "Imagem OS";
                            dataGridView1.Columns[5].HeaderText = "Arquitetura";
                            dataGridView1.Columns[6].HeaderText = "Memoria por alocar";
                            dataGridView1.Columns[0].Visible = true;
                            dataGridView1.Columns[1].Visible = true;
                            dataGridView1.Columns[2].Visible = true;
                            dataGridView1.Columns[3].Visible = true;
                            dataGridView1.Columns[4].Visible = true;
                            dataGridView1.Columns[5].Visible = true;
                            dataGridView1.Columns[6].Visible = true;
                            dataGridView1.Columns[3].Width = 75;
                            dataGridView1.Columns[4].Width = 125;
                            dataGridView1.Columns[5].Width = 100;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Chama a função Listar Namespaces
        private void namespacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Listar";
            listarNamespaces();
        }

        //Listar Namespaces
        public void listarNamespaces()
        {

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/namespaces");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    comboBox1.Items.Clear();
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            comboBox1.Items.Add(name);
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();
                            string estado = node["status"]["phase"].Value<string>();
                            string UID = node["metadata"]["uid"].Value<string>();
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[0].Value = name;
                            row.Cells[1].Value = version;
                            row.Cells[2].Value = timestamp;
                            row.Cells[3].Value = estado;
                            row.Cells[4].Value = UID;

                            dataGridView1.Columns[0].Visible = true;
                            dataGridView1.Columns[1].Visible = true;
                            dataGridView1.Columns[2].Visible = true;
                            dataGridView1.Columns[3].Visible = true;
                            dataGridView1.Columns[4].Visible = true;

                            dataGridView1.Columns[3].HeaderText = "Estado"; 
                            dataGridView1.Columns[4].HeaderText = "UID";
                            dataGridView1.Columns[3].Width = 75;
                            dataGridView1.Columns[4].Width = 250;
                            dataGridView1.Columns[5].Visible = false;
                            dataGridView1.Columns[6].Visible = false;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Listar Namespaces 2
        public void listarNamespaces2()
        {


            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/namespaces");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    comboBox1.Items.Clear();
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            comboBox1.Items.Add(name);
                        }
                    }
                    else // If "items" is a single node object
                    {
                        string name = responseJson["metadata"]["name"].Value<string>();
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Chama a função Listar Pods
        private void podsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Listar";
            listarPods();
            
        }


        //Listar Pods 2
        public void listarPods()
        {
        
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/pods");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property

                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();
                            string estado = node["status"]["phase"].Value<string>();
                            string dnsPolicy = node["spec"]["dnsPolicy"].Value<string>();
                            string namespaces = node["metadata"]["namespace"].Value<string>();
                            string UID = node["metadata"]["uid"].Value<string>();


                            // Create a new DataGridViewRow and set its values
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[0].Value = name;
                            row.Cells[1].Value = version;
                            row.Cells[2].Value = timestamp;
                            row.Cells[3].Value = estado;
                            row.Cells[4].Value = namespaces;
                            row.Cells[5].Value = dnsPolicy;
                            row.Cells[6].Value = UID;

                            dataGridView1.Columns[3].HeaderText = "Estado";
                            dataGridView1.Columns[4].HeaderText = "Namespaces";
                            dataGridView1.Columns[5].HeaderText = "Política DNS";
                            dataGridView1.Columns[6].HeaderText = "UID";
                            dataGridView1.Columns[3].Width = 60;
                            dataGridView1.Columns[4].Width = 100;
                            dataGridView1.Columns[5].Width = 100;
                            dataGridView1.Columns[6].Width = 250;
                            dataGridView1.Columns[0].Visible = true;
                            dataGridView1.Columns[1].Visible = true;
                            dataGridView1.Columns[2].Visible = true;
                            dataGridView1.Columns[3].Visible = true;
                            dataGridView1.Columns[4].Visible = true;
                            dataGridView1.Columns[5].Visible = true;
                            dataGridView1.Columns[6].Visible = true;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }
                    else // If "items" is a single node object
                    {
                        string name = responseJson["metadata"]["name"].Value<string>();
                        string version = responseJson["metadata"]["resourceVersion"].Value<string>();
                        string timestamp = responseJson["metadata"]["creationTimestamp"].Value<string>();
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dataGridView1);
                        row.Cells[0].Value = name;
                        row.Cells[1].Value = version;
                        row.Cells[2].Value = timestamp;

                        // Add the new row to the DataGridView
                        dataGridView1.Rows.Add(row);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //Listar Pods
        public void listarPods2()
        {

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/pods");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    comboBox3.Items.Clear();
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            comboBox3.Items.Add(name);
                        }
                    }
                    else // If "items" is a single node object
                    {
                        string name = responseJson["metadata"]["name"].Value<string>();
                        comboBox3.Items.Add(name);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        //Chama a função Listar Deployments
        private void deploymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Listar";
            listarDeployments();
        }


        //Lista Deployments
        public void listarDeployments()
        {

            string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/apps/v1/deployments";

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();
                            string namespaces = node["metadata"]["namespace"].Value<string>();
                            string replicas = node["status"]["replicas"].Value<string>();
                            string UID = node["metadata"]["uid"].Value<string>();
                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[0].Value = name;
                            row.Cells[1].Value = version;
                            row.Cells[2].Value = timestamp;
                            row.Cells[3].Value = namespaces;
                            row.Cells[4].Value = replicas;
                            row.Cells[5].Value = UID;

                            dataGridView1.Columns[3].Width = 125;
                            dataGridView1.Columns[4].Width = 75;
                            dataGridView1.Columns[5].Width = 250;
                            dataGridView1.Columns[3].HeaderText = "Namespaces";
                            dataGridView1.Columns[4].HeaderText = "Replicas";
                            dataGridView1.Columns[0].Visible = true;
                            dataGridView1.Columns[1].Visible = true;
                            dataGridView1.Columns[2].Visible = true;
                            dataGridView1.Columns[3].Visible = true;
                            dataGridView1.Columns[4].Visible = true;
                            dataGridView1.Columns[5].Visible = true;
                            dataGridView1.Columns[6].Visible = false;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }

                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Lista Deployments
        public void listarDeployments2()
        {

            string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/apps/v1/deployments";

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    comboBox3.Items.Clear();
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            comboBox3.Items.Add(name);
                        }
                    }
                    else // If "items" is a single node object
                    {
                        string name = responseJson["metadata"]["name"].Value<string>();
                        comboBox3.Items.Add(name);
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Chama a função Listar Services
        private void servicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Listar";
            listarServices();
        }


        //Listar Services
        public void listarServices()
        {
            

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/services");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            string version = node["metadata"]["resourceVersion"].Value<string>();
                            string timestamp = node["metadata"]["creationTimestamp"].Value<string>();
                            string ipCluster = node["spec"]["clusterIP"].Value<string>();
                            string namespaces = node["metadata"]["namespace"].Value<string>();
                            string ipFamilies = node["spec"]["ipFamilyPolicy"].Value<string>();
                            string UID = node["metadata"]["uid"].Value<string>();

                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(dataGridView1);
                            row.Cells[0].Value = name;
                            row.Cells[1].Value = version;
                            row.Cells[2].Value = timestamp;
                            row.Cells[3].Value = ipCluster;
                            row.Cells[4].Value = namespaces;
                            row.Cells[5].Value = ipFamilies;
                            row.Cells[6].Value = UID;

                            dataGridView1.Columns[3].HeaderText = "IP Cluster";
                            dataGridView1.Columns[4].HeaderText = "Namespaces";
                            dataGridView1.Columns[5].HeaderText = "Política Familia IPs";
                            dataGridView1.Columns[6].HeaderText = "UID";

                            dataGridView1.Columns[3].Width = 100;
                            dataGridView1.Columns[4].Width = 125;
                            dataGridView1.Columns[5].Width = 150;
                            dataGridView1.Columns[6].Width = 250;

                            dataGridView1.Columns[0].Visible = true;
                            dataGridView1.Columns[1].Visible = true;
                            dataGridView1.Columns[2].Visible = true;
                            dataGridView1.Columns[3].Visible = true;
                            dataGridView1.Columns[4].Visible = true;
                            dataGridView1.Columns[5].Visible = true;
                            dataGridView1.Columns[6].Visible = true;

                            // Add the new row to the DataGridView
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void listarServices2()
        {


            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/services");
                request.Method = "GET";

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    JArray nodes = responseJson["items"] as JArray; // Try to get an array of nodes from the "items" property
                    comboBox3.Items.Clear();
                    if (nodes != null) // If "items" is an array of nodes
                    {
                        foreach (JObject node in nodes)
                        {
                            string name = node["metadata"]["name"].Value<string>();
                            comboBox3.Items.Add(name);
                        }
                    }
                    else // If "items" is a single node object
                    {
                        string name = responseJson["metadata"]["name"].Value<string>();
                        comboBox3.Items.Add(name);
                    }
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        #endregion

        #region Criar

        //Chama a função para criar namespaces
        private void namespacesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Criar";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Criar";
            comboBox1.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label3.Visible = true;
            label4.Visible = false;
            label1.Text = "Nome Namespace";
            label2.Text = "";
            label3.Text = "";
            label1.Visible = true;
            label2.Visible = false;
            textBox1.Visible = true;
            textBox2.Visible = false;
            button1.Visible = true;
            desassociarFuncoes();
            listarNamespaces();
            button1.Click += new EventHandler(criarNamespaces);
        }

        //Criar namespaces
        private void criarNamespaces(object sender, EventArgs e)
        {
            string url = $"{baseUrl}/namespaces/";

            string name = textBox1.Text;

            string payload = "{\"apiVersion\":\"v1\",\"kind\":\"Namespace\",\"metadata\":{\"name\":\""+ name +"\"}}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
               

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }
           
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    MessageBox.Show("Criado com sucesso ","", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }


        //Chama a função para criar pods
        private void podsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Criar";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Criar";
            comboBox1.Visible = true;
            label2.Text = "Nome Pods";
            textBox3.Visible = false;
            textBox4.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label4.Text = "Porto";
            label1.Text = "Namespace";
            label2.Visible = true;
            label3.Text = "Imagem";
            label1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            button1.Visible = true;
            comboBox2.Visible = true;
            desassociarFuncoes();
            listarNamespaces2();
            listarPods();
            button1.Click += new EventHandler(criarPods);
        }

        //Criar pods
        private void criarPods(object sender, EventArgs e)
        {

            string namespaceName = comboBox1.Text;
            string name = textBox2.Text;
            string image = comboBox2.Text;
            string porto = textBox4.Text;
            string url = $"{baseUrl}/namespaces/{namespaceName}/pods/";

            
            string payload = "{\"apiVersion\":\"v1\",\"kind\":\"Pod\",\"metadata\":{\"name\":\"" + name +"\",\"labels\":{\"app\":\"myapp\"}},\"spec\":{\"containers\":[{\"name\":\"mycontainer\",\"image\":\""+image+"\",\"ports\":[{\"containerPort\":"+porto+"}]}]}}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";


                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    MessageBox.Show("Criado com sucesso ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }


        //Chama a função para criar Deployments quando se clica no botão criar
        private void deploymentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Criar";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Criar";
            comboBox1.Visible = true;
            button1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = true;
            comboBox2.Visible = true;
            label3.Visible = true;
            label3.Text = "Imagem";
            label1.Visible = true;
            label1.Text = "Nome da Namespace";
            label2.Visible = true;
            label2.Text = "Nome do deployment";
            label4.Visible = false;
            label4.Text = "Porto";
            label4.Visible = true;
            textBox5.Visible = true;
            label17.Visible = true;
            label17.Text = "Replicas";
            label5.Visible= true;
            desassociarFuncoes();
            listarDeployments();
            listarNamespaces2();
            button1.Click += new EventHandler(criarDeployment);
        }

        //Criar deployments
        private void criarDeployment(object sender, EventArgs e)
        {

            string namespaceName = comboBox1.Text;
            string nameDeployments = textBox2.Text;
            string image = comboBox2.Text;
            string porto2 = textBox4.Text;
            string replicas = textBox5.Text;
            string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/apps/v1/namespaces/" + namespaceName + "/deployments";

            string payload = $@"{{
                        ""apiVersion"": ""apps/v1"",
                        ""kind"": ""Deployment"",
                        ""metadata"": {{
                            ""name"": ""{nameDeployments}"",
                            ""labels"": {{
                                ""app"": ""myapp""
                            }}
                        }},
                        ""spec"": {{
                            ""selector"": {{
                                ""matchLabels"": {{
                                    ""app"": ""myapp""
                                }}
                            }},
                            ""replicas"": {replicas},
                            ""template"": {{
                                ""metadata"": {{
                                    ""labels"": {{
                                        ""app"": ""myapp""
                                    }}
                                }},
                                ""spec"": {{
                                    ""containers"": [
                                        {{
                                            ""name"": ""mycontainer"",
                                            ""image"": ""{image}"",
                                            ""ports"": [
                                                {{
                                                    ""containerPort"": {porto2}
                                                }}
                                            ]
                                        }}
                                    ]
                                }}
                            }}
                        }}
                    }}";


            try
            {
                // Create a new Http POST request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // Write the payload to the request body
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                // Send the request and get the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Read the response
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    MessageBox.Show("Criado com sucesso", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Close the response
                response.Close();
            }
            catch (WebException error)
            {
                // Request error
                using (var stream = error.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: " + responseText, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Chama a função para criar Services
        private void servicesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Criar";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            button1.Text = "Criar";
            comboBox1.Visible = true;
            comboBox3.Visible = true;
            button1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = true;
            textBox4.Visible = true;
            textBox5.Visible = true;
            label1.Visible = true;
            label1.Text = "Nome da Namespace";
            label2.Visible = true;
            label2.Text = "Nome do Pod";
            label3.Visible = true;
            label4.Visible = true;
            label3.Text = "Nome do Service";
            label4.Text = "Endereço IP";
            label17.Visible = true;
            label17.Text = "Porto";
            desassociarFuncoes();
            listarNamespaces2();
            listarPods2();
            listarServices();
            button1.Click += new EventHandler(criarServices);
        }

        //Criar Services
        private void criarServices(object sender, EventArgs e)
        {

            string namespaceName = comboBox1.Text;
            string namePod = comboBox3.Text;
            string nameService = textBox3.Text;
            string porto = textBox5.Text;
            string endereçoIP = textBox4.Text;
            string url = $"{baseUrl}/namespaces/{namespaceName}/services";


            //Criar um novo pedido POST Http
            string payload = $@"{{
                    ""apiVersion"": ""v1"",
                    ""kind"": ""Service"",
                    ""metadata"": {{
                        ""name"": ""{nameService}"",
                        ""labels"": {{
                            ""app"": ""myapp""
                        }}
                    }},
                    ""spec"": {{
                        ""selector"": {{
                            ""app"": ""myapp"",
                            ""name"": ""{namePod}""
                        }},
                        ""ports"": [
                            {{
                                ""name"": ""http"",
                                ""port"": {porto},
                                ""targetPort"": 80
                            }}
                        ],
                        ""clusterIP"": ""{endereçoIP}""
                    }}
                }}";

            try
            {
                // Create a new Http POST request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                // Write the payload to the request body
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                // Send the request and get the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Read the response
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    MessageBox.Show("Criado com sucesso", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Close the response
                response.Close();
            }
            catch (WebException ex)
            {
                // Request error
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    MessageBox.Show("Erro: " + responseText, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Eliminar

        //Chama a função para eliminar namespaces
        private void namespacesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Eliminar";
            listarNamespaces();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Nome do Namespace";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarNamespaces);

        }
       

        //Eliminar namespaces
        private void eliminarNamespaces(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                string name = comboBox1.Text;
                string url = $"{baseUrl}/namespaces/{name}";

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        MessageBox.Show("Eliminado com sucesso ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //Chama a função para eliminar Pods
        private void podsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Eliminar";
            listarPods();
            listarPods2();
            listarNamespaces2();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = true;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Nome do namespace";
            label2.Visible = true;
            label2.Text = "Nome do Pods";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarPods);
        }
  

        //Eliminar Pods
        private void eliminarPods(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                string namespaceName = comboBox1.Text;
                string podName = comboBox3.Text;
                string url = $"{baseUrl}/namespaces/{namespaceName}/pods/{podName}";

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        MessageBox.Show("Eliminado com sucesso ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        //Chama a função para eliminar deployments quando se clica no botão eliminar 
        private void deploymentsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Eliminar";
            listarDeployments();
            listarDeployments2();
            listarNamespaces2();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = true;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Nome do namespace";
            label2.Visible = true;
            label2.Text = "Nome do Deployment";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarDeployment);
        }


        //Eliminar deployments
        private void eliminarDeployment(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                string namespaceName = comboBox1.Text;
                string deploymentName = comboBox3.Text;
                string url = "http://" + routerOSIpAddress + ":" + porto + "/apis/apps/v1/namespaces/" + namespaceName + "/deployments/"+deploymentName;

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        MessageBox.Show("Eliminado com sucesso ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para eliminar Services quando se clica no botão eliminar
        private void servicesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            limparCluster();
            label5.Text = "Eliminar";
            listarServices();
            listarServices2();
            listarNamespaces2();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox3.Text = "";
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox3.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Nome do namespace";
            label2.Visible = true;
            label2.Text = "Nome do Service";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarServices);
        }


        //Eliminar Services
        private void eliminarServices(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                
                string namespaceName = comboBox1.Text;
                string serviceName = comboBox3.Text;
                string url = $"{baseUrl}/namespaces/{namespaceName}/services/{serviceName}";

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        MessageBox.Show("Eliminado com sucesso ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        MessageBox.Show("Erro: ", responseText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Click -= button1_Click;
        }

        //Desassocia as funções dos botões
        private void desassociarFuncoes()
        {
            button1.Click -= criarNamespaces;
            button1.Click -= criarPods;
            button1.Click -= criarDeployment;
            button1.Click -= criarServices;
            button1.Click -= eliminarNamespaces;
            button1.Click -= eliminarPods;
            button1.Click -= eliminarDeployment;
            button1.Click -= eliminarServices;
        }

        //Mete a invisible o que não precisamos de ocultar no form
        private void limparForm()
        {
            dataGridView1.Rows.Clear();
            button1.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible=false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible= false;
            label2.Visible=false;
            label3.Visible=false;
            label4.Visible=false;
            comboBox1.Visible=false;
            comboBox3.Visible=false;
            comboBox2.Visible=false;
            textBox5.Visible=false;
            label17.Visible=false;
        }

        //Limpa o dashboard
        private void limparCluster()
        {
            label5.Visible=true;
            label7.Visible = false;
            label8.Visible=false;
            panel6.Visible=false;
            panel7.Visible = false;
            panel8.Visible = false;
            panel9.Visible = false;
            panel10.Visible = false;
        }

        #region Form

        //Permite dar drag ao form

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

        //Permite quando fechamos o Form 2, mostrar o Form 1
        private void label6_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Form1 newForm = new Form1();
            newForm.Show();
        }

    }
}
    

   

