using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using MetroFramework.Controls;


// TEECKO Dev
// https://mugen.co.id/

namespace MnOT
{

	public partial class HalUtama : MetroFramework.Forms.MetroForm
	{

		public float temperature;

		public HalUtama()
		{
			InitializeComponent();
			lblFQDN.Text = GetFQDN();
			lblCPUNAME.Text = GetCPUNAME();
			InsertData(ref lstPerangkat);
			InsertDataHDD(ref lstHDD);
			InsertDataMEM(ref lstMEMORY);
			InsertDataNET(ref lstNET);
		}

		public static string GetFQDN()
		{
			string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
			string hostName = Dns.GetHostName();
			string fqdn = "";
			if (!hostName.Contains(domainName))
				fqdn = hostName + "." +domainName;
			else
				fqdn = hostName;

			return fqdn;
		}
		public static string GetCPUNAME() 
		{
			string cpuname = "";
			ManagementClass mc = new ManagementClass("win32_processor");
			ManagementObjectCollection moc = mc.GetInstances();

			foreach (ManagementObject mo in moc)
			{
				if (cpuname == "")
				{
					cpuname = mo.Properties["Name"].Value.ToString();
				}
			}
			return cpuname;
		}
		
		private void HalUtama_Load(object sender, EventArgs e)
        {
			notifyIcon1.BalloonTipText = "MnOT Minimize Double Klik Untuk Membuka.";
			timer1.Start();
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
			float fcpu = pCPU.NextValue();
			float fram = pRAM.NextValue();
			float fdisk = pDISK.NextValue();
			float fhandle = pHANDLE.NextValue();
			float fthreads = pTHREADS.NextValue();
			float fmemava = pMEMAVA.NextValue();
			float fchace = pCHACE.NextValue();
			float freaddisk = pREADDISK.NextValue();
			float fwritedisk = pWRITEDISK.NextValue();

			circCPU.Value = (int)fcpu;
			circCPU.Text = string.Format("{0:0.00}%", fcpu);

			circRAM.Value = (int)fram;
			circRAM.Text = string.Format("{0:0.00}%", fram);

			circDISK.Value = (int)fdisk;
			circDISK.Text = string.Format("{0:0.00}%", fdisk);

			lblHANDLE.Text = string.Format("{0:0}", fhandle);
			lblTHREADS.Text = string.Format("{0:0}", fthreads);
			lblMEMAVA.Text = string.Format("{0:0.0} GB", fmemava / 1024);
			lblCHACE.Text = string.Format("{0:0.0} MB", fchace / 1024f);

			lblREAD.Text = string.Format("{0:0} KB/s", freaddisk / 1024);
			lblWRITE.Text = string.Format("{0:0} KB/s", fwritedisk / 1024);


			var computer = new Computer();
			computer.CPUEnabled = true;
			computer.Open();


			foreach (var hardware in computer.Hardware)
			{
				if (hardware.HardwareType == HardwareType.CPU)
				{
					hardware.Update();
					foreach (var sensor in hardware.Sensors)
					{

						if (sensor.SensorType == SensorType.Temperature)
							temperature = sensor.Value.GetValueOrDefault();

					}
				}
			}
			circCPUTEMP.Value = (int)temperature;
			lblCPUTEMP.Text = string.Format("{0:0}", temperature);
		}

