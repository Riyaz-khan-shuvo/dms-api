using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanDms.Service.ViewModel
{
    public class ProductReplaceReceiveDetailsVM
    {
        public int Id { get; set; }
        public int ProductReplaceReceiveId { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        [Display(Name = "Ctn Quantity")]
        public int CtnQuantity { get; set; }

        [Display(Name = "Pcs Quantity")]
        public int PcsQuantity { get; set; }

        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }

        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }

        [Display(Name = "SD")]
        public decimal SD { get; set; }

        [Display(Name = "SDAmount")]
        public decimal SDAmount { get; set; }
        public int? Line { get; set; }
    }

}
