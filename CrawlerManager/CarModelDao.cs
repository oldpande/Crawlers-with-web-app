using CarBase.Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarBase.CrawlerManager
{
    public class CarModelDao
    {
        public List<Brand> GetAllBrands()
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Brands
                    .ToList();
            }
        }

        public Brand GetBrandByName(string name)
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Brands
                    .Include("Models")
                    .SingleOrDefault(b => b.Name == name);
            }
        }

        public void DeleteAll()
        {
            using (var entityData = new EntityDataModel())
            {
                entityData.Database.ExecuteSqlCommand("delete from Model");
            }
        }

        public void Insert(Model model)
        {
            using (var entityData = new EntityDataModel())
            {
                entityData.Models.Add(model);
                entityData.SaveChanges();
            }
        }

        public List<Model> GetSpecificModels(string brandName)
        {
            using (var entityData = new EntityDataModel())
            {
                var table = entityData.Models.Join(entityData.Brands,
                    m => m.BrandId,
                    b => b.Id,
                    (m, b) => new
                    {
                        ID = m.Id,
                        m.Name,
                        m.PriceFrom,
                        m.Link,
                        BrandModel = b.Name
                    })
                    .Where(t => t.BrandModel == brandName)
                    .ToList();
                var modelList = new List<Model>();
                foreach(var t in table)
                {
                    var model = new Model();
                    model.Id = t.ID;
                    model.Link = t.Link;
                    model.Name = t.Name;
                    model.PriceFrom = t.PriceFrom;
                    modelList.Add(model);
                }    

                return modelList;
            }
        }

        public Model GetModelByName(string modelName)
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Models
                    .Include("Versions")
                    .SingleOrDefault(b => b.Name == modelName);
            }
        }

        public List<Model> GetAllModels()
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Models
                    .Include("Versions")
                    .ToList();
            }
        }

        public int GetModelCount()
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Models.Count();
            }
        }

        public List<CarVersion> GetAllVersions()
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Versions
                    .ToList();
            }
        }

        public int GetVersionCount()
        {
            using (var entityData = new EntityDataModel())
            {
                return entityData.Versions.Count();
            }
        }
    }
}
