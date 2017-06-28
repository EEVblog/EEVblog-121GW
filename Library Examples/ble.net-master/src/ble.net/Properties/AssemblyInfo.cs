﻿// Copyright Malachi Griffie
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Reflection;
using nexus.protocols.ble;

[assembly: AssemblyTitle( AssemblyInfo.ID )]
[assembly: AssemblyProduct( AssemblyInfo.ID )]
[assembly: AssemblyDescription( AssemblyInfo.DESCRIPTION )]

// ReSharper disable once CheckNamespace

namespace nexus.protocols.ble
{
   internal static partial class AssemblyInfo
   {
      internal const String ID = PROJECT_ID;
      internal const String DESCRIPTION = PROJECT_DESCRIPTION;
   }
}