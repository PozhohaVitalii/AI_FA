using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;

namespace AI_FA
{
    public partial class Form1 : Form
    {
        public Bitmap ImageExempl;
        public Bitmap BlackWhite;
        Color[,] color;
        public static int SectorsCount;
        Class2 BlackBox;
        public int NumbOFclasses = 0;
        public static List<Class2> ClassesBlackBox = new List<Class2>();
        List<Form> classForm = new List<Form>();
        List<Button> buttons = new List<Button>();

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    ImageExempl = new Bitmap(pictureBox1.Image);
                    BlackWhite = new Bitmap(pictureBox1.Image);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int minj = 0;
            int mini = 0;
            int maxj = 0;
            int maxi1 = 0;

            try
            {
                SectorsCount = int.Parse(textBox1.Text);
            }
            catch
            {
                textBox1.Text = "input count first";
            }
            if (SectorsCount != 0)
            {
                double sectorAngle = (double)90.0 / SectorsCount;
                int CountOfBlackPoints = 0;
                color = GetPixelArray(ImageExempl);
                mini = color.GetLength(0);
                minj = color.GetLength(1);

                for (int i = 0; i < color.GetLength(0); i++)
                {
                    for (int j = 0; j < color.GetLength(1); j++)
                    {
                        int gray = (int)(0.299 * color[i, j].R + 0.587 * color[i, j].G + 0.114 * color[i, j].B);
                        if (gray > 128)
                        {
                            gray = 255;
                        }
                        else if (gray <= 128)
                        {
                            gray = 1;
                            CountOfBlackPoints++;
                            mini = mini < i ? mini : i;
                            minj = minj < j ? minj : j;
                            maxi1 = maxi1 > i ? maxi1 : i;
                            maxj = maxj > j ? maxj : j;

                        }
                        double hipotenusa = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(color.GetLength(1) - j, 2));
                        double angle = Math.Asin((double)i / hipotenusa) * (180.0 / Math.PI);
                        int n = 0;
                        do
                        {

                            n++;
                        }
                        while (angle > n * sectorAngle);
                        if (gray == 255)
                        {

                            int m = 7;
                            if (SectorsCount < 8) m = 17;
                            gray = gray - n * m;
                        }

                        BlackWhite.SetPixel(i, j, Color.FromArgb(255, gray, gray, gray));
                    }
                }
                // textBox1.Text = sectorAngle.ToString("F4");
                richTextBox1.Text += "Count of black points: " + CountOfBlackPoints.ToString() + "\n";
                using (Graphics g = Graphics.FromImage(BlackWhite))
                {
                    Pen pen = new Pen(Color.Red, 1);
                    g.DrawRectangle(pen, mini, minj, maxi1 - mini, maxj - minj);
                    pen.Dispose();
                }


