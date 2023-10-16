using Microsoft.AspNetCore.Mvc;

namespace ReadingClub.UnitTests
{
    public static class TestHelper
    {
        public static bool IsHttpActionAttributePresent(Controller controller, string method, Type httpAttribute)
        {
            var exists = controller.GetType()
                .GetMethod(method)
                ?.GetCustomAttributes(httpAttribute, false)
                .Any();

            return exists ?? false;
        }
    }
}
