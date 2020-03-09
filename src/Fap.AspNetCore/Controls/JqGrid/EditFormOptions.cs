using Fap.Core.Extensions;

namespace Fap.AspNetCore.Controls.JqGrid
{
    public class EditFormOptions
    {
        public string Elmprefix { get; set; }
        public string Elmsuffix { get; set; }
        public string Label { get; set; }
        public int? Rowpos { get; set; }
        public int? Colpos { get; set; }

        public override string ToString()
        {
            return this.ToJsonIgnoreNullValue();
        }
    }
}
