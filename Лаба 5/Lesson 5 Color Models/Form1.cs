using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Lesson_5_Color_Models
{
    public partial class Form1 : Form
    {
        // pictureBox1
        BitmapData bitmap;
        Bitmap bit;
        Graphics g;
        int totalSize;
        byte[] imageBytes;

        public Form1()
        {
            InitializeComponent();
            trackbars.Enabled = false;
            bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bit;
           // g = Graphics.FromImage(pictureBox1.Image);
           //g.Clear(Color.White);
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e) // Загрузка изображения
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(ofd.FileName);
            }
        }

        private void getBitmapData(PictureBox CurrentPicture) 
        {
            Rectangle bounds = new Rectangle(0, 0, CurrentPicture.Image.Width, CurrentPicture.Image.Height);
            bitmap = ((Bitmap)CurrentPicture.Image).LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            int RowSizeBytes = bitmap.Stride;
            totalSize = RowSizeBytes * bitmap.Height;
            imageBytes = new byte[totalSize];
            Marshal.Copy(bitmap.Scan0, imageBytes, 0, totalSize);
        }

        private void returnBitmapData(PictureBox CurrentPicture)
        {
            Marshal.Copy(imageBytes, 0, bitmap.Scan0, totalSize);
            ((Bitmap)CurrentPicture.Image).UnlockBits(bitmap);
            imageBytes = null;
            bitmap = null;
        }

        private void чернобелоеToolStripMenuItem_Click(object sender, EventArgs e) //Черно-белое
        {
            trackbars.Enabled = false;
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Нет изображения");
                return;
            }

            // pictureBox2
            byte[] grayBytes;

            // pictureBox3
            byte[] grayBytes2;

            // pictureBox4
            byte[] grayDifference;


            getBitmapData(pictureBox1);

            grayBytes = new byte[totalSize];
            grayBytes2 = new byte[totalSize];
            grayDifference = new byte[totalSize];

            imageBytes.CopyTo(grayBytes, 0);
            imageBytes.CopyTo(grayBytes2, 0);

            // обработка исходного изображения
            int k = 0;
            for (int i = 0; i < pictureBox1.Image.Height; ++i)
                for (int j = 0; j < pictureBox1.Image.Width; ++j)
                {
                    k = i * bitmap.Stride + j * 4;
                    // серый цвет
                    byte c = (byte)(0.0722 * grayBytes[k] + 0.7152 * grayBytes[k + 1] + 0.2126 * grayBytes[k + 2]);
                    grayBytes[k] = c;
                    grayBytes[k + 1] = c;
                    grayBytes[k + 2] = c;
                    // другой серый цвет
                    c = (byte)(0.33 * grayBytes2[k] + 0.33 * grayBytes2[k + 1] + 0.33 * grayBytes2[k + 2]);
                    grayBytes2[k] = c;
                    grayBytes2[k + 1] = c;
                    grayBytes2[k + 2] = c;
                    // Разница между двумя серыми цветами
                    grayDifference[k] = (byte)Math.Abs(grayBytes[k] - grayBytes2[k]);
                    grayDifference[k + 1] = (byte)Math.Abs(grayBytes[k + 1] - grayBytes2[k + 1]);
                    grayDifference[k + 2] = (byte)Math.Abs(grayBytes[k + 2] - grayBytes2[k + 2]);
                }

     
            returnBitmapData(pictureBox1);

            DisplayImages(grayBytes, grayBytes2, grayDifference); // ссылка на создание
        }

        void DisplayImages(byte[] im1, byte[] im2, byte[] im3) // создание изображений на всех полях
        {
            Rectangle bounds;
            // создание 1ого варианта серого цвета в поле 2 
            pictureBox2.Image = new Bitmap(pictureBox1.Image);
            bounds = new Rectangle(0,0,pictureBox2.Image.Width, pictureBox2.Image.Height);
            BitmapData GrayBm = ((Bitmap)pictureBox2.Image).LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
        
            Marshal.Copy(im1, 0, GrayBm.Scan0, totalSize);
            ((Bitmap)pictureBox2.Image).UnlockBits(GrayBm);
            pictureBox2.Refresh();

            // создание 2ого варианта серого цветв в пол 3
            pictureBox3.Image = new Bitmap(pictureBox1.Image);
            bounds = new Rectangle(0,0,pictureBox3.Image.Width,pictureBox3.Image.Height);
            BitmapData GrayBm2 = ((Bitmap)pictureBox3.Image).LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            Marshal.Copy(im2, 0, GrayBm2.Scan0, totalSize);
            ((Bitmap)pictureBox3.Image).UnlockBits(GrayBm2);
            pictureBox3.Refresh();

            // создание разницы серых цветов в поле 4
            pictureBox4.Image = new Bitmap(pictureBox1.Image);
            bounds = new Rectangle(0,0,pictureBox4.Image.Width,pictureBox4.Image.Height);
            BitmapData differenceBitmapData = ((Bitmap)pictureBox4.Image).LockBits(bounds,ImageLockMode.ReadWrite,PixelFormat.Format32bppRgb);

            Marshal.Copy(im3, 0, differenceBitmapData.Scan0, totalSize);
            ((Bitmap)pictureBox4.Image).UnlockBits(differenceBitmapData);
            pictureBox4.Refresh();
        }

        void DisplayImages(byte[] im1) //создание изображения для HSV
        {
            Rectangle bounds;
            // создание картинки на pictureBox2
            pictureBox2.Image = new Bitmap(pictureBox1.Image);
            bounds = new Rectangle(0,0,pictureBox2.Image.Width,pictureBox2.Image.Height);
            BitmapData GrayBm = ((Bitmap)pictureBox2.Image).LockBits(bounds, ImageLockMode.ReadWrite,PixelFormat.Format32bppRgb);

            Marshal.Copy(im1, 0, GrayBm.Scan0, totalSize);
            ((Bitmap)pictureBox2.Image).UnlockBits(GrayBm);
            pictureBox2.Refresh();
        }

        private void разложитьПоКаналамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackbars.Enabled = false;
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Нет изображения");
                return;
            }

            // pictureBox2
            byte[] red;

            // pictureBox3
            byte[] green;

            // pictureBox4
            byte[] blue;


            getBitmapData(pictureBox1);

            red = new byte[totalSize];
            green = new byte[totalSize];
            blue = new byte[totalSize];

            imageBytes.CopyTo(red, 0);
            imageBytes.CopyTo(green, 0);
            imageBytes.CopyTo(blue, 0);

            // Обработка исходного изображения
            int k = 0;
            for (int i = 0; i < pictureBox1.Image.Height; ++i)
                for (int j = 0; j < pictureBox1.Image.Width; ++j)
                {
                    k = i * bitmap.Stride + j * 4;
                    // красный канал
                    red[k] = 0;
                    red[k + 1] = 0;
                    // зеленый канал
                    green[k] = 0;
                    green[k + 2] = 0;
                    // синий канал
                    blue[k + 1] = 0;
                    blue[k + 2] = 0;
                }

   
            returnBitmapData(pictureBox1);

            DisplayImages(red, green, blue); // ссылка на создание 



        }

        public static Bitmap CalculateBarChart(Bitmap image, string name_ch)
        {


            int width = 700;
            int height = 500;

            Bitmap hist = new Bitmap(width, height);

            // создаем массивы, в котором будут содержаться количества повторений для каждого из значений каналов.
            // индекс соответствует значению канала
            int[] arr_col = new int[256];

            System.Drawing.Color color;
            // собираем статистику для изображения
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                {
                    color = image.GetPixel(i, j);
                    if (name_ch == "Red")
                    {
                        arr_col[color.R]++;
                    }
                    else if (name_ch == "Green")
                    {
                        arr_col[color.G]++;
                    }
                    else
                    {
                        arr_col[color.B]++;
                    }
                }
            // находим самый высокий столбец, чтобы корректно масштабировать гистограмму по высоте
            int max = 0;
            for (int i = 0; i < 256; ++i)
            {
                if (arr_col[i] > max)
                    max = arr_col[i];
            }

            // определяем коэффициент масштабирования по высоте
            double point = (double)max / height;
            // отрисовываем столбец за столбцом нашу гистограмму с учетом масштаба

            System.Drawing.Color color_hist;

            if (name_ch == "Red")
            {
                color_hist = System.Drawing.Color.Red;
            }
            else if (name_ch == "Green")
            {
                color_hist = System.Drawing.Color.Green;
            }
            else
            {
                color_hist = System.Drawing.Color.Blue;
            }

            for (int i = 0; i < width - 3; ++i)
            {
                for (int j = height - 1; j > height - arr_col[i / 3] / point; --j)
                {
                    hist.SetPixel(i, j, color_hist);
                }

            }

            return hist;
        }


        private void RGBtoHSV(byte b, byte g, byte r, ref double h, ref double s, ref double v)   //вспоманалтельная процедура для RGB - > HSV
        {
            double r1 = r / 255.0;
            double g1 = g / 255.0;
            double b1 = b / 255.0;

            double cMax = Math.Max(r1, Math.Max(g1, b1));
            double cMin = Math.Min(r1, Math.Min(g1, b1));
            double delta = cMax - cMin;

            // H
            if (delta == 0)
                h = 0;
            else if (cMax == r1)
                h = 60.0 * (((g1 - b1) / delta) % 6);
            else if (cMax == g1)
                h = 60.0 * ((b1 - r1) / delta + 2);
            else
                h = 60.0 * ((r1 - g1) / delta + 4);
            // S
            if (cMax == 0)
                s = 0;
            else
                s = delta / cMax;

            // V
            v = cMax;
        }

       

        private void HSVtoRGB(double h, double s, double v, ref double r, ref double g, ref double b) //вспомагательная процедура для HSV - > RGB
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;
            double r1 = 0, g1 = 0, b1 = 0;
            if (0 <= h && h < 60)
                { r1 = c; g1 = x; b1 = 0; }
            else if (60 <= h && h < 120)
                { r1 = x; g1 = c; b1 = 0; }
            else if (120 <= h && h < 180)
                { r1 = 0; g1 = c; b1 = x; }
            else if (180 <= h && h < 240)
                { r1 = 0; g1 = x; b1 = c; }
            else if (240 <= h && h < 300)
                { r1 = x; g1 = 0; b1 = c; }
            else if (300 <= h && h < 360)
                { r1 = c; g1 = 0; b1 = x; }

            r = (r1 + m) * 255;
            g = (g1 + m) * 255;
            b = (b1 + m) * 255;
        }

        private void преобразоватьВHSVToolStripMenuItem_Click(object sender, EventArgs e) //RGB - > HSV
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Нет изображения");
                return;
            }
            trackbars.Enabled = true;

            // pictureBox2 
            byte[] HSV;
            
      
            getBitmapData(pictureBox1);

            HSV = new byte[totalSize];
          

            imageBytes.CopyTo(HSV, 0);
            

            // Обработка исходного изображения
            int k = 0;
            for (int i = 0; i < pictureBox1.Image.Height; ++i)
                for (int j = 0; j < pictureBox1.Image.Width; ++j)
                {
                    double h = 0, s = 0, v = 0;
                    k = i * bitmap.Stride + j * 4;
                    double r = imageBytes[k + 2];
                    double g = imageBytes[k + 1];
                    double b = imageBytes[k];
                    RGBtoHSV((byte)b, (byte)g, (byte)r, ref h, ref s, ref v);
                    // получаем HSV канал 
                    HSVtoRGB(h, s, v, ref r, ref g, ref b);
                    HSV[k] = (byte)b;
                    HSV[k + 1] = (byte)g;
                    HSV[k + 2] = (byte)r; // HSV
                }

     
            returnBitmapData(pictureBox1);

            DisplayImages(HSV);  // ссылка на создание

        }

        private void ProcessHSV(object sender, MouseEventArgs e) // HVS - > RGB
        {
         
            getBitmapData(pictureBox1);

            // pictureBox4
            byte[] blue;

            blue = new byte[totalSize];

            // Обработка исходного изображения
            int k = 0;
            for (int i = 0; i < pictureBox1.Image.Height; ++i)
                for (int j = 0; j < pictureBox1.Image.Width; ++j)
                {
                    double h = 0, s = 0, v = 0;
                    k = i * bitmap.Stride + j * 4;
                    double r = imageBytes[k + 2];
                    double g = imageBytes[k + 1];
                    double b = imageBytes[k];
                    RGBtoHSV((byte)b, (byte)g, (byte)r, ref h, ref s, ref v);
                    h += hue.Value;
                    if (h < 0)
                        h += 360;
                    s += sat.Value / 100.0;
                    s = Math.Min(1, Math.Max(s, 0));
                    v += val.Value / 100.0;
                    v = Math.Min(1, Math.Max(v, 0));
                    HSVtoRGB(h, s, v, ref r, ref g, ref b);
                    blue[k + 3] = 255;
                    blue[k + 2] = (byte)r;
                    blue[k + 1] = (byte)g;
                    blue[k] = (byte)b;
                }

         
            returnBitmapData(pictureBox1);

            pictureBox1.Refresh();

            DisplayImages(blue); // ссылка на создание
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void сохранитьtoolStripMenuItem1_Click(object sender, EventArgs e) //сохранение
        {
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("Нет изображения");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "jpg";
            sfd.Filter =
            "Изображение (*.pmb;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.png;*.exif)|*.pmb;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.png;*.exif";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                bit.Save(sfd.FileName);
        }
        
        private void гистограммаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bm = new Bitmap(pictureBox1.Image);
            pictureBox2.Image = (CalculateBarChart(bm, "Red"));
            pictureBox3.Image = (CalculateBarChart(bm, "Green"));
            pictureBox4.Image = (CalculateBarChart(bm, "Blue"));
            

        }
    }
}
