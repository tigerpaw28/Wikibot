using System;
using System.Collections.Generic;
using System.Text;
using WikiClientLibrary.Generators;

namespace Wikibot.Logic.Extensions
{
    public static class PropertyFilterOptionExtension
    {
        public static string ToString(this PropertyFilterOption value,
    string withValue, string withoutValue, string allValue = "all")
        {
            switch (value)
            {
                case PropertyFilterOption.Disable:
                    return allValue;
                case PropertyFilterOption.WithProperty:
                    return withValue;
                case PropertyFilterOption.WithoutProperty:
                    return withoutValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
