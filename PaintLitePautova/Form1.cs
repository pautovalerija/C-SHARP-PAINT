using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintLitePautova
{
    public partial class Form1 : Form
    {
        Image imgOriginal;
        bool drawing;
        int historyCounter; //счетчик истории
        GraphicsPath currentPath;
        Point oldLocation;
        Pen currentPen;
        //Color historycolor;//созранение текущего цвета перед использованием ластика
        List<Image> History;//список для истории
        Graphics g;
        int figuri = 0;
        int locallX = 0;
        int locallY = 0;
        int locallXO = 0;
        int locallYO = 0;
        public Form1()
        {
            InitializeComponent();
            drawing = false;
            currentPen = new Pen(Color.Black);
            currentPen.Width = trackBar1.Value;
            colorresult.historycolor = Color.Black;
            History = new List<Image>();

            


        }

        private void ToolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (picDrawingSurface.Image != null)
            {
                var result = MessageBox.Show("Вы желаете сохранить текущий рисунок?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: SaveToolStripMenuItem_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }
            else
            {
                History.Clear();
                historyCounter = 0;
                Bitmap pic = new Bitmap(585, 355);
                pic.MakeTransparent(Color.White);
                picDrawingSurface.Image = pic;
                History.Add(new Bitmap(picDrawingSurface.Image));

            }
            Graphics graphic = Graphics.FromImage(picDrawingSurface.Image);
            graphic.Clear(Color.White);
            picDrawingSurface.Invalidate();
            imgOriginal = picDrawingSurface.Image;

        }
        
        public void PicDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if(picDrawingSurface.Image == null)
            {
                MessageBox.Show("Сначала создайте новый файл!");
                return;
            }
            if(e.Button == MouseButtons.Left)
            {
                drawing = true;
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }
            if (e.Button == MouseButtons.Right)
            {
                colorresult.historycolor = currentPen.Color;
                currentPen.Color = System.Drawing.Color.White;
                drawing = true;
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }
            
        }
        public void PicDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            if (figuri == 1)
            {
                Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                Rectangle pathRect = new Rectangle(locallX, locallY, locallXO, locallYO);
                currentPath.AddRectangle(pathRect);
                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                picDrawingSurface.Invalidate();
            }
            if (figuri == 2)
            {
                Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                g.DrawEllipse(currentPen, locallX, locallY, locallXO, locallYO);

                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                picDrawingSurface.Invalidate();
            }
            if (figuri == 3)
            {
                Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                g.DrawLine(currentPen, locallX, locallY, locallXO, locallYO);

                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                picDrawingSurface.Invalidate();
            }


            currentPen.Color = colorresult.historycolor;
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(picDrawingSurface.Image));

            if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            drawing = false;
            try
            {
                currentPath.Dispose();
            }
            catch { };
        }

    
    public void PicDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                if (figuri == 0)
                {

                    Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                    label_XY.Text = e.X.ToString() + ", " + e.Y.ToString();
                    BackColor = Color.White;
                    currentPath.AddLine(oldLocation, e.Location);
                    g.DrawPath(currentPen, currentPath);
                    oldLocation = e.Location;
                    g.Dispose();
                    picDrawingSurface.Invalidate();

                }
                else
                {
                    locallX = oldLocation.X;
                    locallY = oldLocation.Y;
                    locallXO = e.Location.X - oldLocation.X;
                    locallYO = e.Location.Y - oldLocation.Y;
                }
                
            }
            label_XY.Text = e.X.ToString() + ", " + e.Y.ToString();
            

                
        }


        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            NewToolStripMenuItem_Click(sender, e);

        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4;//по умолчанию бужет выбрано последнее решение png
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Png);
                        break;
                }
                fs.Close();
            }
            if (picDrawingSurface.Image != null)
            {
                var result=MessageBox.Show("Сохранить текущее изображение перед созданием ноговго рисунка?","Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: SaveToolStripMenuItem_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }
            imgOriginal = picDrawingSurface.Image;

        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4;//по умолчанию бужет выбрано последнее решение png
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Png);
                        break;
                }
                fs.Close();
            }
            if (picDrawingSurface.Image != null)
            {
                var result = MessageBox.Show("Сохранить текущее изображение перед созданием ноговго рисунка?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: ToolStripButton2_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }
            imgOriginal = picDrawingSurface.Image;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //jpg
            if (OP.ShowDialog() != DialogResult.Cancel) picDrawingSurface.Load(OP.FileName);
            picDrawingSurface.AutoSize = true;
            imgOriginal = picDrawingSurface.Image;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = trackBar1.Value;
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                picDrawingSurface.Image = new Bitmap(History[--historyCounter]);
            }
            else MessageBox.Show("История пуста");
        }

        private void RenoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyCounter < History.Count - 1)
            {
                picDrawingSurface.Image = new Bitmap(History[++historyCounter]);
            }
            else MessageBox.Show("История пуста");
        }

        private void SolidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;
            solidToolStripMenuItem.Checked = true;
            dotToolStripMenuItem.Checked = false;
            dashDotDotToolStripMenuItem.Checked = false;

            

        }

        private void DotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot;
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = true;
            dashDotDotToolStripMenuItem.Checked = false;

            
        }

        private void DashDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDotDot;
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;
            dashDotDotToolStripMenuItem.Checked = true;

            
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программу создала Валерия Паутова\nTARpv18\nЛабораторная работа 2");
        }

        private void RedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.Color = Color.Red;
            colorresult.historycolor = Color.Red;
        }

        private void BlackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.Color = Color.Black;
            colorresult.historycolor = Color.Black;
        }

        private void BlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.Color = Color.Blue;
            colorresult.historycolor = Color.Blue;
        }

        private void YellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.Color = Color.Yellow;
            colorresult.historycolor = Color.Yellow;
        }

        private void GreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.Color = Color.Green;
            colorresult.historycolor = Color.Green;
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //jpg
            if (OP.ShowDialog() != DialogResult.Cancel) picDrawingSurface.Load(OP.FileName);
            picDrawingSurface.AutoSize = true;
            imgOriginal = picDrawingSurface.Image;
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы действительно хотите выйти из программу ?", "Предупреждения ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();

            }
            else // Иначе 
                if (res == DialogResult.No)
            {
                
            }
            
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы действительно хотите выйти из программу ?", "Предупреждения ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();

            }
            else // Иначе 
                if (res == DialogResult.No)
            {

            }
        }

        public void ToolStripButton4_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Owner=this;
            f.ShowDialog();
        }

        private void RectangleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //currentPen.DashStyle = DashStyle.DashDotDot;
            this.solidToolStripMenuItem.Checked = false;
            this.dotToolStripMenuItem.Checked = false;
            this.dashDotDotToolStripMenuItem.Checked = false;
            this.rectangleToolStripMenuItem1.Checked = true;
            

            figuri = 1;
        }

        private void PenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///currentPen.DashStyle = DashStyle.DashDotDot;
            this.solidToolStripMenuItem.Checked = false;
            this.dotToolStripMenuItem.Checked = false;
            this.dashDotDotToolStripMenuItem.Checked = false;
            this.rectangleToolStripMenuItem1.Checked = false;


            figuri = 0;
        }

        private void CircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //currentPen.DashStyle = DashStyle.DashDotDot;
            this.solidToolStripMenuItem.Checked = false;
            this.dotToolStripMenuItem.Checked = false;
            this.dashDotDotToolStripMenuItem.Checked = false;
            this.rectangleToolStripMenuItem1.Checked = true;


            figuri = 2;
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            picDrawingSurface.Image = Zoom(imgOriginal, trackBar2.Value);


        }
        Image Zoom(Image img, int size)
        {
            Bitmap bmp = new Bitmap(img, img.Width + (img.Width * size / 10), img.Height + (img.Height * size / 10));
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            return bmp;
        }
    }
}
