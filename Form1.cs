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
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace JSON2BINARY
{
    public partial class Form1 : Form
    {
        public List<JsonFileInfo> JsonFileInfoList;

        private string mExportPath = @"C:\Users\gy\Desktop\";

        public Form1()
        {
            this.JsonFileInfoList = new List<JsonFileInfo>();
            AssemblyManager.Instance.Initialize();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            var rFolderDialog = new FolderBrowserDialog();
            rFolderDialog.SelectedPath = System.Windows.Forms.Application.StartupPath;

            if(rFolderDialog.ShowDialog() == DialogResult.OK)
            {
                this.JsonFileInfoList.Clear();
                this.checkedListBox1.Items.Clear();

                var rPath = rFolderDialog.SelectedPath;
                var rJsonFiles = Directory.GetFiles(rPath,"*.json");

                for (int i = 0; i < rJsonFiles.Length; i++)
                {
                    var rJsonFileInfo = new JsonFileInfo();
                    var rFileNameInfos = rJsonFiles[i].Split('\\');

                    if (rFileNameInfos.Length == 0) continue;

                    rJsonFileInfo.FileName = rFileNameInfos[rFileNameInfos.Length - 1];
                    rJsonFileInfo.FilePath = rJsonFiles[i];
                    this.JsonFileInfoList.Add(rJsonFileInfo);
                    this.checkedListBox1.Items.Add(rJsonFileInfo.FileName, false);
                }

                this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemCheck);
            }
            rFolderDialog.Dispose();
        }

        private void OnItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.JsonFileInfoList[e.Index].IsSelect = e.CurrentValue == CheckState.Unchecked;
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("没有文件可选择");
                return;
            }

            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemCheckState(i, CheckState.Checked);

                this.JsonFileInfoList[i].IsSelect = true;
            }
        }

        private void BtnCancelSelectAll_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("没有文件可选择");
                return;
            }

            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);

                this.JsonFileInfoList[i].IsSelect = false;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("没有文件可导出");
                return;
            }

            var rFolderDialog = new FolderBrowserDialog();
            rFolderDialog.SelectedPath = this.mExportPath;

            if(rFolderDialog.ShowDialog() == DialogResult.OK)
            {
                var bFinishSerialize = false;
                this.mExportPath = rFolderDialog.SelectedPath;
                for (int i = 0; i < this.JsonFileInfoList.Count; i++)
                {
                    if (!this.JsonFileInfoList[i].IsSelect) continue;

                    var rFileNames = this.JsonFileInfoList[i].FileName.Split('.');
                    var rFileName = rFileNames[0];

                    if (string.IsNullOrEmpty(rFileName)) continue;

                    var rJsonFilePath = this.JsonFileInfoList[i].FilePath;
                    var rBytes = this.DeserializeJson(rJsonFilePath, rFileName);

                    var rOutputFileName = this.mExportPath + "/" + rFileName + ".bytes";
                    var rOutputFile = File.OpenWrite(rOutputFileName);
                    rOutputFile.Write(rBytes.ToArray(), 0, rBytes.Count);
                    rOutputFile.Close();
                    bFinishSerialize = true;
                }

                if (bFinishSerialize)
                {
                    MessageBox.Show("序列化完成");
                }
            }

            rFolderDialog.Dispose();
        }

        private List<byte> DeserializeJson(string rJsonFilePath, string rTypeName)
        {
            var rBytes = new List<byte>();
            if (File.Exists(rJsonFilePath))
            {
                var rJsonText = File.ReadAllText(rJsonFilePath);
                var rType = AssemblyManager.Instance.GetContainType(rTypeName);

                if(rType == null)
                {
                    return rBytes;
                }

                var rGenericType = typeof(List<>).MakeGenericType(new Type[] { rType});
                var rJsonDict = JsonConvert.DeserializeObject(rJsonText, rGenericType);

                var rBF = new BinaryFormatter();
                var rMemoryStream = new MemoryStream();
                rBF.Serialize(rMemoryStream, rJsonDict);
                rBytes.AddRange(rMemoryStream.ToArray());
            }

            return rBytes;
        }

    }

    [Serializable]
    public class TestJson
    {
        public List<Test> ValueList;
    }

    [Serializable]
    public class Test
    {
        public int ID;

        public int Value;

        public string Name;
    }


}
