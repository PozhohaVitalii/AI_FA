using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_FA
{
    public class Class2
    {
        // Одновимірний динамічний масив Bitmap
        private List<Bitmap> bitmapArray = new List<Bitmap>();
        private List<Bitmap> BlackWhite = new List<Bitmap>();
        bool toGetBlackWhite = false;
        public double[] classLowLimit;
        public double[] classHighLimit;
        public int SectorCount = 0;
        private List<bool[,]> BlackPixels = new List<bool[,]>();
        private int[,] Sectors;
        private double[,] SectorsS;
        public int[] CountOfBlackPoints;
        public void calcFirst()
        {
            if (bitmapArray.Count != 0)
            {
                List<Color[,]> img1 = new List<Color[,]>();


                foreach (Bitmap elem in bitmapArray) img1.Add(Form1.GetPixelArray(elem));


                CountOfBlackPoints = new int[img1.Count];
                for (int e = 0; e < img1.Count(); e++)
                {
                    BlackPixels.Add(new bool[img1[e].GetLength(0), img1[e].GetLength(1)]);
                    BlackWhite.Add(bitmapArray[e]);
                    for (int i = 0; i < img1[e].GetLength(0); i++)
                    {
                        for (int j = 0; j < img1[e].GetLength(1); j++)
                        {
                            int gray = (int)(0.299 * img1[e][i, j].R + 0.587 * img1[e][i, j].G + 0.114 * img1[e][i, j].B);
                            if (gray > 128)
                            {
                                gray = 255;
                                BlackPixels[e][i, j] = false;
                            }
                            else
                            {
                                gray = 1;
                                CountOfBlackPoints[e]++;
                                BlackPixels[e][i, j] = true;
                            }
                            BlackWhite[e].SetPixel(i, j, Color.FromArgb(255, gray, gray, gray));
                        }
                    }
                }
                this.toGetBlackWhite = true;
                try
                {
                    this.SectorCount = Form1.SectorsCount;
                    if (this.SectorCount == 0) { throw new Exception("ADD SECTORS"); }
                    Sectors = new int[bitmapArray.Count(), SectorCount];
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                SectorsS = new double[bitmapArray.Count(),SectorCount];
                double sectorAngle =(double) 90.0 / SectorCount;
            
                for (int e = 0; e < BlackPixels.Count(); e++)
                {
                    for (int i = 0; i < BlackPixels[e].GetLength(0); i++)
                    {
                        for (int j = 0; j < BlackPixels[e].GetLength(1); j++)
                        {
                            if (BlackPixels[e][i, j])
                            {
                                double hipotenusa = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(BlackPixels[e].GetLength(1) - j, 2));
                                double angle = Math.Asin((double)i / hipotenusa) * (180.0 / Math.PI);
                                int n = 0;
                                do
                                {
                                    n++;
                                }
                                while (angle > n * sectorAngle);
                                Sectors[e, (SectorCount - 1) - (n-1)] += 1;
                            }
                        }

                    }
                        for (int i = 0; i < SectorCount; i++)
                        {
                            SectorsS[e, i] = (double)Sectors[e, i] / (double)CountOfBlackPoints[e];
                        }

                }
                
                classHighLimit = new double[SectorCount];
                classLowLimit = new double[SectorCount];
                for (int j = 0; j < SectorsS.GetLength(1); j++)
                {
                    double min = SectorsS[0, j], max = SectorsS[0, j];

                    for (int i = 0; i < Sectors.GetLength(0); i++)
                    {
                        if (SectorsS[i, j] < min)
                        {
                            min = SectorsS[i, j];
                        }
                        if (SectorsS[i, j] > max)
                        {
                            max = SectorsS[i, j];
                        }                    
                        
                    }

                    classLowLimit[j] = min;
                    classHighLimit[j] = max;
                }
                

            }
            else throw new Exception("Add examples first");
        }
        public Bitmap getBlackWhite(int index)
        {
            if (this.toGetBlackWhite && index > 0 && index <= bitmapArray.Count)
            {
                return BlackWhite[index];

            }
            else { throw new IndexOutOfRangeException("Індекс поза межами масиву BW."); }
        }

        // Гетер і сетер для елементів масиву
        public Bitmap this[int index]
        {
            get
            {
                if (index < 0 || index >= bitmapArray.Count)
                    throw new IndexOutOfRangeException("Індекс поза межами масиву.");
                return bitmapArray[index];
            }
            set
            {
                if (index < 0 || index >= bitmapArray.Count)
                    throw new IndexOutOfRangeException("Індекс поза межами масиву.");

                // Замінюємо існуючий елемент
                bitmapArray[index] = value;
            }
        }

        // Метод для додавання нового елемента
        public void Add(Bitmap bitmap)
        {
            bitmapArray.Add(bitmap);
        }
        public Bitmap Pop()
        {
            if (bitmapArray.Count == 0)
                throw new InvalidOperationException("The array is empty.");

            // Get the last bitmap in the array
            Bitmap bitmap = bitmapArray[bitmapArray.Count - 1];

            // Remove the last bitmap from the array
            bitmapArray.RemoveAt(bitmapArray.Count - 1);

            // Return the bitmap
            return bitmap;
        }

        // Повернення кількості елементів у масиві
        public int Count
        {
            get { return bitmapArray.Count; }
        }

        public int[,] getSector() {
            return Sectors;
        }
        public double[] getLowLimit()
        {
            return classLowLimit;
        }
        public double[] getHighLimit()
        {
            return classHighLimit;
        }
        public int[] getCountOfBlackPoints()
        {
            return CountOfBlackPoints;
        }

    }
}
