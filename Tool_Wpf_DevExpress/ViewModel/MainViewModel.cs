using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ZXing;
using ZXing.QrCode;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using Tool_Wpf_DevExpress.Model;
using Tool_Wpf_DevExpress.View;


namespace Tool_Wpf_DevExpress.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        ToolsEntities toolsDbContext;
        readonly string PATH = @"C:\Users\dpiccinni\source\repos\Tool_Wpf_DevExpress\";
        string nameFileTemplate = "Template.xlsx";
        string nameSheetTemplate = "Template";
        string partialNameSheetReport = "Report Tool ";
        readonly int SIZE_QR_CORE = 150;

        public MainViewModel()
        {
            if(IsInDesignMode)
            {
                Tools = new ObservableCollection<Tools>();
            }
            else
            {
                toolsDbContext = new ToolsEntities();
                toolsDbContext.Tools.Load();
                Tools = toolsDbContext.Tools.Local;
            }
            ReportButton_Command = new RelayCommand(ReportButton);
            ProductPriceChart_Command = new RelayCommand(ProductPriceChart);
        }

        public Tools _toolSelected { get; set; }

        public ObservableCollection<Tools> Tools
        {
            get => GetValue<ObservableCollection<Tools>>();
            private set => SetValue(value);
        }

        public Tools SelectedItem
        {
            get { return GetProperty(() => _toolSelected); }
            set { SetProperty(() => _toolSelected, value);}
        }

        public RelayCommand ReportButton_Command { get; set; }

        public RelayCommand ProductPriceChart_Command { get; set; }

        private void ProductPriceChart(object sender)
        {
            ProductPriceChartView ppc = new ProductPriceChartView();
            ppc.Show();

        }

        private void ReportButton(object sender)
        {
            Tools data = new Tools();

            if((data = SelectedItem) != null)
            {
                Byte[] qrCore = CreateQrCode(data.BoschCode, SIZE_QR_CORE);
                CreateReport(data, qrCore);
            }    
        }

        private void CreateReport(Tools data, Byte[] qrCode)
        {
            string path = PATH;
            IWorkbook workbook;
            string reportString = "Report";
            string extensionFile = ".xls";
            string nameFileReport = reportString + Guid.NewGuid().ToString().Substring(0, 4) + extensionFile;

            // Loading Template.xlsx and save the workbook
            using (FileStream sw = File.Open(path + nameFileTemplate, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(sw);
                sw.Close();
            }

            // Loading sheet, change sheet's name and poputate its
            ISheet sheet = workbook.GetSheet(nameSheetTemplate);
            workbook.SetSheetName(0, partialNameSheetReport + data.IdTool);

            using (FileStream sw = File.Open(path + nameFileReport, FileMode.Create, FileAccess.Write))
            {
                string nameSheetReport = partialNameSheetReport + data.IdTool;

                ISheet sheetReport = workbook.GetSheet(nameSheetReport);
                if (sheetReport is null) workbook.CreateSheet(nameSheetReport);

                PopulateSheet(sheetReport, data);
                AddImageToExcel(workbook, sheetReport, qrCode);
                workbook.Write(sw);
                sw.Close();
            }
        }

        private void PopulateSheet (ISheet sheet, Tools data)
        {
            int indexCellColumn = 1;

            sheet.GetRow(3).CreateCell(indexCellColumn).SetCellValue(data.BoschCode);
            sheet.GetRow(4).CreateCell(indexCellColumn).SetCellValue(data.Description);
            sheet.GetRow(5).CreateCell(indexCellColumn).SetCellValue(data.PrimarySupplier);
            sheet.GetRow(6).CreateCell(indexCellColumn).SetCellValue(data.SecondarySupplier);
            sheet.GetRow(7).CreateCell(indexCellColumn).SetCellValue(data.Quantity.ToString());
        }

        private Byte[] CreateQrCode(string qrText, int sizeQrCode)
        {
            Byte[] byteArray;

            string path = PATH;
            string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);

            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = sizeQrCode,
                    Width = sizeQrCode
                }
            };
            var pixelData = qrCodeWriter.Write(qrText);

            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to folder
                    // bitmap.Save(path + "/file-" + fileGuid + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    // save to stream as PNG
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }
            return byteArray;

        }

        private void AddImageToExcel(IWorkbook workbook, ISheet sheet, byte[] data)
        {
            int D_1 = 0;
            int D_2 = 1023;
            int Col1 = 1;
            int Row1 = 8;

            int pictureIndex = workbook.AddPicture(data, PictureType.PNG);
            ICreationHelper helper = workbook.GetCreationHelper();
            IDrawing drawing = sheet.CreateDrawingPatriarch();
            IClientAnchor anchor = helper.CreateClientAnchor();

            anchor.Col1 = Col1;
            anchor.Row1 = Row1;
            anchor.Col2 = Col1 + 1;
            anchor.Row2 = Row1 + 5;
            anchor.Dx1 = D_1;
            anchor.Dy1 = D_1;
            anchor.Dx2 = D_2;
            anchor.Dy2 = D_2;

            IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
        }
    }
}
