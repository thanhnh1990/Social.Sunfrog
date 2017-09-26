using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Sunfrog
{
    class Logic
    {
        public int execute(DataLoad data)
        {
            int status = 0;
            Logs log = new Logs();
            try
            {
                if (!Is_Exist_Local(data))
                    status = 1;
                else{
                    log.ILogs("Exist: " + data.Sku);
                    status = 2;
                }
            }
            catch (Exception ex)
            {
                log.IErrors("Logic - Execute: " + ex.Message);
            }
            return status;
        }

        public bool Is_Exist_Local(DataLoad data)
        {
            bool result = true;
            Logs log = new Logs();
            try
            {
                Query local = new Query();
                if (!local.Check_Exits_Product(data.Sku, data.CategoryId)){
                    Product product = local.Insert_Product(data);
                    long product_id = product.Id;
                    local.Insert_Product_Category(product.Id, data.CategoryId);
                    Pinterest pinterest = local.Insert_Pinterest(product_id, data);
                    local.Insert_Product_Link_Pinterest(product_id, pinterest.Id);
                    data.TypeName = local.Get_Product_Type_By_Id(data.Type).Name;
                    result = false;
                }
            }
            catch (Exception ex)
            {
                log.IErrors("Logic - Is_Exist_Local: " + ex.Message);
            }
            return result;
        }
    }
}
