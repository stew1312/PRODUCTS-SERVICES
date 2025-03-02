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
        // checking  the  product count
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
        //matching product names
        [TestMethod]
        public async Task GetProductsAsync_ShouldContainExpectedProductNames()
        {
            // Arrange
            var service = new ProductsRepoFake();
            var expectedNames = new HashSet<string> { "Fake Shirt", "Fake Tie", "Fake Belt" };

            // Act
            var products = await service.GetProductsAsync();
            var actualNames = products.Select(p => p.Name.Trim()).ToList(); // Trim to remove spaces

            // Assert
            CollectionAssert.AreEquivalent(expectedNames.ToList(), actualNames, "Product names do not match expected values.");
        }
        //matching the ids to the expected values
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
        //searching correct product by id
        [TestMethod]
        public async Task SearchProductAsync_ShouldReturnCorrectProduct_WhenIdExists()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var product = await service.SearchProductAsync(1);

            // Assert
            Assert.IsNotNull(product, "Product should not be null.");
            Assert.AreEqual(1, product.Id);
            Assert.AreEqual("Fake Shirt", product.Name);
        }
        //searching for non existent product by id
        [TestMethod]
        public async Task SearchProductAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var product = await service.SearchProductAsync(888); // Non-existent ID

            // Assert
            Assert.IsNull(product, "Product should be null when not found.");
        }

        // Searching  by the  exact Product Name
        [TestMethod]
        public async Task SearchProductByNameAsync_ShouldReturnCorrectProduct_WhenNameExists()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var product = await service.SearchProductByNameAsync("Fake Shirt");

            // Assert
            Assert.IsNotNull(product, "Product should not be null.");
            Assert.AreEqual("Fake Shirt", product.Name);
        }

        // Searching by the Name (Case-Insensitive)
        [TestMethod]
        public async Task SearchProductByNameAsync_ShouldReturnCorrectProduct_WhenNameExists_IgnoringCase()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var product = await service.SearchProductByNameAsync("fake shirt"); // Lowercase

            // Assert
            Assert.IsNotNull(product, "Product should not be null.");
            Assert.AreEqual("Fake Shirt", product.Name);
        }

        //Searching  by the  Name (Product Not Found)
        [TestMethod]
        public async Task SearchProductByNameAsync_ShouldReturnNull_WhenNameDoesNotExist()
        {
            // Arrange
            var service = new ProductsRepoFake();

            // Act
            var product = await service.SearchProductByNameAsync("Nonexistent Product");

            // Assert
            Assert.IsNull(product, "Product should be null when not found.");
        }

        

        
    }
}
