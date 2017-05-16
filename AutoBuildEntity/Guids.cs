// Guids.cs
// MUST match guids.h
using System;

namespace 陈珙.AutoBuildEntity
{
    static class GuidList
    {
        public const string guidAutoBuildEntityPkgString = "c095f8f8-3f87-4eac-8dc0-44939a85b2f2";
        public const string guidAutoBuildEntityCmdSetString = "cbe5d1f0-96e6-4ce2-90c5-860da753969c";

        public static readonly Guid guidAutoBuildEntityCmdSet = new Guid(guidAutoBuildEntityCmdSetString);
    };
}