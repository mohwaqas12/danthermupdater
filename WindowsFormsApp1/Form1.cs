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
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //richTextBox1.Enabled = false;
            //uploadFilePath = null;
            connectionInfo = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text File (*.txt)|*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filepath = openFileDialog1.FileName;
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(filepath);
                listBox1.Items.Clear();
                while ((line = file.ReadLine()) != null)
                {

                    listBox1.Items.Add(line);
                }
            }

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        public delegate void InvokeDelegate(string text);

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => uploadFile());


        }

        private void uploadFile()
        {
                      
            // ########### REmove later  . Did this for testing.
           // label5.Text = "192.168.1.100";
            updateFile = "C:\\Users\\waath1\\Desktop\\sitelogic-0.8.10.plc";
            uploadFilePath = new System.IO.FileStream(updateFile, System.IO.FileMode.Open);
            string username = "";
            string password = "";
            string ip = "192.168.1.100";
            // ##################
            Dantherm device1 = new Dantherm(ip, username, password);
            try
            {
                progressBar1.Maximum = (int)uploadFilePath.Length;
                uploadFilePath.Close();
                
                updatetext("Connecting to Device");
                device1.ftpConnect();
                updatetext("\nConnected to Device\n");
               
                device1.uploadFile(updateFile, UploadProgress);

                device1.ftpDisconnect();

                //Run a state machine here. 
                // We will do steps like 
                //device1.makeDirectory();
                //device1.copyFiletoDirectory();
                //device1.setRunPermissions();
                //device1.runInstaller();
                //device1.verifyInstall();
                //device1.checkVersion();
       
                //updatetext("File Uploaded\n");
                    
              
                //runInstaller();

            }
            catch (Exception er)
            {
                updatetext(er.ToString());
            }

        }
        private void UploadProgress(ulong uploaded)
        {
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)uploaded; });
        }

    private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Plc Update File (*.plc)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filepath = openFileDialog1.FileName;

                updateFile = filepath;
                textBox1.Text = filepath;
                uploadFilePath = new System.IO.FileStream(filepath, System.IO.FileMode.Open);
               
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label5.Text = listBox1.SelectedItem.ToString();
        }

        // Not used anymore
        private void runInstaller()
        {
            try
            {

                ssh.Connect();

               updatetext("Connected to SSH\n");

                if (ssh.IsConnected)
                {
                    string command;
                    string fileName = Path.GetFileName(updateFile);
                    updatetext(RunCommand(ssh, "sudo mkdir -p /tmp/bulkUpdater"));
                    updatetext(RunCommand(ssh, "sudo mv /tmp/" + fileName + " /tmp/bulkUpdater/"));
                    //command = "cd /tmp/bulkUpdater; sudo chmod +x " + fileName + " ; sudo ./" + fileName + " extract >> $(date +Log_%d_%m_%y_%H_%M_%S)";
                    command = "cd /tmp/bulkUpdater; sudo chmod +x " + fileName + " ; sudo ./" + fileName + " extract ";
                    updatetext(command + "\n");
                    updatetext(RunCommand(ssh, command) + "\n");

                    // Verify installed software
                    ssh.Disconnect();
                    progressBar1.Value += 30;
                }

            }
            catch (Exception er)
            {
                updatetext(er.ToString());
            }


        }

        public delegate void invoke(string text);
        
         public void settext(string text)
        {
            richTextBox1.AppendText(text);
        }
        public void updatetext(string text)
        {
            richTextBox1.Invoke(new invoke(settext), text);
        }
        private string RunCommand(SshClient sshClient, string command)
        {
            if (sshClient.IsConnected)
            {
        
                SshCommand cmd = sshClient.RunCommand(command);
                if (cmd.ExitStatus == 0)
                    return cmd.Result + "\n";
                else
                    return "Error " + cmd.Error + "\n";
            }
            
            return "";
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}