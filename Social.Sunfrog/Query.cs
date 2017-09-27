using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Sunfrog
{
    class Query
    {
        public IEnumerable<Category> Get_Category_To_Search()
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from search in context.Categories
                        where search.IsActive == 1 && search.IsParent == 0
                        select search;
            return query;
        }

        public IEnumerable<Category> Get_Parent_Category_To_Search(int id)
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from search in context.Categories
                        where search.IsActive == 1 && search.Id == id
                        select search;
            return query;
        }
        public IEnumerable<ProductType> Get_Product_Type_To_Search()
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from search in context.ProductTypes
                        where search.Is_Active == 1
                        select search;
            return query;
        }
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

        public ProductType Get_Product_Type_By_Id(int id)
        {
            MoneyDataContext context = new MoneyDataContext();
            var query = from product in context.ProductTypes
                        where product.Id == id
                        select product;
            return query.FirstOrDefault();
        }

        public Product Insert_Product(DataLoad data)
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
            try
            {
                db.Products.InsertOnSubmit(prod);
                db.SubmitChanges();
            }
            catch (Exception ex) { }
            return prod;
        }

        public bool Insert_Product_Category(long product_id, int category_id)
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

        // Insert Pinterest

        public Pinterest Insert_Pinterest(long product_id, DataLoad data)
        {
            MoneyDataContext db = new MoneyDataContext();
            Pinterest prod = new Pinterest
            {
                Board = data.Tag,
                Backlink = data.UrlName,
                Note = data.Description,
                Image_Url = data.Image,
                Type = 1,
                Is_Pin = 0,
                Created_Date = DateTime.Now
            };
            
            try
            {
                db.Pinterests.InsertOnSubmit(prod);
                db.SubmitChanges();
            }
            catch (Exception ex) { }
            return prod;
        }

        public bool Insert_Product_Link_Pinterest(long product_id, int pinterest_id)
        {
            bool result = false;
            try
            {
                MoneyDataContext db = new MoneyDataContext();
                ProductPinterest prod = new ProductPinterest
                {
                    ProductId = product_id,
                    PinId = pinterest_id
                };
                db.ProductPinterests.InsertOnSubmit(prod);
                db.SubmitChanges();
                result = true;
            }
            catch (Exception ex) { }
            return result;
        }
    }
}
