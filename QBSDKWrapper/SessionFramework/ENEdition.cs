// Copyright 2007 by Intuit Inc.
// All rights reserved
// Usage governed by the QuickBooks SDK Developer's License Agreement

#pragma warning disable IDE1006 // Naming Styles
namespace SessionFramework
{
    public enum ENEdition
    {
        edUS = 0,
        edCA = 1,
        edUK = 2,
    }

    static class QBEdition
    {
        public static readonly string[] codes = { "US", "CA", "UK" };

        public static string getEdition(ENEdition ed)
        {
            return codes[(int)ed];
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles