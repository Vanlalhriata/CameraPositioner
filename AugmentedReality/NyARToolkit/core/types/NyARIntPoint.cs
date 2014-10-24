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
namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * @deprecated ���̃N���X�͖��̕ύX�̂��߁A�폜����܂��B
     * @see NyARIntPoint2d
     */
    [System.Obsolete("This class will be deleted. See NyARIntPoint2d.--���̃N���X�͖��̕ύX�̂��߁A�폜����܂��BNyARIntPoint2d���g���ĉ������B")]
    public class NyARIntPoint : NyARIntPoint2d
    {
        /**
         * �z��t�@�N�g��
         * @param i_number
         * @return
         */
        public new static NyARIntPoint[] createArray(int i_number)
        {
            NyARIntPoint[] ret = new NyARIntPoint[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARIntPoint();
            }
            return ret;
        }
    }
}