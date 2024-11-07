using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.SFN18795
{
    public class SFN18795RequisitionItem
    {
        public int ID { get; set; }

        public int FormID { get; set; }

        public int Quantity { get; set; }

		[StringLength(20, ErrorMessage = "Item Number can only be 20 characters.")]
		public string ItemNumber { get; set; }

		[StringLength(4000, ErrorMessage = "Description can only be 4000 characters.")]
		public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal EstimatedCost { get; set; }

        public string Order { get; set; }

        public SFN18795RequisitionItem(int id, int quantity, string description, string itemNumber, decimal price, decimal estimatedCost, string order)
        {
            ID = id;
            Quantity = quantity;
            Description = description;
            ItemNumber = itemNumber;
            Price = price;
            EstimatedCost = estimatedCost;
            Order = order;
        }

        public static List<SFN18795RequisitionItem> ConvertDataTableToRequisitionItemList(DataTable dtReq)
        {
            List<SFN18795RequisitionItem> reqItemList = new List<SFN18795RequisitionItem>();
            foreach(DataRow row in dtReq.Rows)
            {
                SFN18795RequisitionItem reqItem = new SFN18795RequisitionItem(
					int.Parse(row["ID_NUMBER"].ToString()),
					int.Parse(row["ITEM_QUANTITY"].ToString()),
                    row["ITEM_DESCRIPTION"].ToString(),
                    row["ITEM_NUMBER"].ToString(),
                    decimal.Parse(row["ITEM_PRICE"].ToString()),
                    decimal.Parse(row["ITEM_ESTIMATED_COST"].ToString()),
                    row["ITEM_ORDER"].ToString()
                );

                reqItemList.Add(reqItem);
            }

            return reqItemList;
        }
    }
}