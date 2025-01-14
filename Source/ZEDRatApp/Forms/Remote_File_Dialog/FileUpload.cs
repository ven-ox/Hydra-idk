using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using Guna.UI2.WinForms;


using ZEDRAT.Module;
using Button = Guna.UI2.WinForms.Guna2Button;
using Panel = Guna.UI2.WinForms.Guna2Panel;

namespace ZEDRatApp.Forms.Remote_File_Dialog
{
	public class FileUpload : Form
	{
		public Remote_File_Manager rfm;

		public string UploadFilename;

		public string DirectoryPath;

		public string FileSize;

		public Task tk;

		private IContainer components;

		public CancellationTokenSource source = new CancellationTokenSource();

		private Label label1;

		private Label label2;

		private Button btnBrowse;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private Button button1;

		private Panel panel1;

		private Guna2Elipse Guna2Elipse1;

		private Guna2HtmlLabel txtStatus;

		private Guna2HtmlLabel txtTrans;

		private Guna2HtmlLabel txtFilename;

		private Guna2HtmlLabel Guna2HtmlLabel3;

		private Guna2HtmlLabel Guna2HtmlLabel;

		private Guna2HtmlLabel Guna2HtmlLabel1;

		private Guna2Button Guna2Button1;

		private Guna2Button btnBuild;

		private Guna2ShadowForm guna2ShadowForm1;

		private Guna2AnimateWindow Guna2AnimateWindow1;

		private Guna2ControlBox guna2ControlBox1;

		private Guna2HtmlLabel Guna2HtmlLabel12;

		private BunifuSeparator bunifuSeparator2;

