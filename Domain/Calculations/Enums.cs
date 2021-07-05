using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain
{
    public enum ClimateZoneEnum
    {
        [Description("1A")]
        OneA,
        [Description("2A")]
        TwoA,
        [Description("2B")]
        TwoB,
        [Description("3A")]
        ThreeA,
        [Description("3B")]
        ThreeB,
        [Description("3B-Coast")]
        ThreeBCoast,
        [Description("3C")]
        ThreeC,
        [Description("4A")]
        FourA,
        [Description("4B")]
        FourB,
        [Description("4C")]
        FourC,
        [Description("5A")]
        FiveA,
        [Description("5B")]
        FiveB,
        [Description("6A")]
        SixA,
        [Description("6B")]
        SixB,
        [Description("7")]
        Seven,
        [Description("8")]
        Eight

    }
}
