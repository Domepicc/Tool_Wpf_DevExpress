using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using Tool_Wpf_DevExpress.ViewModel;


namespace Tool_Wpf_DevExpress.View
{
    /// <summary>
    /// Interaction logic for ProductPriceChart.xaml
    /// </summary>
    public partial class ProductPriceChartView : ThemedWindow
    {
        public ProductPriceChartView()
        {
            InitializeComponent();
            DataContext = new ProductPriceChartViewModel();

        }
    }
}
