// ------------------------------------------------------------------------------------------------
// <copyright file="IconService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Corral.Desktop.Services;

/// <summary>
///   Win32-based implementation of <see cref="IIconService" /> that resolves file, folder,
///   shortcut, and URL icons via <c>SHGetFileInfo</c>. Produced <see cref="ImageSource" />
///   instances are frozen so they can be consumed from any thread.
/// </summary>
public class IconService : IIconService
{
  #region Fields

  private readonly ConcurrentDictionary<string, ImageSource> _cache =
    new(StringComparer.OrdinalIgnoreCase);

  #endregion

  #region Implementation of IIconService

  /// <inheritdoc />
  public ImageSource GetIcon(string path)
  {
    if (string.IsNullOrWhiteSpace(path))
    {
      return null;
    }

    if (_cache.TryGetValue(path, out var cached))
    {
      return cached;
    }

    var icon = Resolve(path);
    if (icon != null)
    {
      _cache[path] = icon;
    }

    return icon;
  }

  #endregion

  #region Helpers

  private static ImageSource Resolve(string path)
  {
    // URLs — the shell can't resolve them to a per-site favicon without extra work,
    // so use a reasonable default from the system imageres.dll.
    if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && !uri.IsFile)
    {
      return ExtractFromSystemResource("imageres.dll", 20); // generic globe/link
    }

    var shfi = new SHFILEINFO();
    const uint flags = SHGFI_ICON | SHGFI_LARGEICON | SHGFI_USEFILEATTRIBUTES;
    const uint fileAttributes = 0x80; // FILE_ATTRIBUTE_NORMAL

    // If the path exists on disk, ask for the real icon (resolves .lnk targets automatically).
    var useRealIcon = File.Exists(path) || Directory.Exists(path);
    var effectiveFlags = useRealIcon ? SHGFI_ICON | SHGFI_LARGEICON : flags;

    var result = SHGetFileInfo(
      path,
      useRealIcon ? 0 : fileAttributes,
      ref shfi,
      (uint)Marshal.SizeOf(shfi),
      effectiveFlags
    );

    if (result == IntPtr.Zero || shfi.hIcon == IntPtr.Zero)
    {
      return null;
    }

    try
    {
      var bitmap = Imaging.CreateBitmapSourceFromHIcon(
        shfi.hIcon,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions()
      );

      bitmap.Freeze();
      return bitmap;
    }
    finally
    {
      DestroyIcon(shfi.hIcon);
    }
  }

  private static ImageSource ExtractFromSystemResource(string dll, int index)
  {
    var hIcon = IntPtr.Zero;
    try
    {
      ExtractIconEx(dll, index, out hIcon, out var _, 1);
      if (hIcon == IntPtr.Zero)
      {
        return null;
      }

      var bitmap = Imaging.CreateBitmapSourceFromHIcon(
        hIcon,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions()
      );

      bitmap.Freeze();
      return bitmap;
    }
    finally
    {
      if (hIcon != IntPtr.Zero)
      {
        DestroyIcon(hIcon);
      }
    }
  }

  #endregion

  #region Win32 Interop

  private const uint SHGFI_ICON = 0x000000100;
  private const uint SHGFI_LARGEICON = 0x000000000;
  private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  private struct SHFILEINFO
  {
    public IntPtr hIcon;
    public int iIcon;
    public uint dwAttributes;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szDisplayName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    public string szTypeName;
  }

  [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
  private static extern IntPtr SHGetFileInfo(
    string pszPath,
    uint dwFileAttributes,
    ref SHFILEINFO psfi,
    uint cbFileInfo,
    uint uFlags);

  [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
  private static extern int ExtractIconEx(
    string lpszFile,
    int nIconIndex,
    out IntPtr phiconLarge,
    out IntPtr phiconSmall,
    int nIcons);

  [DllImport("user32.dll")]
  private static extern bool DestroyIcon(IntPtr hIcon);

  #endregion
}
