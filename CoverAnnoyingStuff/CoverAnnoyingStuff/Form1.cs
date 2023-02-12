using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackboxApp
{
    public partial class Form1 : Form
    {
		const string LAST_OPENED_CONFIG_PATH_STORING_FILE = "lastOpenedConfigPathCache.txt"; // The file path where the last loaded config's file path is stored in a separate file. Created in the directory the exe is in
		int resizingMargin = 10; // The margin's size in pixels which you grab, you can resize the picturebox in question
		int minimumSize = 20; // Minimum width and height values in pixels

		public Form1()
		{
			InitializeComponent();
			samplePic.Visible = false;

			draggedPiece = samplePic;
			if (System.IO.File.Exists(LAST_OPENED_CONFIG_PATH_STORING_FILE) && System.IO.File.ReadAllLines(LAST_OPENED_CONFIG_PATH_STORING_FILE).Length != 0)
			{
				string path = System.IO.File.ReadAllLines(LAST_OPENED_CONFIG_PATH_STORING_FILE)[0];
				if (path.EndsWith(".bbcfgg")) path = path.Replace(".bbcfgg", ".bbcfg"); // @Ducttape

				if (System.IO.File.Exists(path))
				{
					LoadConfig(new FileStream(path, FileMode.Open, FileAccess.Read));
					if (pics.Count == 0) // If the recent config got corrupted somehow, or it has no picture boxes, create one so we can interact with the program still
					{
						OnPicCreateClick(null, null);
					}
				}
				else OnPicCreateClick(null, null);
			}
			else
			{
				OnPicCreateClick(null, null);
			}
		}

		private void OnPicSaveClick(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "BlackBox Config (*.bbcfg)|*.bbcfg";
			saveFileDialog1.Title = "Save a Config";
			saveFileDialog1.RestoreDirectory = true;
			saveFileDialog1.ShowDialog();

			if (saveFileDialog1.FileName != "")
			{
				System.IO.FileStream fs =
					(System.IO.FileStream)saveFileDialog1.OpenFile();
				string data = "";
				foreach(var pic in pics)
                {
					data += pic.Size.Width.ToString() + ";" + pic.Size.Height.ToString() + ";" + pic.Location.X.ToString() + ";" + pic.Location.Y.ToString() + "\n";
                }

				byte[] bArr = new byte[data.Length];
				for (int i = 0; i < data.Length; i++) bArr[i] = (byte)data[i];
				fs.Write(bArr, 0, data.Length);
				fs.Close();

				recentLoadedConfigPath = saveFileDialog1.FileName;
				FileStream fs1 = new FileStream(LAST_OPENED_CONFIG_PATH_STORING_FILE, FileMode.OpenOrCreate);
				byte[] bArr1 = new byte[recentLoadedConfigPath.Length];
				for (int i = 0; i < bArr1.Length; i++) bArr1[i] = (byte)recentLoadedConfigPath[i];
				fs1.Write(bArr1, 0, bArr1.Length);
				fs1.Close();
			}
		}

		string recentLoadedConfigPath = "";
		private void OnPicLoadClick(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "BlackBox Config (*.bbcfg)|*.bbcfg";
				openFileDialog.Title = "Load a Config";
				openFileDialog.RestoreDirectory = true;
				
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					recentLoadedConfigPath = openFileDialog.FileName;
					LoadConfig(openFileDialog.OpenFile());

					FileStream fs = new FileStream(LAST_OPENED_CONFIG_PATH_STORING_FILE, FileMode.OpenOrCreate);
					byte[] bArr = new byte[recentLoadedConfigPath.Length];
					for (int i = 0; i < bArr.Length; i++) bArr[i] = (byte)recentLoadedConfigPath[i];
					fs.Write(bArr, 0, bArr.Length);
					fs.Close();
				}
			}
		}

		private void LoadConfig(Stream fileStream)
        {
			var fileContent = string.Empty;

			using (StreamReader reader = new StreamReader(fileStream))
			{
				fileContent = reader.ReadToEnd();
			}

			foreach (var pic in pics)
			{
				Controls.Remove(pic);
			}
			pics.Clear();

			string[] lines = fileContent.Split('\n');
			foreach (string line in lines)
			{
				string[] data = line.Split(';');
				if (data.Length < 4) continue;
				OnPicCreateClick(null, null);
				SizeablePictureBox pic = pics[pics.Count - 1];
				pic.Size = new Size(int.Parse(data[0]), int.Parse(data[1]));
				pic.Location = new Point(int.Parse(data[2]), int.Parse(data[3]));
			}
		}

		List<SizeablePictureBox> pics = new List<SizeablePictureBox>();
		SizeablePictureBox lastClickedPic = new SizeablePictureBox();
		private void OnPicCreateClick(object sender, EventArgs e)
		{
			SizeablePictureBox newPic = new SizeablePictureBox();
			newPic.BackColor = Color.Black;
			newPic.Location = lastClickedPic.Location;
			newPic.Size = lastClickedPic.Size;
			newPic.ContextMenu = new ContextMenu(new MenuItem[6] { 
				new MenuItem("Save", new EventHandler(OnPicSaveClick)),
				new MenuItem("Load", OnPicLoadClick),
				new MenuItem("Create New", OnPicCreateClick),
				new MenuItem("Remove This", OnPicCloseClick),
				new MenuItem("-----------"),
				new MenuItem("Close App", OnPicCloseAppClick) });
			newPic.MouseDown += piece_MouseDown;
			newPic.MouseMove += piece_MouseMove;
			newPic.MouseUp += piece_MouseUp;

			Controls.Add(newPic);
			pics.Add(newPic);
		}

		private void OnPicCloseClick(object sender, EventArgs e)
        {
			if(pics.Count == 1)
            {
				MessageBox.Show("You can't delete this box, as it's the only one left in the configuration.");
				return;
            }

			Controls.Remove(lastClickedPic);
			pics.Remove(lastClickedPic);
		}

		private void OnPicCloseAppClick(object sender, EventArgs e)
        {
			Application.Exit();
        }

		private void Form1_Load(object sender, EventArgs e)
        {
			this.BackColor = Color.LimeGreen;
			this.TransparencyKey = Color.LimeGreen;
			this.TopMost = true;

			SetupMaximizedWindow();
			SetupResizeableWindow();
		}

		private void SetupMaximizedWindow()
		{
			var rectangle = Screen.FromControl(this).Bounds;
			this.FormBorderStyle = FormBorderStyle.None;
			Size = new Size(rectangle.Width, rectangle.Height);
			Location = new Point(0, 0);
			Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
			this.Size = new Size(workingRectangle.Width, workingRectangle.Height);
		}

		private void SetupResizeableWindow()
		{
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
		}

		Control draggedPiece = null;
		bool resizing = false;
		private Point startDraggingPoint;
		private Size startSize;
		Rectangle rectProposedSize = Rectangle.Empty;
		private void piece_MouseDown(object sender, MouseEventArgs e)
		{
			draggedPiece = sender as Control;
			if (draggedPiece != null) lastClickedPic = (SizeablePictureBox)draggedPiece;

			if ((e.X <= resizingMargin) || (e.X >= draggedPiece.Width - resizingMargin) ||
				(e.Y <= resizingMargin) || (e.Y >= draggedPiece.Height - resizingMargin))
			{
				resizing = true;

				// Indicate resizing
				this.Cursor = Cursors.SizeNWSE;

				this.startSize = new Size(e.X, e.Y);
				Point pt = this.PointToScreen(draggedPiece.Location);
				rectProposedSize = new Rectangle(pt, startSize);
			}
			else
			{
				resizing = false;
				// Indicate moving
				this.Cursor = Cursors.SizeAll;
			}

			// Start point location
			this.startDraggingPoint = e.Location;
		}

		private void piece_MouseMove(object sender, MouseEventArgs e)
		{
			if (draggedPiece != null)
			{
				if (resizing)
				{
					rectProposedSize.Width = e.X - this.startDraggingPoint.X + this.startSize.Width;
					rectProposedSize.Height = e.Y - this.startDraggingPoint.Y + this.startSize.Height;

					if (rectProposedSize.Width > minimumSize && rectProposedSize.Height > minimumSize)
					{
						this.draggedPiece.Size = rectProposedSize.Size;
					}
					else
					{
						this.draggedPiece.Size = new Size((int)Math.Max(minimumSize, rectProposedSize.Width), Math.Max(minimumSize, rectProposedSize.Height));
					}
				}
				else
				{
					Point pt;
					if (draggedPiece == sender) pt = new Point(e.X, e.Y);
					else pt = draggedPiece.PointToClient((sender as Control).PointToScreen(new Point(e.X, e.Y)));

					draggedPiece.Left += pt.X - this.startDraggingPoint.X;
					draggedPiece.Top += pt.Y - this.startDraggingPoint.Y;
				}
			}
		}
		private void piece_MouseUp(object sender, MouseEventArgs e)
		{
			if (resizing)
			{
				if (rectProposedSize.Width > minimumSize && rectProposedSize.Height > minimumSize)
				{
					this.draggedPiece.Size = rectProposedSize.Size;
				}
				else
				{
					this.draggedPiece.Size = new Size((int)Math.Max(minimumSize, rectProposedSize.Width), Math.Max(minimumSize, rectProposedSize.Height));
				}
			}

			this.draggedPiece = null;
			this.startDraggingPoint = Point.Empty;
			this.Cursor = Cursors.Default;
		}
	}
}
