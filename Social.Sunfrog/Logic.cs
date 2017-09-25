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
                {
                    if (!Is_Exist_Server(data))
                    {
                        NewProduct(data);
                        QueryLocal local = new QueryLocal();
                        local.Update_Exist(int.Parse(data.Sku));
                        status = 1;
                    }
                    else
                        status = 2;
                    var db = DBConnection.Instance();
                    db.Close();
                    return status;
                }
                else
                {
                    log.ILogs("Exist: " + data.Sku);
                    return 2;
                }
            }
            catch (Exception ex)
            {
                log.IErrors("Logic - Execute: " + ex.Message);
                return status;
            }
        }

        public bool Is_Exist_Local(DataLoad data)
        {
            bool result = false;
            Logs log = new Logs();
            try
            {
                Query local = new Query();
                if (!local.Check_Exits_Product(data.Sku, data.CategoryId))
                    local.Insert_Product(data);
                else
                    result = true;
                data.TypeName = local.Get_Product_Type_By_Id(data.Type).Name;
            }
            catch (Exception ex)
            {
                log.IErrors("Logic - Is_Exist_Local: " + ex.Message);
            }
            return result;
        }
    }
}
