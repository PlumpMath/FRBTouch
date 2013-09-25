//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading;

namespace FRBTouch.MultiTouch
{
#pragma warning disable 1591
    /// <summary>
    /// All availible digitizer capabilities
    /// </summary>
    [Flags]
    public enum DigitizerStatus : byte
    {
        IntegratedTouch = 0x01,
        ExternalTouch = 0x02,
        IntegratedPan = 0x04,
        ExternalPan = 0x08,
        MultiInput = 0x40,
        StackReady = 0x80
    }
}