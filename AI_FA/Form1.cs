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
namespace AI_FA
{
    public partial class Form1 : Form
    {
        public Bitmap ImageExempl;
        public Bitmap BlackWhite;
        Color[,] color;
        public static int SectorsCount;
        Class2 BlackBox;
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
            catch {
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
                textBox1.Text = sectorAngle.ToString("F4");
                richTextBox1.Text += "Count of black points: " + CountOfBlackPoints.ToString() + "\n";
                using (Graphics g = Graphics.FromImage(BlackWhite))
                {                   
                    Pen pen = new Pen(Color.Red, 1);
                    g.DrawRectangle(pen, mini, minj, maxi1-mini, maxj-minj);                    
                    pen.Dispose();
                }
               

                pictureBox1.Image = BlackWhite;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                BlackBox = new Class2();
                Rectangle part = new Rectangle(mini, minj, maxi1 - mini, maxj - minj);
                Bitmap clonedBitmap = ClonePartOfBitmap(ImageExempl, part);
                BlackBox.Add(clonedBitmap);
               


               
            }
        }
        public Bitmap ClonePartOfBitmap(Bitmap sourceBitmap, Rectangle section)
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
                FedoryshinAndriyS1[i] = FedoryshinAndriyS1[i] / BlackBox.CountOfBlackPoints;
            }
            richTextBox1.Text = richTextBox1.Text + "FedoryshinAndriyS1" + "\n" ;
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
    }
}