                pictureBox1.Image = BlackWhite;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                BlackBox = new Class2();
                Rectangle part = new Rectangle (mini, minj , maxi1 - mini , maxj - minj );
                Bitmap clonedBitmap = ClonePartOfBitmap(ImageExempl, part);
                BlackBox.Add(clonedBitmap);




            }
        }
        public static Bitmap ClonePartOfBitmap(Bitmap sourceBitmap, Rectangle section)
        {
            // Clone the specified section of the bitmap.
            return sourceBitmap.Clone(section, sourceBitmap.PixelFormat);
        }

        public static Color[,] GetPixelArray(Bitmap imageExempl)
        {
            // Створення двовимірного масиву для пікселів
            int width = imageExempl.Width;
            int height = imageExempl.Height;
            Color[,] pixelArray = new Color[width, height];

            // Блокування бітів зображення
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = imageExempl.LockBits(rect, ImageLockMode.ReadOnly, imageExempl.PixelFormat);

            try
            {
                // Визначення кількості байтів на один рядок
                int bytesPerPixel = Image.GetPixelFormatSize(imageExempl.PixelFormat) / 8;
                int byteCount = bitmapData.Stride * height;
                byte[] pixels = new byte[byteCount];

                // Копіювання піксельних даних у масив
                Marshal.Copy(bitmapData.Scan0, pixels, 0, byteCount);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Обчислення індексу в масиві для конкретного пікселя
                        int index = (y * bitmapData.Stride) + (x * bytesPerPixel);

                        // Зчитування компонентів кольору
                        byte blue = pixels[index];
                        byte green = pixels[index + 1];
                        byte red = pixels[index + 2];

                        // Якщо формат зображення підтримує альфа-канал
                        byte alpha = bytesPerPixel == 4 ? pixels[index + 3] : (byte)255;

                        // Додавання кольору до двовимірного масиву
                        pixelArray[x, y] = Color.FromArgb(alpha, red, green, blue);
                    }
                }
            }
            finally
            {
                // Розблокування бітів зображення
                imageExempl.UnlockBits(bitmapData);
            }

            return pixelArray;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BlackBox.calcFirst();
            int[,] SignsVector = BlackBox.getSector();
            double[] FedoryshinAndriyM1 = new double[SignsVector.GetLength(1)];
            double[] FedoryshinAndriyS1 = new double[SignsVector.GetLength(1)];
            richTextBox1.Text = richTextBox1.Text + "\n";
            richTextBox1.Text = richTextBox1.Text + "Absolute vector" + "\n";

            for (int i = 0; i < SignsVector.GetLength(0); i++)
            {

                for (int j = 0; j < SignsVector.GetLength(1); j++)
                {
                    FedoryshinAndriyM1[j] = SignsVector[i, j];
                    FedoryshinAndriyS1[j] = SignsVector[i, j];
                    richTextBox1.Text = richTextBox1.Text + " " + SignsVector[i, j];
                }
                richTextBox1.Text = richTextBox1.Text + "\n";
            }
            double mx = FedoryshinAndriyM1.Max();
            for (int i = 0; i < FedoryshinAndriyM1.Length; i++)
            {

                FedoryshinAndriyM1[i] = FedoryshinAndriyM1[i] / mx;
                FedoryshinAndriyS1[i] = FedoryshinAndriyS1[i] / BlackBox.CountOfBlackPoints[0];
            }
            richTextBox1.Text = richTextBox1.Text + "FedoryshinAndriyS1" + "\n";
            for (int i = 0; i < FedoryshinAndriyS1.Length; i++)
            {
                richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyS1[i].ToString("F2") + "  ";
            }
            richTextBox1.Text = richTextBox1.Text + "FedoryshinAndriyM1" + "\n";
            for (int i = 0; i < FedoryshinAndriyM1.Length; i++)
            {
                richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyM1[i].ToString("F2") + "  ";
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SectorsCount = int.Parse(textBox1.Text);
            }
            catch (Exception) { }

            NumbOFclasses++;
            ClassesBlackBox.Add(new Class2());
            classForm.Add(new Form2(NumbOFclasses));
            classForm[classForm.Count - 1].ShowDialog();

            for (int i = 0; i < ClassesBlackBox.Count; i++)
            {
                buttons.Add(new Button());
                buttons[i].Text =  (i).ToString();
                buttons[i].Size = new System.Drawing.Size(50, 30);
                buttons[i].Location = new System.Drawing.Point(0 + i * 58, 510);

                if (i != ClassesBlackBox.Count - 1)
                {
                    this.Controls.Add(buttons[i]);
                    continue;
                }

                buttons[i].Click += (sender, e) => Button_Click(sender, e, i);

                // Add the button to the form's controls
                this.Controls.Add(buttons[i]);
            }
        }
        private void Button_Click(object sender, EventArgs e, int index)
        {
            classForm[index - 1].ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BlackBox.calcFirst();
            int[,] SignsVector = BlackBox.getSector();
            double[] FedoryshinAndriyM1 = new double[SignsVector.GetLength(1)];
            double[] FedoryshinAndriyS1 = new double[SignsVector.GetLength(1)];
            double[] AbsoluteVector = new double[SignsVector.GetLength(1)];

            for (int i = 0; i < SignsVector.GetLength(0); i++)
            {
                richTextBox1.Text = richTextBox1.Text + "\n";
                for (int j = 0; j < SignsVector.GetLength(1); j++)
                {
                    FedoryshinAndriyM1[j] = SignsVector[i, j];
                    FedoryshinAndriyS1[j] = SignsVector[i, j];
                    AbsoluteVector[j] = SignsVector[i, j];
                    richTextBox1.Text = richTextBox1.Text + " " + SignsVector[i, j];
                }
            }
            double mx = FedoryshinAndriyM1.Max();
            for (int i = 0; i < FedoryshinAndriyM1.Length; i++)
            {

                FedoryshinAndriyM1[i] = FedoryshinAndriyM1[i] / mx;
                FedoryshinAndriyS1[i] = FedoryshinAndriyS1[i] / BlackBox.CountOfBlackPoints[0];
            }
            richTextBox1.Text = richTextBox1.Text + "\n";
            for (int i = 0; i < FedoryshinAndriyS1.Length; i++)
            {
                richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyS1[i].ToString("F2") + "  ";
            }
            richTextBox1.Text = richTextBox1.Text + "\n";
            for (int i = 0; i < FedoryshinAndriyM1.Length; i++)
            {
                richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyM1[i].ToString("F2") + "  ";
            }


            double[] LowLimS;
            double[] HighLimS;

            bool[] classEntity = new bool[ClassesBlackBox.Count];
            for (int t = 0; t < ClassesBlackBox.Count; t++)
            {
                ClassesBlackBox[t].calcFirst();

                classEntity[t] = true;
                LowLimS = ClassesBlackBox[t].getLowLimit();
                HighLimS = ClassesBlackBox[t].getHighLimit();
                richTextBox1.Text = richTextBox1.Text + "\n";
                for (int j = 0; j < LowLimS.Length; j++)
                {
                    richTextBox1.Text = richTextBox1.Text + LowLimS[j].ToString("F3") + "  ";
                }
                richTextBox1.Text = richTextBox1.Text + "\n";
                richTextBox1.Text = richTextBox1.Text + "\n";

                for (int j = 0; j < HighLimS.Length; j++)
                {
                    richTextBox1.Text = richTextBox1.Text + HighLimS[j].ToString("F3") + "  ";
                }
                richTextBox1.Text = richTextBox1.Text + "\n";



                richTextBox1.Text = richTextBox1.Text + "\n\n";




                for (int i = 0; i < LowLimS.Length; i++)
                {
                    if (LowLimS[i] > FedoryshinAndriyS1[i]) classEntity[t] = false;
                    if (HighLimS[i] < FedoryshinAndriyS1[i]) classEntity[t] = false;
                }

                if (classEntity[t])
                {
                    richTextBox1.Text = richTextBox1.Text + "\n" + " Object recognized like:  class" + t.ToString();
                    richTextBox1.Text = richTextBox1.Text + "\n";

                    for (int j = 0; j < HighLimS.Length; j++)
                    {
                        richTextBox1.Text = richTextBox1.Text + HighLimS[j].ToString("F3") + "  ";
                    }
                    richTextBox1.Text = richTextBox1.Text + "\n";

                    for (int j = 0; j < LowLimS.Length; j++)
                    {
                        richTextBox1.Text = richTextBox1.Text + LowLimS[j].ToString("F3") + "  ";
                    }
                    richTextBox1.Text = richTextBox1.Text + "\n";

                }else
                {
                    richTextBox1.Text = richTextBox1.Text + "Class  " + t + " are not recognized " + "\n"+"High lim" + "\n";
                    for (int j = 0; j < HighLimS.Length; j++)
                    {
                        richTextBox1.Text = richTextBox1.Text + HighLimS[j].ToString("F3") + "  ";
                    }
                    richTextBox1.Text = richTextBox1.Text + "\n"+ " Object "+"\n";
                    for (int j = 0; j < FedoryshinAndriyS1.Length; j++)
                    {
                        richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyS1[j].ToString("F3") + "  ";
                    }
                    richTextBox1.Text = richTextBox1.Text + "\n" + " Low lim " + "\n";
                    for (int j = 0; j < LowLimS.Length; j++)
                    {
                        richTextBox1.Text = richTextBox1.Text + LowLimS[j].ToString("F3") + "  ";
                    }
                    richTextBox1.Text = richTextBox1.Text + "\n";

                }


            }
/*
            for (int i = 0; i < classEntity.Length; i++)
            {
                if (classEntity[i])
                {
                    richTextBox1.Text = richTextBox1.Text + "\n" + " Object recognized like:  class" + i.ToString();
                    richTextBox1.Text = richTextBox1.Text + "\n";


                }
                else
                {
                    richTextBox1.Text = richTextBox1.Text + "\n" + " Class " + i.ToString() + " are not recognized !!!";
                    richTextBox1.Text = richTextBox1.Text + "\n";
                }

                richTextBox1.Text = richTextBox1.Text + "\n";

                richTextBox1.Text = richTextBox1.Text + "\n";
                for (int j = 0; j < FedoryshinAndriyS1.Length; j++)
                {
                    richTextBox1.Text = richTextBox1.Text + FedoryshinAndriyS1[j].ToString("F3") + "  ";
                }
                richTextBox1.Text = richTextBox1.Text + "\n";

            }*/

        }

        private void textBox1_MouseHover(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                SectorsCount = int.Parse(textBox1.Text);
            }
            catch (Exception) { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}