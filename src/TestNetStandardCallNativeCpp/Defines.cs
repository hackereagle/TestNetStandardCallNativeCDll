using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestNetStandardCallNativeCpp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageStruct
    {
        public int Width;
        public int Height;
        public int Channel;
        public IntPtr Data;

        private int GetChannel(PixelFormat pixelFormat)
        {   
            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    return 4;
                default:
                    return 0;
            }
        }

        GCHandle _pinnedArray;

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi, SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        public ImageStruct(Bitmap bm)
        { 
            try
            {
                this.Width = bm.Width;
                this.Height = bm.Height;
                this.Channel = GetChannel(bm.PixelFormat);
                byte[] data = new byte[bm.Width * bm.Height * this.Channel];

                if (bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    System.Drawing.Imaging.BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    int stride = bd.Stride;
                    int skipByte = stride - bd.Width;
                    int size = bm.Width * bm.Height;
                    if (skipByte == 0)
                        Marshal.Copy(bd.Scan0, data, 0, size);
                    else
                    {
                        int idx = 0;
                        for (int i = 0; i < bm.Height; i++)
                        {
                            Marshal.Copy(bd.Scan0 + i * stride, data, idx, bm.Width);
                            idx = idx + bm.Width;
                        }
                    }
                    bm.UnlockBits(bd);

                    _pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
                    this.Data = _pinnedArray.AddrOfPinnedObject();
                }
                else if (bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    System.Drawing.Imaging.BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    int stride = bd.Stride;
                    int lineSize = bm.Width * 3;
                    int skipByte = stride - lineSize;
                    int size = bm.Width * bm.Height * 3;
                    if (skipByte == 0)
                        Marshal.Copy(bd.Scan0, data, 0, size);
                    else
                    {
                        int idx = 0;
                        for (int i = 0; i < bm.Height; i++)
                        {
                            Marshal.Copy(bd.Scan0 + i * stride, data, idx, lineSize);
                            idx = idx + lineSize;
                        }
                    }
                    bm.UnlockBits(bd);

                    _pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
                    this.Data = _pinnedArray.AddrOfPinnedObject();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap ret;
            if (this.Channel == 1)
            {
                ret = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
                ColorPalette cp = ret.Palette;
                for (int i = 0; i < 256; i++)
                    cp.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                ret.Palette = cp;

                BitmapData bd = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                int stride = bd.Stride;
                int skipByte = stride - bd.Width;
                int size = ret.Width * ret.Height;

                if (skipByte == 0)
                    CopyMemory(bd.Scan0, Data, Convert.ToUInt32(size));
                else
                {
                    int idx = 0;
                    for (int i = 0; i < ret.Height; i++)
                    {
                        CopyMemory(bd.Scan0 + i * stride, Data + idx, Convert.ToUInt32(ret.Width));
                        idx = idx + ret.Width;
                    }
                }
                ret.UnlockBits( bd );
            }
            else if (this.Channel == 3)
            { 
                ret = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);

                BitmapData bd = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int lineSize = ret.Width * 3;
                int stride = bd.Stride;
                int skipByte = stride - lineSize;
                int size = lineSize * ret.Height;
                if (skipByte == 0)
                    CopyMemory(bd.Scan0, Data, Convert.ToUInt32(size));
                else
                {
                    int idx = 0;
                    for (int i = 0; i < ret.Height; i++)
                    {
                        CopyMemory(bd.Scan0 + i * stride, Data + idx, Convert.ToUInt32(lineSize));
                        //idx = idx + lineSize;
                        idx = idx + stride;
                    }
                }
                ret.UnlockBits( bd );
            }
            else if (this.Channel == 4)
            { 
                ret = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppRgb);

                BitmapData bd = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                int lineSize = ret.Width * 4;
                int stride = bd.Stride;
                int skipByte = stride - lineSize;
                int size = lineSize * ret.Height;
                if (skipByte == 0)
                    CopyMemory(bd.Scan0, Data, Convert.ToUInt32(size));
                else
                {
                    int idx = 0;
                    for (int i = 0; i < ret.Height; i++)
                    {
                        CopyMemory(bd.Scan0 + i * stride, Data + idx, Convert.ToUInt32(lineSize));
                        idx = idx + stride;
                    }
                }
                ret.UnlockBits( bd );
            }
            else
            {
                throw new NotImplementedException($"{this.Channel} channels image still not be defined!");
            }

            return ret;

        }

        public void Release()
        { 
            if (_pinnedArray.IsAllocated)
                _pinnedArray.Free();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct StructWithArr
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public int[] IParam;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public double[] DParam;
	};
}