		private void InsertData(ref MetroListView lst)
        {
			lst.Items.Clear();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
			try
			{
				foreach (ManagementObject share in searcher.Get())
				{

					ListViewGroup grp;
					try
					{
						grp = lst.Groups.Add(share["Nama"].ToString(), share["Nama"].ToString());
					}
					catch
					{
						grp = lst.Groups.Add(share.ToString(), share.ToString());
					}

					if (share.Properties.Count <= 0)
					{
						MessageBox.Show("No Information Available", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					foreach (PropertyData PC in share.Properties)
					{

						ListViewItem item = new ListViewItem(grp);
						if (lst.Items.Count % 2 != 0)
							item.BackColor = Color.White;
						else
							item.BackColor = Color.WhiteSmoke;

						item.Text = PC.Name;

						if (PC.Value != null && PC.Value.ToString() != "")
						{
							switch (PC.Value.GetType().ToString())
							{
								case "System.String[]":
									string[] str = (string[])PC.Value;

									string str2 = "";
									foreach (string st in str)
										str2 += st + " ";

									item.SubItems.Add(str2);

									break;
								case "System.UInt16[]":
									ushort[] shortData = (ushort[])PC.Value;


									string tstr2 = "";
									foreach (ushort st in shortData)
										tstr2 += st.ToString() + " ";

									item.SubItems.Add(tstr2);

									break;

								default:
									item.SubItems.Add(PC.Value.ToString());
									break;
							}
						}
						lst.Items.Add(item);
						
					}
				}
			}


			catch (Exception exp)
			{
				MessageBox.Show("Data Error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void InsertDataHDD(ref MetroListView lst)
		{
			lst.Items.Clear();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
			try
			{
				foreach (ManagementObject share in searcher.Get())
				{

					ListViewGroup grp;
					try
					{
						grp = lst.Groups.Add(share["Nama"].ToString(), share["Nama"].ToString());
					}
					catch
					{
						grp = lst.Groups.Add(share.ToString(), share.ToString());
					}

					if (share.Properties.Count <= 0)
					{
						MessageBox.Show("No Information Available", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					foreach (PropertyData PC in share.Properties)
					{

						ListViewItem item = new ListViewItem(grp);
						if (lst.Items.Count % 2 != 0)
							item.BackColor = Color.White;
						else
							item.BackColor = Color.WhiteSmoke;

						item.Text = PC.Name;

						if (PC.Value != null && PC.Value.ToString() != "")
						{
							switch (PC.Value.GetType().ToString())
							{
								case "System.String[]":
									string[] str = (string[])PC.Value;

									string str2 = "";
									foreach (string st in str)
										str2 += st + " ";

									item.SubItems.Add(str2);

									break;
								case "System.UInt16[]":
									ushort[] shortData = (ushort[])PC.Value;


									string tstr2 = "";
									foreach (ushort st in shortData)
										tstr2 += st.ToString() + " ";

									item.SubItems.Add(tstr2);

									break;

								default:
									item.SubItems.Add(PC.Value.ToString());
									break;
							}
						}
						lst.Items.Add(item);

					}
				}
			}


			catch (Exception exp)
			{
				MessageBox.Show("Data Error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		private void InsertDataMEM(ref MetroListView lst)
		{
			lst.Items.Clear();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
			try
			{
				foreach (ManagementObject share in searcher.Get())
				{

					ListViewGroup grp;
					try
					{
						grp = lst.Groups.Add(share["Nama"].ToString(), share["Nama"].ToString());
					}
					catch
					{
						grp = lst.Groups.Add(share.ToString(), share.ToString());
					}

					if (share.Properties.Count <= 0)
					{
						MessageBox.Show("No Information Available", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					foreach (PropertyData PC in share.Properties)
					{

						ListViewItem item = new ListViewItem(grp);
						if (lst.Items.Count % 2 != 0)
							item.BackColor = Color.White;
						else
							item.BackColor = Color.WhiteSmoke;

						item.Text = PC.Name;

						if (PC.Value != null && PC.Value.ToString() != "")
						{
							switch (PC.Value.GetType().ToString())
							{
								case "System.String[]":
									string[] str = (string[])PC.Value;

									string str2 = "";
									foreach (string st in str)
										str2 += st + " ";

									item.SubItems.Add(str2);

									break;
								case "System.UInt16[]":
									ushort[] shortData = (ushort[])PC.Value;


									string tstr2 = "";
									foreach (ushort st in shortData)
										tstr2 += st.ToString() + " ";

									item.SubItems.Add(tstr2);

									break;

								default:
									item.SubItems.Add(PC.Value.ToString());
									break;
							}
						}
						lst.Items.Add(item);

					}
				}
			}


			catch (Exception exp)
			{
				MessageBox.Show("Data Error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		private void InsertDataNET(ref MetroListView lst)
		{
			lst.Items.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_NetworkAdapter");
			try
			{
				foreach (ManagementObject share in searcher.Get())
				{

					ListViewGroup grp;
					try
					{
						grp = lst.Groups.Add(share["Nama"].ToString(), share["Nama"].ToString());
					}
					catch
					{
						grp = lst.Groups.Add(share.ToString(), share.ToString());
					}

					if (share.Properties.Count <= 0)
					{
						MessageBox.Show("No Information Available", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					foreach (PropertyData PC in share.Properties)
					{

						ListViewItem item = new ListViewItem(grp);
						if (lst.Items.Count % 2 != 0)
							item.BackColor = Color.White;
						else
							item.BackColor = Color.WhiteSmoke;

						item.Text = PC.Name;

						if (PC.Value != null && PC.Value.ToString() != "")
						{
							switch (PC.Value.GetType().ToString())
							{
								case "System.String[]":
									string[] str = (string[])PC.Value;

									string str2 = "";
									foreach (string st in str)
										str2 += st + " ";

									item.SubItems.Add(str2);

									break;
								case "System.UInt16[]":
									ushort[] shortData = (ushort[])PC.Value;


									string tstr2 = "";
									foreach (ushort st in shortData)
										tstr2 += st.ToString() + " ";

									item.SubItems.Add(tstr2);

									break;

								default:
									item.SubItems.Add(PC.Value.ToString());
									break;
							}
						}
						lst.Items.Add(item);

					}
				}
			}


			catch (Exception exp)
			{
				MessageBox.Show("Data Error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

        private void HalUtama_Resize(object sender, EventArgs e)
        {
			if (FormWindowState.Minimized == this.WindowState)
			{
				notifyIcon1.Visible = true;
				notifyIcon1.ShowBalloonTip(500);
				this.Hide();
			}
			else if (FormWindowState.Normal == this.WindowState)
			{
				notifyIcon1.Visible = false;
			}
		}

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
				this.Show();
				this.WindowState = FormWindowState.Normal;
		}
    }
}
