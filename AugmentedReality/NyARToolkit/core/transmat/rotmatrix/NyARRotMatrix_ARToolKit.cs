﻿/* 
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
using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRotMatrix_ARToolKit : NyARRotMatrix
    {
        /**
         * インスタンスを準備します。
         * 
         * @param i_param
         */
        public NyARRotMatrix_ARToolKit(NyARPerspectiveProjectionMatrix i_matrix):base(i_matrix)
        {
            this.__initRot_vec1 = new NyARRotVector(i_matrix);
            this.__initRot_vec2 = new NyARRotVector(i_matrix);
            this._angle = new NyARDoublePoint3d();
            return;
        }
        private NyARRotVector __initRot_vec1;
        private NyARRotVector __initRot_vec2;
        protected NyARDoublePoint3d _angle;

	    public override void initRotBySquare(NyARLinear[] i_linear,NyARDoublePoint2d[] i_sqvertex)
	    {
		    base.initRotBySquare(i_linear,i_sqvertex);
		    //Matrixからangleをロード
		    this.updateAngleFromMatrix();
		    return;
	    }
	    public NyARDoublePoint3d refAngle()
	    {
		    return this._angle;
	    }

        /**
         * 回転角から回転行列を計算してセットします。
         * @param i_x
         * @param i_y
         * @param i_z
         */
        public virtual void setAngle(double i_x, double i_y, double i_z)
        {
            double sina = Math.Sin(i_x);
            double cosa = Math.Cos(i_x);
            double sinb = Math.Sin(i_y);
            double cosb = Math.Cos(i_y);
            double sinc = Math.Sin(i_z);
            double cosc = Math.Cos(i_z);
            // Optimize
            double CACA = cosa * cosa;
            double SASA = sina * sina;
            double SACA = sina * cosa;
            double SASB = sina * sinb;
            double CASB = cosa * sinb;
            double SACACB = SACA * cosb;

            this.m00 = CACA * cosb * cosc + SASA * cosc + SACACB * sinc - SACA * sinc;
            this.m01 = -CACA * cosb * sinc - SASA * sinc + SACACB * cosc - SACA * cosc;
            this.m02 = CASB;
            this.m10 = SACACB * cosc - SACA * cosc + SASA * cosb * sinc + CACA * sinc;
            this.m11 = -SACACB * sinc + SACA * sinc + SASA * cosb * cosc + CACA * cosc;
            this.m12 = SASB;
            this.m20 = -CASB * cosc - SASB * sinc;
            this.m21 = CASB * sinc - SASB * cosc;
            this.m22 = cosb;
            updateAngleFromMatrix();
            return;
        }

        /**
         * 現在のMatrixからangkeを復元する。
         * @param o_angle
         */
        private void updateAngleFromMatrix()
        {
            double a, b, c;
            double sina, cosa, sinb, cosb, sinc, cosc;

            if (this.m22 > 1.0)
            {// <Optimize/>if( rot[2][2] > 1.0 ) {
                cosb = 1.0;// <Optimize/>rot[2][2] = 1.0;
            }
            else if (this.m22 < -1.0)
            {// <Optimize/>}else if( rot[2][2] < -1.0 ) {
                cosb = -1.0;// <Optimize/>rot[2][2] = -1.0;
            }
            else
            {
                cosb = this.m22;// <Optimize/>cosb = rot[2][2];
            }
            b = Math.Acos(cosb);
            sinb = Math.Sin(b);
            double rot02 = this.m02;
            double rot12 = this.m12;
            if (b >= 0.000001 || b <= -0.000001)
            {
                cosa = rot02 / sinb;// <Optimize/>cosa = rot[0][2] / sinb;
                sina = rot12 / sinb;// <Optimize/>sina = rot[1][2] / sinb;
                if (cosa > 1.0)
                {
                    cosa = 1.0;
                    sina = 0.0;
                }
                if (cosa < -1.0)
                {
                    cosa = -1.0;
                    sina = 0.0;
                }
                if (sina > 1.0)
                {
                    sina = 1.0;
                    cosa = 0.0;
                }
                if (sina < -1.0)
                {
                    sina = -1.0;
                    cosa = 0.0;
                }
                a = Math.Acos(cosa);
                if (sina < 0)
                {
                    a = -a;
                }
                double tmp = (rot02 * rot02 + rot12 * rot12);
                sinc = (this.m21 * rot02 - this.m20 * rot12) / tmp;
                cosc = -(rot02 * this.m20 + rot12 * this.m21) / tmp;

                if (cosc > 1.0)
                {
                    cosc = 1.0;
                    sinc = 0.0;
                }
                if (cosc < -1.0)
                {
                    cosc = -1.0;
                    sinc = 0.0;
                }
                if (sinc > 1.0)
                {
                    sinc = 1.0;
                    cosc = 0.0;
                }
                if (sinc < -1.0)
                {
                    sinc = -1.0;
                    cosc = 0.0;
                }
                c = Math.Acos(cosc);
                if (sinc < 0)
                {
                    c = -c;
                }
            }
            else
            {
                a = b = 0.0;
                cosa = cosb = 1.0;
                sina = sinb = 0.0;
                cosc = this.m00;//cosc = rot[0];// <Optimize/>cosc = rot[0][0];
                sinc = this.m01;//sinc = rot[1];// <Optimize/>sinc = rot[1][0];
                if (cosc > 1.0)
                {
                    cosc = 1.0;
                    sinc = 0.0;
                }
                if (cosc < -1.0)
                {
                    cosc = -1.0;
                    sinc = 0.0;
                }
                if (sinc > 1.0)
                {
                    sinc = 1.0;
                    cosc = 0.0;
                }
                if (sinc < -1.0)
                {
                    sinc = -1.0;
                    cosc = 0.0;
                }
                c = Math.Acos(cosc);
                if (sinc < 0)
                {
                    c = -c;
                }
            }
            //angleの更新
            this._angle.x = a;// wa.value=a;//*wa = a;
            this._angle.y = b;// wb.value=b;//*wb = b;
            this._angle.z = c;// wc.value=c;//*wc = c;
            return;
        }
    }
}