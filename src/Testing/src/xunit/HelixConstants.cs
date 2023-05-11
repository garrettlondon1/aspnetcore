// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Testing;

public static class HelixConstants
{
    public const string Windows10Arm64 = "Windows.10.Arm64v8.Open;";
    public const string DebianAmd64 = "Debian.11.Amd64.Open;";
    public const string DebianArm64 = "Debian.11.Arm64.Open;";
    public const string AlmaLinuxAmd64 = "(AlmaLinux.8.Amd64.Open)Ubuntu.1804.Amd64.Open@svencontainers.azurecr.io/nonroot-alma;";
    public const string NativeAotNotSupportedHelixQueues = "All.OSX;All.Linux;Windows.11.Amd64.Client.Open;Windows.Amd64.Server2022.Open;windows.11.arm64.open";
}
