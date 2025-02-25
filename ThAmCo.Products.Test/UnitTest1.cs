using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThAmCo.Products.Services.ProductsRepo;
using System.Collections.Generic;

namespace ThAmCo.Products.Test
{
    [TestClass]
    public class ProductsRepoTest
    {
        [TestMethod]
        public async Task GetProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var products = await service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(products, "Product list should not be null.");
            Assert.AreEqual(3, products.Count(), "Product list should contain exactly 3 products.");
        }

        [TestMethod]
        public async Task GetProductsAsync_ShouldContainExpectedProductNames()
        {
            // Arrange
            var service = new ProductsRepoFake();
            var expectedNames = new HashSet<string> { "Fake repo product D", "Fake repo product E", "Fake repo product F" };

            // Act
            var products = await service.GetProductsAsync();

            // Assert
            CollectionAssert.AreEquivalent(expectedNames.ToList(), products.Select(p => p.Name).ToList(), "Product names do not match expected values.");
        }

        [TestMethod]
        public async Task GetProductsAsync_ShouldHaveCorrectProductIds()
        {
            // Arrange
            var service = new ProductsRepoFake();
            var expectedIds = new HashSet<int> { 1, 2, 3 };

            // Act
            var products = await service.GetProductsAsync();

            // Assert
            CollectionAssert.AreEquivalent(expectedIds.ToList(), products.Select(p => p.Id).ToList(), "Product IDs do not match expected values.");
        }
    }
}
