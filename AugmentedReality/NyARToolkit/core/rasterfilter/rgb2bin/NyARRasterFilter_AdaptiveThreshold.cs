/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System.Diagnostics;
using System.Threading;
namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRasterFilter_AdaptiveThreshold : INyARRasterFilter_Rgb2Bin
    {
        private IdoThFilterImpl _do_threshold_impl;

        public NyARRasterFilter_AdaptiveThreshold(int i_in_raster_type)
        {
            if (!initInstance(i_in_raster_type, NyARBufferType.INT1D_BIN_8))
            {
                throw new NyARException();
            }
        }
        public NyARRasterFilter_AdaptiveThreshold(int i_threshold, int i_in_raster_type, int i_out_raster_type)
        {
            if (!initInstance(i_in_raster_type, i_out_raster_type))
            {
                throw new NyARException();
            }
        }
        protected bool initInstance(int i_in_raster_type, int i_out_raster_type)
        {
            switch (i_out_raster_type)
            {
                case NyARBufferType.INT1D_BIN_8:
                    switch (i_in_raster_type)
                    {
                        case NyARBufferType.BYTE1D_X8R8G8B8_32:
                            this._do_threshold_impl = new doThFilterImpl_BUFFERFORMAT_BYTE1D_X8R8G8B8_32();
                            break;
                        case NyARBufferType.BYTE1D_R8G8B8_24:
                            this._do_threshold_impl = new doThFilterImpl_BUFFERFORMAT_BYTE1D_R8G8B8_24();
                            break;
                        default:
                            return false;
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void doFilter(INyARRgbRaster i_input, NyARBinRaster i_output)
        {

            Debug.Assert(i_input.getSize().isEqualSize(i_output.getSize()) == true);
            this._do_threshold_impl.doThFilter(i_input, i_output, i_output.getSize());
            return;
        }

        interface IdoThFilterImpl
        {
            void doThFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size);
        }

        class doThFilterImpl_BUFFERFORMAT_BYTE1D_X8R8G8B8_32 : IdoThFilterImpl
        {
            public void doThFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size)
            {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));

                int[] out_buf = (int[])i_output.getBuffer();
                byte[] in_buf = (byte[])i_input.getBuffer();

                byte[] grey = new byte[i_size.w * i_size.h];
                ulong[] integralImg = new ulong[i_size.w * i_size.h];

                ulong sum = 0;
                float T = 0.15f;
                double s = i_size.w / 8;
                int x1, x2, y1, y2;
                int index, s2 = (int)(s / 2);
                int count = 0;

                // greyscale
                for (int i = 0, j = 0; i < in_buf.Length; )
                {
                    grey[j++] = (byte)
                        ((in_buf[i + 1] & 0xff) * 0.11 + // b
                        (in_buf[i + 2] & 0xff) * 0.59 + // g
                        (in_buf[i + 3] & 0xff) * 0.3); // r
                    i += 4; // skip alpha
                }

                // integral image
                for (int i = 0; i < i_size.w; i++)
                {
                    sum = 0; // cumulative row sum
                    for (int j = 0; j < i_size.h; j++)
                    {
                        index = j * i_size.w + i;
                        sum += grey[index];
                        if (i == 0)
                            integralImg[index] = (ulong)sum;
                        else
                            integralImg[index] = integralImg[index - 1] + (ulong)sum;
                    }
                }

                for (int i = 0; i < i_size.w; i++)
                {
                    for (int j = 0; j < i_size.h; j++)
                    {
                        index = j * i_size.w + i;

                        // set the SxS region
                        x1 = i - s2; x2 = i + s2;
                        y1 = j - s2; y2 = j + s2;

                        // check the border
                        if (x1 < 0) x1 = 0;
                        if (x2 >= i_size.w) x2 = i_size.w - 1;
                        if (y1 < 0) y1 = 0;
                        if (y2 >= i_size.h) y2 = i_size.h - 1;

                        count = (x2 - x1) * (y2 - y1);

                        sum = integralImg[y2 * i_size.w + x2] -
                              integralImg[y1 * i_size.w + x2] -
                              integralImg[y2 * i_size.w + x1] +
                              integralImg[y1 * i_size.w + x1];

                        if ((long)(grey[index] * count) < (long)(sum * (1.0 - T)))
                            out_buf[index] = 0;
                        else
                            out_buf[index] = 1;
                    }
                }

                return;
            }
        }
        class doThFilterImpl_BUFFERFORMAT_BYTE1D_R8G8B8_24 : IdoThFilterImpl
        {
            public void doThFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size)
            {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));

                int[] out_buf = (int[])i_output.getBuffer();
                byte[] in_buf = (byte[])i_input.getBuffer();

                byte[] grey = new byte[i_size.w * i_size.h];
                ulong[] integralImg = new ulong[i_size.w * i_size.h];

                ulong sum = 0;
                float T = 0.15f;
                double s = i_size.w / 8;
                int x1, x2, y1, y2;
                int index, s2 = (int)(s / 2);
                int count = 0;

                // greyscale
                for (int i = 0, j = 0; i < in_buf.Length; )
                {
                    grey[j++] = (byte)
                        ((in_buf[i + 0] & 0xff) * 0.11 + // b
                        (in_buf[i + 1] & 0xff) * 0.59 + // g
                        (in_buf[i + 2] & 0xff) * 0.3); // r
                    i += 3; // skip alpha
                }

                // integral image
                for (int i = 0; i < i_size.w; i++)
                {
                    sum = 0; // cumulative row sum
                    for (int j = 0; j < i_size.h; j++)
                    {
                        index = j * i_size.w + i;
                        sum += grey[index];
                        if (i == 0)
                            integralImg[index] = (ulong)sum;
                        else
                            integralImg[index] = integralImg[index - 1] + (ulong)sum;
                    }
                }

                for (int i = 0; i < i_size.w; i++)
                {
                    for (int j = 0; j < i_size.h; j++)
                    {
                        index = j * i_size.w + i;

                        // set the SxS region
                        x1 = i - s2; x2 = i + s2;
                        y1 = j - s2; y2 = j + s2;

                        // check the border
                        if (x1 < 0) x1 = 0;
                        if (x2 >= i_size.w) x2 = i_size.w - 1;
                        if (y1 < 0) y1 = 0;
                        if (y2 >= i_size.h) y2 = i_size.h - 1;

                        count = (x2 - x1) * (y2 - y1);

                        sum = integralImg[y2 * i_size.w + x2] -
                              integralImg[y1 * i_size.w + x2] -
                              integralImg[y2 * i_size.w + x1] +
                              integralImg[y1 * i_size.w + x1];

                        if ((long)(grey[index] * count) < (long)(sum * (1.0 - T)))
                            out_buf[index] = 0;
                        else
                            out_buf[index] = 1;
                    }
                }

                return;
            }
        }
    }
}
