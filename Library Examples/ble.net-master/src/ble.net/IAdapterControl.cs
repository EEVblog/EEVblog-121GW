// Copyright Malachi Griffie
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

namespace nexus.protocols.ble
{
   /// <summary>
   /// The state of an adapter and controls to enable or disable it
   /// </summary>
   public interface IAdapterControl : IAdapterState
   {
      /// <summary>
      /// <c>true</c> if the current platform allows disabling this adapter
      /// </summary>
      Boolean AdapterCanBeDisabled { get; }

      /// <summary>
      /// <c>true</c> if the current platform allows enabling this adapter
      /// </summary>
      Boolean AdapterCanBeEnabled { get; }

      /// <summary>
      /// Disable this adapter system-wide
      /// </summary>
      /// <returns><c>true</c> if the operation succeeded in disabling the adapter</returns>
      Task<Boolean> DisableAdapter();

      /// <summary>
      /// Enable this adapter system-wide
      /// </summary>
      /// <returns><c>true</c> if the operation succeeded in enabling the adapter</returns>
      Task<Boolean> EnableAdapter();
   }
}