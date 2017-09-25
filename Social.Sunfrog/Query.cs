using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Sunfrog
{
    class Query
    {

        public Category Get_Category_By_Name(string name)
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from category in context.Categories
                        where category.Name == name
                        select category;
            return query.FirstOrDefault();
        }
        public Product Get_Product_By_Id(int id)
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from product in context.Products
                        where product.Source_Id == id
                        select product;
            return query.FirstOrDefault();
        }
        public bool Check_Exits_Product(string product_id, int category_id)
        {
            bool result = false;
            try
            {
                MoneyDataContext db = new MoneyDataContext();
                var query = from product in db.Products
                            join category in db.ProductCategoryDetails on product.Id equals category.Product_Id
                            where product.Sku == product_id && category.Category.Id == category_id
                            select product;
                if (query.Count() > 0)
                    result = true;
            }
            catch (Exception ex) { }
            return result;
        }

        public Product Get_Product_Type_By_Id(int id)
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from product in context.Products
                        where product.Source_Id == id
                        select product;
            return query.FirstOrDefault();
        }

        public bool Insert_Product(DataLoad data)
        {
            bool result = false;
            try
            {
                MoneyDataContext db = new MoneyDataContext();
                Product prod = new Product
                {
                    Sku = data.Sku,
                    Group_Id = data.GroupId,
                    Name = data.Title,
                    Description = data.Description,
                    Url_Source = data.Url,
                    Url_Name = data.UrlName,
                    ProductTypeId = data.Type,
                    Image = data.Image,
                    Price = data.Price,
                    Keywords = data.Keywords,
                    CreatedDate = DateTime.Now,
                    Source_Id = 1
                };
                db.Products.InsertOnSubmit(prod);
                db.SubmitChanges();
                result = true;
            }
            catch (Exception ex) { }
            return result;
        }

        public bool Insert_Product_Category(int product_id, int category_id)
        {
            bool result = false;
            try
            {
                MoneyDataContext db = new MoneyDataContext();
                ProductCategoryDetail prod = new ProductCategoryDetail
                {
                    Product_Id = product_id,
                    Category_Id = category_id
                };
                db.ProductCategoryDetails.InsertOnSubmit(prod);
                db.SubmitChanges();
                result = true;
            }
            catch (Exception ex) { }
            return result;
        }
    }
}
