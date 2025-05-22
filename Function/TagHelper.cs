using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace ProMats.Function
{
    public static class TagHelper
    {
        public static string IsActive(this IHtmlHelper helper, string controller, string action)
        {
            var context = helper.ViewContext;
            var values = context.RouteData.Values;

            string _controller = values["controller"]?.ToString();
            string _action = values["action"]?.ToString();

            return (action == _action && controller == _controller) ? "active bg-info" : "";
        }

        public static string IsMenuopen(this IHtmlHelper helper, string controller, string action)
        {
            var context = helper.ViewContext;
            var values = context.RouteData.Values;

            string _controller = values["controller"]?.ToString();
            string _action = values["action"]?.ToString();

            return ((action == _action || string.IsNullOrEmpty(action)) && controller == _controller) ? "menu-open" : "";
        }
    }
}