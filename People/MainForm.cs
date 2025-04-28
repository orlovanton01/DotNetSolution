using People;
using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace XmlDataApp
{
    public partial class MainForm : Form
    {
        private DataTable dataTable;

        public MainForm()
        {
            InitializeComponent();
            InitializeDataTable();
        }

        private void InitializeDataTable()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("Возраст", typeof(int));
            dataGridView1.DataSource = dataTable;
        }

        private void btnLoadFromXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Выберите XML файл"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dataTable.Rows.Clear();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog.FileName);

                    XmlNodeList personNodes = xmlDoc.SelectNodes("//Person");
                    foreach (XmlNode node in personNodes)
                    {
                        string name = node.SelectSingleNode("Name").InnerText;
                        int age = int.Parse(node.SelectSingleNode("Age").InnerText);
                        dataTable.Rows.Add(name, age);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке XML: {ex.Message}");
                }
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                dataTable.Rows.Add(addForm.PersonName, addForm.PersonAge);
            }
        }
    }
}