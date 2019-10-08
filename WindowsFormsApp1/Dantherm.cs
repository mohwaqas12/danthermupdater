using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;

namespace WindowsFormsApp1
{
    class Dantherm
    {
        private
        string ip;
        string username;
        string password;
        string uploadFilePath;
        string log;
        string fileName;
        private Renci.SshNet.ConnectionInfo connectionInfo;
        private Renci.SshNet.SftpClient ftp;
        private Renci.SshNet.SshClient ssh;

        public Dantherm(string ip, string username, string password)
        {
            this.ip = ip;
            this.username = username;
            this.password = password;
            this.connectionInfo = new ConnectionInfo(this.ip, username, new PasswordAuthenticationMethod(username, password),
                                       new PrivateKeyAuthenticationMethod("rsa.key"));
        }

        public void sshConnect()
        {
            ssh = new SshClient(connectionInfo);
            ssh.Connect();
        }
        
        public void sshDisconnect()
        {
            if (this.ssh.IsConnected)
                this.ssh.Disconnect();
        }

        public void ftpConnect()
        {
            ftp = new SftpClient(connectionInfo);
            ftp.Connect();
        }

        public void ftpDisconnect()
        {
            ftp.Disconnect();
        }
        public string sshRunCommand(string command)
        {
            if (ssh.IsConnected)
            {
                SshCommand cmd = ssh.RunCommand(command);
                if (cmd.ExitStatus == 0)
                    return cmd.Result + "\n";
                else
                    return "Error " + cmd.Error + "\n";
            }
            return "Warning: not connected to SSH";
        }
        public void uploadFile(string localpath, Action<ulong> uploadProgress=null)
        {
            System.IO.FileStream filestream = new System.IO.FileStream(localpath, System.IO.FileMode.Open);
            fileName = Path.GetFileName(localpath);
            ftp.UploadFile(filestream, "/tmp/" + fileName, uploadProgress);
            filestream.Close();
            ftp.Disconnect();
        }

        public string makeDirectory()
        {
            return sshRunCommand("sudo mkdir -p /tmp/bulkUpdater");
        }

        public string copyFiletoDirectory()
        {
            return sshRunCommand("sudo mv /tmp/" + fileName + " /tmp/bulkUpdater/");
        }

        public string setRunPermissions()
        {
            return sshRunCommand("cd /tmp/bulkUpdater; sudo chmod +x " + fileName);
            
        }

        public string checkVersion()
        {
            return "";
        }

        public string runInstaller()
        {
            string command = "cd /tmp/bulkUpdater; sudo ./" + fileName + " run ";
            return sshRunCommand(command);
        }

        public void verifyInstall()
        {

        }


        public void Install()
        {

        }
    }
}
