using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cortside.RestFS.WebApi.Tests {
    public abstract class ControllerTest<T> where T : ControllerBase {
        protected T controller;
        protected UnitTestFixture testFixture;

        protected ControllerTest() {
            testFixture = new UnitTestFixture();
        }
        protected ControllerContext GetControllerContext() {
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = new DefaultHttpContext();
            return controllerContext;
        }
    }
}
