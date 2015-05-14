using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirectGraphResultFinder
{
    using Model;

    public partial class BasicIODisplay : Form
    {
        public BasicIODisplay(string intialInput)
        {
            InitializeComponent();
            txbInputText.Text = intialInput;
        }

        private void btnTakeActionOnText_Click(object sender, EventArgs e)
        {
            findRegionInformation(txbInputText.Text);
        }

        private void findRegionInformation(string inputText)
        {
            txbOutputText.BackColor = Color.White;
            txbOutputText.Text = String.Empty;
            DataPointRegion[] regionResults = null;
            var handleData = new BackgroundWorker();
            handleData.WorkerReportsProgress = true;
            handleData.ProgressChanged += new ProgressChangedEventHandler(handleData_ProgressChanged);
            handleData.DoWork += (sender,evengArgs) => 
                {
                    try
                    {
                        
                        var parsedData = ProcessData.parseGivenInput(inputText);
                        regionResults = ProcessData.findRegions(parsedData, handleData.ReportProgress);
                    }
                    catch (Exception ex)
                    {
                        evengArgs.Result = "Error While Processing Data:" + ex.ToString(); 
                    }
                };
            handleData.RunWorkerCompleted += (sender, evengArgs) => 
                {
                    string runResult = evengArgs.Result as string;
                    bool errorFound = !String.IsNullOrEmpty(runResult);
                    if (!errorFound)
                    {
                        runResult = ProcessData.exportInformation(regionResults);
                    }
                    returnRegionInformation(runResult, errorFound);
                };
            btnTakeActionOnText.Enabled = false;
            handleData.RunWorkerAsync();
        }

        private void handleData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbRunningStatus.Value = e.ProgressPercentage;
        }

        private void returnRegionInformation(string resultOfWork, bool wasErrorFound)
        {
            txbOutputText.BackColor = wasErrorFound ? Color.Tomato : Color.White;
            btnTakeActionOnText.Enabled = true;
            txbOutputText.Text = resultOfWork;
            tcDataResults.SelectedTab = tpOutput;
            pbRunningStatus.Value = 0;
            
        }
    }
}
