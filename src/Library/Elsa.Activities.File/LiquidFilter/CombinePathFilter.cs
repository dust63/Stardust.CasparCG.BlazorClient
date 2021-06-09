using Elsa.Scripting.Liquid.Services;
using Fluid;
using Fluid.Values;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Elsa.Activities.File.LiquidFilter
{
    public class CombinePathFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var path = arguments.Values.Select(x => x.ToStringValue()).Aggregate(input.ToStringValue(), (cur, path) => Path.Combine(cur, path));
            return new StringValue(path);
        }
    }
}
