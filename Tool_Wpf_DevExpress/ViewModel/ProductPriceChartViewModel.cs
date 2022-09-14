using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DevExpress.Mvvm;
using Tool_Wpf_DevExpress.Model;
using System.Collections.ObjectModel;

namespace Tool_Wpf_DevExpress.ViewModel
{
    class ProductPriceChartViewModel : ViewModelBase
    {
        CostoGiornalieroEncodedEquipmentEntities CostoGiornalieroEncodedEquipmentDbContext;
        
        public ProductPriceChartViewModel()
        {
            if (IsInDesignMode)
            {
                CostoGiornalieroEncodedEquipments = new ObservableCollection<CostoGiornalieroEncodedEquipment>();
            }
            else
            {                                                              
                CostoGiornalieroEncodedEquipmentDbContext = new CostoGiornalieroEncodedEquipmentEntities();
                CostoGiornalieroEncodedEquipmentDbContext.CostoGiornalieroEncodedEquipment.Load();
                CostoGiornalieroEncodedEquipments = CostoGiornalieroEncodedEquipmentDbContext.CostoGiornalieroEncodedEquipment.Local;
            }

        }

        public ObservableCollection<CostoGiornalieroEncodedEquipment> CostoGiornalieroEncodedEquipments
        {
            get => GetValue<ObservableCollection<CostoGiornalieroEncodedEquipment>>();
            private set => SetValue(value);
        }



    }
}
