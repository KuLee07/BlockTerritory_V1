using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockTerritory
{
    internal class Dimension
    {
        setting setting = new setting();

        //升高維度
        public string ActionToBoard(int action)
        {

            int i = (action / 2) % setting.BoardSizeXY; //X
            int j = action / (2 * setting.BoardSizeXY); //Y
            int k = action % 2; //Z

            return i.ToString() + ";" + j.ToString() + ";" + k.ToString();
        }

        //降低維度
        public string BoardToAction(int i, int j, int k)
        {
            int action = i * 2 + j * setting.BoardSizeXY * 2 + k;

            return action.ToString();
        }


    }
}
