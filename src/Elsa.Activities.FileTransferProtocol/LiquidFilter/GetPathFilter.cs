using Elsa.Scripting.Liquid.Services;
using Fluid;
using Fluid.Values;
using System.IO;
using System.Threading.Tasks;

namespace Elsa.Activities.FileTransferProtocol.LiquidFilter
{
    public class GetPathFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var path = Path.GetDirectoryName(input.ToStringValue());
            return new StringValue(path);
        }
    }
}
