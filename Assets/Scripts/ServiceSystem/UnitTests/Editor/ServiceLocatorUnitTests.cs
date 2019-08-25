using NUnit.Framework;

namespace Utils.Core.Services.Testing
{
    [TestFixture]
    public class ServiceLocatorUnitTests
    {
        public interface ITestInterface
        {
            void TestMethod();
        }

        public class TestImplementationOne : ITestInterface
        {
            public void TestMethod() { }
        }

        public class TestImplementationTwo : ITestInterface
        {
            public void TestMethod() { }
        }

        private class TestServiceWithoutFactory : IService
        {

        }

        private class TestServiceWithFactory : IService
        {
            public readonly ITestInterface testDependency;

            public TestServiceWithFactory(ITestInterface testDependency)
            {
                this.testDependency = testDependency;
            }
        }

        private class TestServiceFactory : IServiceFactory<TestServiceWithFactory>
        {
            public bool useOne = true;

            public TestServiceWithFactory Construct()
            {
                ITestInterface implementation;
                if (useOne)
                {
                    implementation = new TestImplementationOne();
                }
                else
                {
                    implementation = new TestImplementationTwo();
                }

                return new TestServiceWithFactory(implementation);
            }
        }

        [Test]
        public void TestService_Without_Factory_Is_Instantiated()
        {
            TestServiceWithoutFactory testService = GlobalServiceLocator.Instance.Get<TestServiceWithoutFactory>();
            Assert.IsInstanceOf<TestServiceWithoutFactory>(testService);
        }
        
        [Test]
        public void TestService_With_Factory_Is_Instantiated()
        {
            TestServiceWithFactory testService = GlobalServiceLocator.Instance.Get<TestServiceWithFactory>();
            Assert.IsInstanceOf<TestServiceWithFactory>(testService);
            Assert.IsInstanceOf<TestImplementationOne>(testService.testDependency);
        }
    }
}
