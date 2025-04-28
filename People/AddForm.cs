using System;
using System.Windows.Forms;

namespace People
{
    public partial class AddForm : Form
    {
        public string PersonName { get; private set; }
        public int PersonAge { get; private set; }

        public AddForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите имя");
                return;
            }

            if (!int.TryParse(textBox2.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Введите корректный возраст");
                return;
            }

            PersonName = textBox1.Text;
            PersonAge = age;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