		public FileUpload(Remote_File_Manager _rfm, string directorypath)
		{
			this.InitializeComponent();
			this.rfm = _rfm;
			this.DirectoryPath = directorypath;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				openFileDialog.Filter = "*.*|";
				openFileDialog.ValidateNames = true;
				openFileDialog.CheckPathExists = true;
				openFileDialog.CheckFileExists = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					this.UploadFilename = openFileDialog.FileName;
					FileInfo fileInfo = new FileInfo(this.UploadFilename);
					this.DirectoryPath = this.DirectoryPath + "\\" + fileInfo.Name;
					((Control)(object)this.txtFilename).Text = fileInfo.Name;
					this.FileSize = this.GetFileSize(fileInfo.Length);
					((Control)(object)this.txtTrans).Text = this.FileSize;
					((Control)(object)this.txtStatus).Text = "Waiting for upload...";
				}
			}
			catch (Exception)
			{
			}
		}

		private string GetFileSize(double size)
		{
			try
			{
				if (size >= 0.0 && size < 1024.0)
				{
					return size + " Bytes";
				}
				double num = size / 1024.0;
				if (num >= 1.0 && num < 1024.0)
				{
					return num.ToString("#.0") + " KB";
				}
				double num2 = size / 1048576.0;
				if (num2 >= 1.0 && num2 < 1024.0)
				{
					return num2.ToString("#.0") + " MB";
				}
				double num3 = size / 1073741824.0;
				return (num3 >= 1.0) ? (num3.ToString("#.0") + " GB") : null;
			}
			catch
			{
				return null;
			}
		}

		private void RemoteFile_BlockSend(byte[] bt)
		{
			try
			{
				Task.Factory.StartNew(delegate
				{
					uint num4 = 102400u;
					uint num5 = 0u;
					uint num3 = 0u;
					List<byte> list = new List<byte>(bt);
					int count = list.Count;
					while (list.Count > 15000 && !this.source.IsCancellationRequested)
					{
						this.rfm.Remote_File_Send("UploadBegin", list.GetRange(0, 15000).ToArray());
						list.RemoveRange(0, 15000);
						num3 += 15000u;
						num5 += 15000;
						if (num5 > num4)
						{
							Task.Delay(500);
							num5 = 0u;
						}
						base.Invoke((MethodInvoker)delegate
						{
							((Control)(object)this.txtTrans).Text = this.GetFileSize(num3) + "/" + this.FileSize;
						});
					}
					base.Invoke((MethodInvoker)delegate
					{
						((Control)(object)this.txtTrans).Text = this.FileSize + "/" + this.FileSize;
					});
					this.rfm.Remote_File_Send("UploadBegin", list.ToArray());
					this.rfm.Remote_File_Send("UploadEnd", Encoding.UTF8.GetBytes(count + "|" + this.DirectoryPath));
					this.rfm.Remote_File_Send("FileList", Encoding.UTF8.GetBytes(this.DirectoryPath));
				});
			}
			catch
			{
				throw;
			}
		}

		private void FileUpLoad_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				if (this.tk != null)
				{
					if (!this.tk.IsCompleted)
					{
						this.source.Cancel();
					}
					this.tk.Dispose();
				}
				this.source.Dispose();
				this.rfm.Remote_File_Send("ExitUpload", Encoding.UTF8.GetBytes("hello"));
			}
			catch
			{
			}
			finally
			{
				this.rfm = null;
				this.tk = null;
				this.source = null;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(this.UploadFilename))
				{
					return;
				}
				using FileStream fileStream = new FileStream(this.UploadFilename, FileMode.Open, FileAccess.Read);
				using BinaryReader binaryReader = new BinaryReader(fileStream);
				byte[] array = binaryReader.ReadBytes((int)fileStream.Length);
				if (array.Length == 0)
				{
					throw new Exception("File Length is null");
				}
				((Control)(object)this.txtTrans).Text = "Uploading..";
				this.RemoteFile_BlockSend(array);
				((Control)(object)this.txtStatus).Text = "File upload successfully";
			}
			catch (Exception)
			{
			}
		}

		private void siticoneGradientCircleButton1_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
																																																																					this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZEDRatApp.Forms.Remote_File_Dialog.FileUpload));
			this.Guna2Elipse1 = new Guna2Elipse(this.components);
			this.Guna2HtmlLabel1 = new Guna2HtmlLabel();
			this.Guna2HtmlLabel = new Guna2HtmlLabel();
			this.Guna2HtmlLabel3 = new Guna2HtmlLabel();
			this.txtFilename = new Guna2HtmlLabel();
			this.txtTrans = new Guna2HtmlLabel();
			this.txtStatus = new Guna2HtmlLabel();
			this.btnBuild = new Guna2Button();
			this.Guna2Button1 = new Guna2Button();
			this.guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
			this.Guna2AnimateWindow1 = new Guna2AnimateWindow(this.components);
			this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
			this.Guna2HtmlLabel12 = new Guna2HtmlLabel();
			this.bunifuSeparator2 = new Bunifu.UI.WinForms.BunifuSeparator();
			base.SuspendLayout();
			this.Guna2Elipse1.BorderRadius = 0;
			this.Guna2Elipse1.TargetControl = this;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).BackColor = System.Drawing.Color.Transparent;
			this.Guna2HtmlLabel1.ForeColor = System.Drawing.Color.FromArgb(226, 28, 71);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).Location = new System.Drawing.Point(43, 58);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).Name = "Guna2HtmlLabel1";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).Size = new System.Drawing.Size(53, 15);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).TabIndex = 0;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1).Text = "File Name:";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).BackColor = System.Drawing.Color.Transparent;
			this.Guna2HtmlLabel.ForeColor = System.Drawing.Color.FromArgb(226, 28, 71);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).Location = new System.Drawing.Point(43, 98);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).Name = "Guna2HtmlLabel";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).Size = new System.Drawing.Size(110, 15);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).TabIndex = 1;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel).Text = "Transmission progress:";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).BackColor = System.Drawing.Color.Transparent;
			this.Guna2HtmlLabel3.ForeColor = System.Drawing.Color.FromArgb(226, 28, 71);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).Location = new System.Drawing.Point(43, 138);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).Name = "Guna2HtmlLabel3";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).Size = new System.Drawing.Size(36, 15);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).TabIndex = 2;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3).Text = "Status:";
			((System.Windows.Forms.Control)(object)this.txtFilename).BackColor = System.Drawing.Color.Transparent;
			this.txtFilename.ForeColor = System.Drawing.Color.White;
			((System.Windows.Forms.Control)(object)this.txtFilename).Location = new System.Drawing.Point(109, 59);
			((System.Windows.Forms.Control)(object)this.txtFilename).Name = "txtFilename";
			((System.Windows.Forms.Control)(object)this.txtFilename).Size = new System.Drawing.Size(6, 15);
			((System.Windows.Forms.Control)(object)this.txtFilename).TabIndex = 3;
			((System.Windows.Forms.Control)(object)this.txtFilename).Text = ".";
			((System.Windows.Forms.Control)(object)this.txtTrans).BackColor = System.Drawing.Color.Transparent;
			this.txtTrans.ForeColor = System.Drawing.Color.White;
			((System.Windows.Forms.Control)(object)this.txtTrans).Location = new System.Drawing.Point(186, 99);
			((System.Windows.Forms.Control)(object)this.txtTrans).Name = "txtTrans";
			((System.Windows.Forms.Control)(object)this.txtTrans).Size = new System.Drawing.Size(6, 15);
			((System.Windows.Forms.Control)(object)this.txtTrans).TabIndex = 4;
			((System.Windows.Forms.Control)(object)this.txtTrans).Text = ".";
			((System.Windows.Forms.Control)(object)this.txtStatus).BackColor = System.Drawing.Color.Transparent;
			this.txtStatus.ForeColor = System.Drawing.Color.White;
			((System.Windows.Forms.Control)(object)this.txtStatus).Location = new System.Drawing.Point(87, 138);
			((System.Windows.Forms.Control)(object)this.txtStatus).Name = "txtStatus";
			((System.Windows.Forms.Control)(object)this.txtStatus).Size = new System.Drawing.Size(6, 15);
			((System.Windows.Forms.Control)(object)this.txtStatus).TabIndex = 5;
			((System.Windows.Forms.Control)(object)this.txtStatus).Text = ".";
			(this.btnBuild).BorderColor = System.Drawing.Color.FromArgb(226, 28, 71);
			(this.btnBuild).BorderThickness = 1;
			(this.btnBuild).CheckedState.Parent = this.btnBuild;
			(this.btnBuild).CustomBorderColor = System.Drawing.Color.FromArgb(226, 28, 71);
			(this.btnBuild).CustomBorderThickness = new System.Windows.Forms.Padding(1, 0, 0, 0);
			(this.btnBuild).CustomImages.Parent = this.btnBuild;
			(this.btnBuild).FillColor = System.Drawing.Color.FromArgb(18, 15, 24);
			((System.Windows.Forms.Control)(object)this.btnBuild).Font = new System.Drawing.Font("Century Gothic", 9f);
			((System.Windows.Forms.Control)(object)this.btnBuild).ForeColor = System.Drawing.Color.White;
			((System.Windows.Forms.Control)(object)this.btnBuild).Location = new System.Drawing.Point(393, 169);
			((System.Windows.Forms.Control)(object)this.btnBuild).Name = "btnBuild";
			(this.btnBuild).ShadowDecoration.Parent = (System.Windows.Forms.Control)(object)this.btnBuild;
			((System.Windows.Forms.Control)(object)this.btnBuild).Size = new System.Drawing.Size(123, 36);
			((System.Windows.Forms.Control)(object)this.btnBuild).TabIndex = 24;
			((System.Windows.Forms.Control)(object)this.btnBuild).Text = "Upload";
			((System.Windows.Forms.Control)(object)this.btnBuild).Click += new System.EventHandler(button1_Click);
			(this.Guna2Button1).BorderColor = System.Drawing.Color.FromArgb(226, 28, 71);
			(this.Guna2Button1).BorderThickness = 1;
			(this.Guna2Button1).CheckedState.Parent = this.Guna2Button1;
			(this.Guna2Button1).CustomBorderColor = System.Drawing.Color.FromArgb(226, 28, 71);
			(this.Guna2Button1).CustomBorderThickness = new System.Windows.Forms.Padding(1, 0, 0, 0);
			(this.Guna2Button1).CustomImages.Parent = this.Guna2Button1;
			(this.Guna2Button1).FillColor = System.Drawing.Color.FromArgb(18, 15, 24);
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Font = new System.Drawing.Font("Century Gothic", 9f);
			((System.Windows.Forms.Control)(object)this.Guna2Button1).ForeColor = System.Drawing.Color.White;
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Location = new System.Drawing.Point(24, 169);
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Name = "Guna2Button1";
			(this.Guna2Button1).ShadowDecoration.Parent = (System.Windows.Forms.Control)(object)this.Guna2Button1;
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Size = new System.Drawing.Size(123, 36);
			((System.Windows.Forms.Control)(object)this.Guna2Button1).TabIndex = 25;
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Text = "Browse";
			((System.Windows.Forms.Control)(object)this.Guna2Button1).Click += new System.EventHandler(btnBrowse_Click);
			this.guna2ShadowForm1.TargetForm = this;
			this.guna2ControlBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.guna2ControlBox1.BorderColor = System.Drawing.Color.DarkGray;
			this.guna2ControlBox1.BorderRadius = 12;
			this.guna2ControlBox1.FillColor = System.Drawing.Color.FromArgb(226, 28, 71);
			this.guna2ControlBox1.HoverState.Parent = this.guna2ControlBox1;
			this.guna2ControlBox1.IconColor = System.Drawing.Color.Black;
			this.guna2ControlBox1.Location = new System.Drawing.Point(482, 12);
			this.guna2ControlBox1.Name = "guna2ControlBox1";
			this.guna2ControlBox1.ShadowDecoration.Parent = this.guna2ControlBox1;
			this.guna2ControlBox1.Size = new System.Drawing.Size(45, 29);
			this.guna2ControlBox1.TabIndex = 54;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).Anchor = System.Windows.Forms.AnchorStyles.Top;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).BackColor = System.Drawing.Color.Transparent;
			this.Guna2HtmlLabel12.Font = new System.Drawing.Font("Century Gothic", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Guna2HtmlLabel12.ForeColor = System.Drawing.Color.LightGray;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).Location = new System.Drawing.Point(203, 14);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).Name = "Guna2HtmlLabel12";
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).Size = new System.Drawing.Size(93, 20);
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).TabIndex = 99;
			((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12).Text = "File Upload";
			this.bunifuSeparator2.BackColor = System.Drawing.Color.Transparent;
			this.bunifuSeparator2.BackgroundImage = (System.Drawing.Image)resources.GetObject("bunifuSeparator2.BackgroundImage");
			this.bunifuSeparator2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.bunifuSeparator2.DashCap = Bunifu.UI.WinForms.BunifuSeparator.CapStyles.Flat;
			this.bunifuSeparator2.LineColor = System.Drawing.Color.DimGray;
			this.bunifuSeparator2.LineStyle = Bunifu.UI.WinForms.BunifuSeparator.LineStyles.DoubleEdgeFaded;
			this.bunifuSeparator2.LineThickness = 1;
			this.bunifuSeparator2.Location = new System.Drawing.Point(-3, 43);
			this.bunifuSeparator2.Margin = new System.Windows.Forms.Padding(6, 2, 6, 2);
			this.bunifuSeparator2.Name = "bunifuSeparator2";
			this.bunifuSeparator2.Orientation = Bunifu.UI.WinForms.BunifuSeparator.LineOrientation.Horizontal;
			this.bunifuSeparator2.Size = new System.Drawing.Size(544, 10);
			this.bunifuSeparator2.TabIndex = 98;
			this.BackColor = System.Drawing.Color.FromArgb(9, 8, 13);
			base.ClientSize = new System.Drawing.Size(539, 222);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel12);
			base.Controls.Add(this.bunifuSeparator2);
			base.Controls.Add(this.guna2ControlBox1);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.Guna2Button1);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.btnBuild);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.txtStatus);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.txtTrans);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.txtFilename);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel3);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel);
			base.Controls.Add((System.Windows.Forms.Control)(object)this.Guna2HtmlLabel1);
			this.Font = new System.Drawing.Font("Century Gothic", 8.999999f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.ForeColor = System.Drawing.Color.FromArgb(226, 28, 71);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "FileUpload";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(FileUpLoad_FormClosed);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
