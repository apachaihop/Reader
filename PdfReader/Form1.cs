using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Windows.Forms;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace Reader
{
    public partial class Form1 : Form
    {
        List<String> files = new List<String>();
        int page = 1;
        string currentFile = "";
        int pageCount = 0;
        int w = 0;
        Font curentFont;

        public Form1()
        {
            InitializeComponent();
            UpdateButtonSizeAndPosition();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            curentFont = new Font(richTextBox1.Font.FontFamily, 12, FontStyle.Regular);
        }

        private void button_file_select(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            string filePath = btn.Name;
            currentFile = filePath;
            pageCount = GetPagesCount(currentFile);
            if (page > pageCount)
            {
                page = pageCount;
                textBox1.Text = page.ToString();
            }

            richTextBox1.Text = GetTextFromPage(currentFile, page);
            label2.Text = "/";
            label2.Text += pageCount.ToString();
        }

        private void addFile(String fileName, String filePath)
        {
            if (!files.Contains(filePath))
            {
                System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
                btn.Top = 15 + w;
                w += 25;
                btn.Left = 10;
                btn.Text = fileName;
                btn.Name = filePath;
                btn.Click += new EventHandler(this.button_file_select);
                panel1.Controls.Add(btn);
                files.Add(filePath);
            }

            ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    this.addFile(openFileDialog.SafeFileName, filePath);
                    currentFile = filePath;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Что-то пошло не так", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            pageCount = GetPagesCount(currentFile);
            page = 1;
            textBox1.Text = page.ToString();

            richTextBox1.Text = GetTextFromPage(currentFile, page);
            label2.Text = "/";
            label2.Text += pageCount.ToString();
            textBox1.ReadOnly = false;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (currentFile == "")
            {
                return;
            }
            else
            {
                if (curentFont.Size - 1 < 1)
                {
                    return;
                }
                else
                {
                    curentFont = new Font(richTextBox1.Font.FontFamily, curentFont.Size - 1, FontStyle.Regular);
                    richTextBox1.Font = curentFont;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentFile == "")
            {
                return;
            }

            {
                try
                {
                    curentFont = new Font(richTextBox1.Font.FontFamily, curentFont.Size + 1, FontStyle.Regular);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Что-то пошло не так", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                richTextBox1.Font = curentFont;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (page < 2)
            {
                return;
            }

            richTextBox1.Text = GetTextFromPage(currentFile, --page);
            textBox1.Text = page.ToString();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (page + 1 > pageCount)
            {
                return;
            }

            richTextBox1.Text = GetTextFromPage(currentFile, ++page);
            textBox1.Text = page.ToString();
        }


        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            pageCount = GetPagesCount(currentFile);
            if (page > pageCount)
            {
                page = pageCount;
                textBox1.Text = page.ToString();
            }

            richTextBox1.Text = GetTextFromPage(currentFile, page);

            label2.Text = '/' + pageCount.ToString();
        }


        private int CalculateCharsPerLine(RichTextBox richTextBox)
        {
            using (Graphics g = richTextBox.CreateGraphics())
            {
                float charSize = richTextBox.Font.Size;
                int charsPerLine = (int)(richTextBox.Width / (charSize / 1.33));
                return charsPerLine;
            }
        }

        private long GetFileLenght(string filePath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            long size = file.Length;
            return size;
        }

        private string GetTextFromPage(string filePath, int targetPageNumber)
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);

                int charSizeInBytes, pageSizeInBytes;
                GetBytesPerPage(richTextBox1, out charSizeInBytes, out pageSizeInBytes);

                int offset = (int)((targetPageNumber - 1) * pageSizeInBytes);

                sr.BaseStream.Seek(offset, SeekOrigin.Begin);


                char[] buffer = new char[pageSizeInBytes / 2];

                int bytesRead = sr.ReadBlock(buffer, 0, buffer.Length);

                StringBuilder pageText = new StringBuilder();

                pageText.Append(buffer, 0, bytesRead);
                return pageText.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Что-то пошло не так", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void GetBytesPerPage(RichTextBox richTextBox, out int charSizeInBytes, out int pageSizeInBytes)
        {
            int charsPerLine = CalculateCharsPerLine(richTextBox);
            int linesPerPage = CalculateLinesPerPage(richTextBox);

            charSizeInBytes = Encoding.UTF8.GetByteCount("а");
            pageSizeInBytes = charsPerLine * linesPerPage * charSizeInBytes;
        }

        private int GetPagesCount(string filePath)
        {
            try
            {
                int charSizeInBytes, pageSizeInBytes;
                GetBytesPerPage(richTextBox1, out charSizeInBytes, out pageSizeInBytes);
                return (int)(GetFileLenght(filePath) / pageSizeInBytes) + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Максимальный размер шрифта.", "Что-то пошло не так", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                int charSizeInBytes, pageSizeInBytes;
                GetBytesPerPage(richTextBox1, out charSizeInBytes, out pageSizeInBytes);
                curentFont = new Font(richTextBox1.Font.FontFamily, curentFont.Size - 1, FontStyle.Regular);
                richTextBox1.Font = curentFont;
                return (int)(GetFileLenght(filePath) / (pageSizeInBytes + 1)) + 1;
            }
        }

        private int CalculateLinesPerPage(RichTextBox richTextBox)
        {
            using (Graphics g = richTextBox.CreateGraphics())
            {
                SizeF charSize = g.MeasureString("а", richTextBox.Font);
                int linesPerPage = (int)(richTextBox.Height / charSize.Height);
                return linesPerPage;
            }
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            UpdateButtonSizeAndPosition();
            if (richTextBox1.Text != "")
            {
                pageCount = GetPagesCount(currentFile);
                richTextBox1.Text = GetTextFromPage(currentFile, page);

                label2.Text = '/' + pageCount.ToString();
            }
            else
            {
                return;
            }
        }

        private void UpdateButtonSizeAndPosition()
        {
            int buttonWidth = ClientSize.Width / 10;
            int buttonHeight = 25;
            int margin = 5;

            button1.Size = new Size(buttonWidth, buttonHeight);
            button1.Location = new Point(0, 0);

            button2.Size = new Size(buttonWidth, buttonHeight);
            button2.Location = new Point(button1.Right + margin, 0);

            button3.Size = new Size(buttonWidth, buttonHeight);
            button3.Location = new Point(button2.Right + margin, 0);

            button4.Size = new Size(buttonWidth, buttonHeight);
            button4.Location = new Point(button3.Right + margin, 0);

            textBox1.Location = new Point(button4.Right + margin, 4);
            label2.Location = new Point(textBox1.Right + 2, 4);

            button5.Size = new Size(buttonWidth, buttonHeight);
            button5.Location = new Point(label2.Right + margin + 20, 0);

            richTextBox1.Location = new Point(0, button1.Bottom + margin);
            richTextBox1.Size = new Size(ClientSize.Width, ClientSize.Height - richTextBox1.Top);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                return;
            }

            try
            {
                Int32.TryParse(textBox1.Text, out page);
                if (page > pageCount)
                {
                    page = pageCount;
                    textBox1.Text = pageCount.ToString();
                    throw new Exception("Нет такой страницы");
                }

                richTextBox1.Text = GetTextFromPage(currentFile, page);
            }
            catch (Exception ex)
            {
                textBox1.Text = page.ToString();
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
    }
}