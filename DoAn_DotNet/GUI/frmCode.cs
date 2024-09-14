using AForge.Video;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using BLL;
using static DoAn_DotNet.GUI.frmSell;
using AForge.Video.DirectShow;

namespace DoAn_DotNet.GUI
{
    public partial class frmCode : Form
    {
        DonHangBLL bllDonHang = new DonHangBLL();
        public layMaTCQR send;
        public frmCode()
        {
            InitializeComponent();
        }

        public frmCode(layMaTCQR sender)
        {
            InitializeComponent();
            this.send = sender;
        }

        frmSell sell = new frmSell();
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        private void ConnectCamera()
        {
            // Lấy danh sách các thiết bị video (camera)
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Không tìm thấy camera nào.");
                return;
            }

            // Chọn camera đầu tiên trong danh sách
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Hiển thị khung hình mới từ camera lên PictureBox
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pic_cam.Image = bitmap;
        }

        private void DisconnectCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        //Load thông báo
        public void Alert(string msg, frmCustomTB.enmType type)
        {
            frmCustomTB frm = new frmCustomTB();
            frm.showAlert(msg, type);
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (btn_Connect.Text == "Connect")
            {
                timer1.Enabled = true;
                timer1.Start();
                ConnectCamera();
                btn_Connect.Text = "Disconnect";
            }
            else
            {
                btn_Connect.Text = "Connect";
                //timer1.Stop();
                //stream.Stop();
                DisconnectCamera();
            }

        }
        public void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            pic_cam.Image = bmp;
        }
        private void frmCode_FormClosing(Object sender, FormClosingEventArgs e)
        {
            DisconnectCamera();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap img = (Bitmap)pic_cam.Image;
            if (img != null)
            {
                ZXing.BarcodeReader Reader = new ZXing.BarcodeReader();
                Result result = Reader.Decode(img);
                try
                {
                    string decoded = result.ToString().Trim();
                    string maTCQR = "";
                    bool kt = true;
                    if (maTCQR.Trim() == "")
                    {
                        maTCQR = decoded;
                        if (this.send(maTCQR) == false)
                        {
                            DisconnectCamera();
                            img.Dispose();
                            this.Close();
                            kt = false;
                            this.Alert("Thú cưng đã có trong giỏ hàng", frmCustomTB.enmType.Error);
                        }
                        DisconnectCamera();
                        if (kt == true)
                        {
                            this.Alert("Thú cưng đã được thêm vào giỏ hàng", frmCustomTB.enmType.Success);
                        }
                    }
                    img.Dispose();
                    this.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "");
                }
            }
        }
    }

}
