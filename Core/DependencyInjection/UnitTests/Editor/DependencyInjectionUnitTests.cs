using NUnit.Framework;
using UnityEngine;
using Utils.Core.Services;

namespace Utils.Core.Injection.Testing
{
    [TestFixture]
    public class DependencyInjectionUnitTests
    {
        public class TestService : IService
        {

        }

        public class InjectionTestWithConstructor
        {
            public readonly TestService dependency;

            public InjectionTestWithConstructor(TestService testService)
            {
                dependency = testService;
            }
        }

        public class InjectionTestWithMethod
        {
            public TestService dependency;

            public void InjectDependencies(TestService testService)
            {
                dependency = testService;
            }
        }

        public class MultipleDependencyInjectionTest
        {
            public TestService testService;
            public NonServiceTest nonServiceTest;

            public void InjectDependencies(TestService testService, NonServiceTest nonServiceTest)
            {
                this.testService = testService;
                this.nonServiceTest = nonServiceTest;
            }
        }

        public class TestMonoBehaviour : MonoBehaviour
        {
            public TestService dependency;

            public void InjectDependencies(TestService testService)
            {
                dependency = testService;
            }
        }

        public class NonServiceTest
        {

        }

        public class InjectionTestRegisteredObject
        {
            public NonServiceTest dependency;

            public void InjectDependencies(NonServiceTest testClass)
            {
                dependency = testClass;
            }
        }

        private DependencyInjector injector;

        [SetUp]
        public void Setup()
        {
            injector = new DependencyInjector();
        }

        [Test]
        public void Injected_Service_Via_Method()
        {
            InjectionTestWithMethod test = new InjectionTestWithMethod();
            injector.InjectMethod(test);

            Assert.IsNotNull(test.dependency);
            Assert.IsInstanceOf<TestService>(test.dependency);
        }

        [Test]
        public void Injected_Service_Via_GameObject()
        {
            GameObject instance = new GameObject();
            TestMonoBehaviour testMonoBehaviour = instance.AddComponent<TestMonoBehaviour>();
            injector.InjectGameObject(instance);

            Assert.IsNotNull(testMonoBehaviour.dependency);
            Assert.IsInstanceOf<TestService>(testMonoBehaviour.dependency);

            Object.DestroyImmediate(instance);
        }

        [Test]
        public void Injected_Registered_Instance_To_DependencyInjector()
        {
            NonServiceTest nonServiceTestClass = new NonServiceTest();
            injector.RegisterInstance<NonServiceTest>(nonServiceTestClass);

            InjectionTestRegisteredObject test = new InjectionTestRegisteredObject();
            injector.InjectMethod(test);

            Assert.IsNotNull(test.dependency);
            Assert.IsInstanceOf<NonServiceTest>(test.dependency);
        }

        [Test]
        public void Unregistered_Instance_From_DependencyInjector()
        {
            NonServiceTest nonServiceTest = new NonServiceTest();
            injector.RegisterInstance<NonServiceTest>(nonServiceTest);

            nonServiceTest = null;
            injector.UnRegisterInstance<NonServiceTest>();

            InjectionTestRegisteredObject test = new InjectionTestRegisteredObject();
            injector.InjectMethod(test);

            Assert.IsNull(test.dependency);
        }

        [Test]
        public void Injected_Service_And_Instance_Via_Method()
        {
            NonServiceTest nonServiceTest = new NonServiceTest();
            injector.RegisterInstance<NonServiceTest>(nonServiceTest);

            MultipleDependencyInjectionTest test = new MultipleDependencyInjectionTest();
            injector.InjectMethod(test);

            Assert.IsNotNull(test.testService);
            Assert.IsNotNull(test.nonServiceTest);
            Assert.IsInstanceOf<TestService>(test.testService);
            Assert.IsInstanceOf<NonServiceTest>(test.nonServiceTest);
        }

        public interface ITest { }
        public class TestImplementationOne : ITest { }
        public class TestImplementationTwo : ITest { }

        public class DependencyTestClass
        {
            public ITest dependency;

            public void InjectDependencies(ITest testClass)
            {
                dependency = testClass;
            }
        }

        [Test]
        public void Registered_Interface_Implementation()
        {
            ITest test = new TestImplementationOne();
            injector.RegisterInstance<ITest>(test);

            DependencyTestClass classWithDependency = new DependencyTestClass();
            injector.InjectMethod(classWithDependency);

            Assert.IsNotNull(classWithDependency.dependency);
            Assert.IsInstanceOf<TestImplementationOne>(classWithDependency.dependency);
        }
    }
}
