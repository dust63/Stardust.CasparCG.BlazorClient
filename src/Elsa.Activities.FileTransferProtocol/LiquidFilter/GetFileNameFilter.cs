using Elsa.Scripting.Liquid.Services;
using Fluid;
using Fluid.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Activities.FileTransferProtocol.LiquidFilter
{
    public class GetFilenameFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var fileName = Path.GetFileName(input.ToStringValue());
            return new StringValue(fileName);
        }
    }
}
