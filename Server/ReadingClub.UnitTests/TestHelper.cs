using Microsoft.AspNetCore.Mvc;

namespace ReadingClub.UnitTests
{
    public static class TestHelper
    {
        public static bool IsAttributePresent(Controller controller, string method, Type attribute)
        {
            var exists = controller.GetType()
                .GetMethod(method)
                ?.GetCustomAttributes(attribute, false)
                .Any();

            return exists ?? false;
        }

        public static bool IsAttributePresentAtControllerLevel(Controller controller, Type attribute)
        {
            var exists = controller.GetType()
                ?.GetCustomAttributes(attribute, false)
                .Any();

            return exists ?? false;
        }
    }
}
