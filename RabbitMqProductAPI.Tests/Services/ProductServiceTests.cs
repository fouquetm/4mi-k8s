using System.Collections.Generic;
using FluentAssertions;
using Moq;
using RabbitMqProductAPI.Models;
using RabbitMqProductAPI.Services;
using RabbitMqProductAPI.RabbitMQ;
using Xunit;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace RabbitMqProductAPI.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IRabbitMQProducer> _rabbitMQProducerMock;

        public ProductServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _productServiceMock = _fixture.Freeze<Mock<IProductService>>();
            _rabbitMQProducerMock = _fixture.Freeze<Mock<IRabbitMQProducer>>();
        }

        [Fact]
        public void GetProductList_ShouldReturnListOfProducts()
        {
            // Arrange
            var expectedProducts = _fixture.Create<List<Product>>();
            _productServiceMock
                .Setup(service => service.GetProductList())
                .Returns(expectedProducts);

            var sut = _fixture.Create<IProductService>();

            // Act
            var result = sut.GetProductList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedProducts);
            result.Count().Should().Be(expectedProducts.Count);
            _productServiceMock.Verify(service => service.GetProductList(), Times.Once);
        }

        [Fact]
        public void GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var expectedProduct = _fixture.Create<Product>();
            _productServiceMock
                .Setup(service => service.GetProductById(expectedProduct.ProductId))
                .Returns(expectedProduct);

            var sut = _fixture.Create<IProductService>();

            // Act
            var result = sut.GetProductById(expectedProduct.ProductId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedProduct);
            _productServiceMock.Verify(service => service.GetProductById(expectedProduct.ProductId), Times.Once);
        }

        [Fact]
        public void GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            _productServiceMock
                .Setup(service => service.GetProductById(productId))
                .Returns((Product)null);

            var sut = _fixture.Create<IProductService>();

            // Act
            var result = sut.GetProductById(productId);

            // Assert
            result.Should().BeNull();
            _productServiceMock.Verify(service => service.GetProductById(productId), Times.Once);
        }


        [Fact]
        public void DeleteProduct_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            _productServiceMock
                .Setup(service => service.DeleteProduct(productId))
                .Returns(false);

            var sut = _fixture.Create<IProductService>();

            // Act
            var result = sut.DeleteProduct(productId);

            // Assert
            result.Should().BeFalse();
            _productServiceMock.Verify(service => service.DeleteProduct(productId), Times.Once);
        }

        [Fact]
        public void UpdateProduct_ShouldUpdateOnlyModifiedFields()
        {
            // Arrange
            var existingProduct = _fixture.Build<Product>()
                                          .With(p => p.ProductName, "Old Name")
                                          .Create();

            var updatedProduct = existingProduct;
            updatedProduct.ProductName = "New Name";

            _productServiceMock
                .Setup(service => service.UpdateProduct(updatedProduct))
                .Returns(updatedProduct);

            var sut = _fixture.Create<IProductService>();

            // Act
            var result = sut.UpdateProduct(updatedProduct);

            // Assert
            result.ProductName.Should().Be("New Name");
            result.ProductDescription.Should().Be(existingProduct.ProductDescription);
            _productServiceMock.Verify(service => service.UpdateProduct(updatedProduct), Times.Once);
        }

        [Fact]
        public void DeleteProduct_ShouldThrowException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            _productServiceMock
                .Setup(service => service.DeleteProduct(productId))
                .Throws(new Exception("Unexpected error"));

            var sut = _fixture.Create<IProductService>();

            // Act
            var act = () => sut.DeleteProduct(productId);

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Unexpected error");
            _productServiceMock.Verify(service => service.DeleteProduct(productId), Times.Once);
        }
    }
}
