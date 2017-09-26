using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Sunfrog
{
    class Execute
    {
        public bool Get_Shirts()
        {
            bool result = false;
            Logs log = new Logs();
            try
            {
                Query query_local = new Query();
                var search_category = query_local.Get_Category_To_Search();
                var product_type = query_local.Get_Product_Type_To_Search();
                if (search_category != null && product_type != null)
                {
                    foreach (var cat in search_category)
                    {
                        string parent_name =  query_local.Get_Parent_Category_To_Search(Int32.Parse(cat.Parent_Id.ToString())).FirstOrDefault().Name;
                        foreach (var type in product_type)
                        {
                            int ddot = 1;
                            bool first = true;
                            int count = 0;
                            int count_more = 0;
                            for (int i = 0; i < 500; i++)
                            {
                                Crawler crawler = new Crawler();
                                if (ddot == 0)
                                    break;
                                if (first)
                                    first = false;
                                else
                                    count_more = count + 1;
                                string url = TypeSearch(cat, type.Slug, count_more);
                                ddot = crawler.ReadHtmlResponse(url, parent_name, cat.Name, cat.Id, type.Id, i);
                                if (ddot == -1)
                                    return false;
                                else if (ddot == 2)
                                    break;
                                count = count + 40;
                                log.ILogs(count_more + " - " + url);
                            }
                            log.ILogs("-------------Finish :" + " - " + type.Name + "---------------");
                        }
                        log.ILogs("-------------Finish :" + " - " + parent_name + " - " + cat.Name + "---------------");
                    }
                }
            }
            catch (Exception ex)
            {
                log.IErrors("Action - Crawler: " + ex.Message);
            }
            return result;
        }
        private string TypeSearch(Category cat, string type, int count)
        {
            string url = "";
            if (cat != null)
            {
                switch (cat.LoadType)
                {
                    case 1:
                        url = FormatUrl(cat.Slug, cat.Site_Id.ToString(), type, count);
                        break;
                }
            }
            return url;
        }
        private string FormatUrl(string keyword, string cat, string type, int count)
        {
            return string.Format("https://www.sunfrog.com/search/paged3.cfm?schTrmFilter=new&productType={0}&search={1}&cID={2}&offset={3}", type, keyword, cat, count);
        }
    }
}
