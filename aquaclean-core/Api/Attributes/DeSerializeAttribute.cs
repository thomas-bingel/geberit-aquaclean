using System;

namespace Geberit.AquaClean.Core.Api
{
    internal class DeSerializeAttribute : Attribute
    {
        public int Length { get; set; }
    }
}