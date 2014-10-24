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
    public interface INyARRaster
    {
    	
	    int getWidth();
	    int getHeight();
	    NyARIntSize getSize();
	    /**
	     * �o�b�t�@�I�u�W�F�N�g��Ԃ��܂��B
	     * @return
	     */
	    object getBuffer();
	    /**
	     * �o�b�t�@�I�u�W�F�N�g�̃^�C�v��Ԃ��܂��B
	     * @return
	     */
	    int getBufferType();
	    /**
	     * �o�b�t�@�̃^�C�v��i_type_value�ł��邩�A�`�F�b�N���܂��B
	     * @param i_type_value
	     * @return
	     */
	    bool isEqualBufferType(int i_type_value);
	    /**
	     * getBuffer���I�u�W�F�N�g��Ԃ��邩�̐^�U�l�ł��B
	     * @return
	     */
	    bool hasBuffer();
	    /**
	     * i_ref_buf�����b�v���܂��B�ł�����萮�����`�F�b�N���s���܂��B
	     * �o�b�t�@�̍ă��b�s���O���\�Ȋ֐��̂݁A���̊֐����������Ă��������B
	     * @param i_ref_buf
	     */
	    void wrapBuffer(object i_ref_buf);
    }
}