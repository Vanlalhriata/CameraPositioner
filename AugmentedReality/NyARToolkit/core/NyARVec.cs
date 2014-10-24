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
using System;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARVec
    {
        private int clm;

        public NyARVec(int i_clm)
        {
            v = new double[i_clm];
            clm = i_clm;
        }

        private double[] v;

        /**
         * i_clmサイズの列を格納できるように列サイズを変更します。 実行後、列の各値は不定になります。
         * 
         * @param i_clm
         */
        public void realloc(int i_clm)
        {
            if (i_clm <= this.v.Length)
            {
                // 十分な配列があれば何もしない。
            }
            else
            {
                // 不十分なら取り直す。
                v = new double[i_clm];
            }
            this.clm = i_clm;
        }

        public int getClm()
        {
            return clm;
        }

        public double[] getArray()
        {
            return v;
        }

        /**
         * arVecDispの代替品
         * 
         * @param value
         * @return
         */
        public int arVecDisp()
        {
            NyARException.trap("未チェックのパス");
            Debug.WriteLine(" === vector (" + clm + ") ===\n");// printf(" ===
            // vector (%d)
            // ===\n",
            // v->clm);
            Debug.WriteLine(" |");// printf(" |");
            for (int c = 0; c < clm; c++)
            {// for( c = 0; c < v->clm; c++ ){
                Debug.WriteLine(" " + v[c]);// printf( " %10g", v->v[c] );
            }
            Debug.WriteLine(" |");// printf(" |\n");
            Debug.WriteLine(" ===================");// printf("
            // ===================\n");
            return 0;
        }

        /**
         * arVecInnerproduct関数の代替品
         * 
         * @param x
         * @param y
         * @param i_start
         *            演算開始列(よくわからないけどarVecTridiagonalizeの呼び出し元でなんかしてる)
         * @return
         * @throws NyARException
         */
        public double vecInnerproduct(NyARVec y, int i_start)
        {
            NyARException.trap("この関数は動作確認できていません。");
            double result = 0.0;
            // double[] x_array=x.v;.getArray();
            // double[] y_array=y.getArray();

            if (this.clm != y.clm)
            {
                throw new NyARException();// exit();
            }
            for (int i = i_start; i < this.clm; i++)
            {
                NyARException.trap("未チェックのパス");
                result += this.v[i] * y.v[i];// result += x->v[i] * y->v[i];
            }
            return result;
        }

        /**
         * double arVecHousehold関数の代替品
         * 
         * @param x
         * @param i_start
         *            演算開始列(よくわからないけどarVecTridiagonalizeの呼び出し元でなんかしてる)
         * @return
         * @throws NyARException
         */
        public double vecHousehold(int i_start)
        {
            NyARException.trap("この関数は動作確認できていません。");
            double s, t;
            s = Math.Sqrt(this.vecInnerproduct(this, i_start));
            // double[] x_array=x.getArray();
            if (s != 0.0)
            {
                NyARException.trap("未チェックのパス");
                if (this.v[i_start] < 0)
                {
                    s = -s;
                }
                NyARException.trap("未チェックのパス");
                {
                    this.v[i_start] += s;// x->v[0] += s;
                    t = 1 / Math.Sqrt(this.v[i_start] * s);// t = 1 / sqrt(x->v[0] * s);
                }
                for (int i = i_start; i < this.clm; i++)
                {
                    NyARException.trap("未チェックのパス");
                    this.v[i] *= t;// x->v[i] *= t;
                }
            }
            return -s;
        }

        /**
         * 現在ラップしている配列を取り外して、新しい配列をラップします。
         * 
         * @param i_v
         * @param i_clm
         */
        public void setNewArray(double[] i_array, int i_clm)
        {
            this.v = i_array;
            this.clm = i_clm;
        }
    }

}