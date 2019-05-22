using CarBase.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CarBase.Data.Tests
{
    [TestClass]
    public class CarModelDaoTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            new CarModelDao().DeleteAll();
        }

        [TestMethod]
        public void InsertCarModel()
        {
            CarModelDao dao = new CarModelDao();

            Model car = new Model();
            car.Name = "Car1";
            car.PriceFrom = 10000;

            CarVersion v1 = new CarVersion();
            v1.Engine = "2.0";
            car.Versions.Add(v1);

            CarVersion v2 = new CarVersion();
            v2.Engine = "2.2";
            car.Versions.Add(v2);

            dao.Insert(car);

            Assert.AreEqual(1, dao.GetModelCount());
            Assert.AreEqual(2, dao.GetVersionCount());

            Model actualCar = dao.GetAllModels().Single();
            Assert.AreEqual("Car1", actualCar.Name);
            Assert.AreEqual(10000, actualCar.PriceFrom);
            Assert.AreEqual(2, actualCar.Versions.Count);
            
            new CarModelDao().DeleteAll();

            for (int i = 0; i < 50; i++)
            {
                Model carMore = new Model();
                carMore.Name = "Car" + i;
                carMore.PriceFrom = i;

                CarVersion v1More = new CarVersion();
                v1More.Engine = "2." + i;
                carMore.Versions.Add(v1More);

                CarVersion v2More = new CarVersion();
                v2More.Engine = "2." + i;
                carMore.Versions.Add(v2More);
                dao.Insert(carMore);
            }
            Assert.AreEqual(50, dao.GetModelCount());
            Assert.AreEqual<int>(100, dao.GetVersionCount());
        }
    }
}
