using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts
{
    public class Decoder
    {
        static Dictionary<string, int> stock_kinds = new Dictionary<string, int>() { { "E", 0 }, { "G", 1 }, { "P", 2 } };
        static int board_size = 12;

        static Dictionary<string, double> PositionStrToIndex = new Dictionary<string, double>()
        {
            {"1a", 0 }, {"2a", 1 }, {"3a", 2},
            {"1b", 3 }, {"2b", 4 }, {"3b", 5},
            {"1c", 6 }, {"2c", 7 }, {"3c", 8},
            {"1d", 9 }, {"2d", 10 }, {"3d", 11},
        };

        // 문자를 넣으면 숫자ㄹ 바꾸는 함수 (보조 함수)
        //public static double convert_position_string_to_pos_index(string pos_str)
        //{
        //    pos_str = pos_str.ToLower();
        //    double col = Double.Parse(pos_str[0] + "") - 1;
        //    double row = string1.IndexOf(pos_str[1]);
        //    return row * 3 + col;
        //}
        // 문자를 넣으면 숫자ㄹ 바꾸는 함수
        public static double encode_to_action_index(string input1, string input2, double to_play)
        {
            double board_size = 12;
            double action;
            double from_stock = -1;
            double from_board = -1;
            double to_board = -1;
            string from_pos = input1.ToUpper();
            if (stock_kinds.ContainsKey(from_pos))
            {
                from_stock = stock_kinds[from_pos];
            }
            else
            {
                from_board = PositionStrToIndex[input1];
            }
            to_board = PositionStrToIndex[input2];
            if (from_stock == -1)
            {
                action = from_board;
            }
            else
            {
                action = board_size + from_stock;
            }
            action *= board_size * 2;
            action += to_board * 2;
            action += to_play;
            //Debug.Assert(0 <= action && action < (board_size + 3) * board_size * 2);
            return action;
        }
        // 숫자와, 보드판 상태를 넣으면 문자로 바꾸는 함수 (보조 함수)
        public static (double, double, double, double) decode_from_action_index(double action)
        {
            double promote = action % 2;
            double new_action = Math.Truncate((double)action / 2);
            double to_board = new_action % board_size;
            new_action = Math.Truncate((double)new_action / board_size);
            double from_board;
            double from_stock;
            if (new_action < board_size)
            {
                from_board = new_action;
                from_stock = -1;
            }
            else
            {
                from_board = -1;
                from_stock = new_action - board_size;
            }
            return (from_board, from_stock, to_board, promote);
        }

        static string[] PieceStringID_Five = new string[5] { "L", "E", "G", "P","C"};
        static string[] PieceStringID_Three = new string[3] { "E", "G", "P" };
        public static (string, string) action_to_stringTuple(double action_num)
        {


            //List<double> move = new List<double>();
            (double i0, double i1, double i2, double i3) move = decode_from_action_index(action_num);
            //move.Add(tu1.Item1);
            //move.Add(tu1.Item2);
            //move.Add(tu1.Item3);
            //move.Add(tu1.Item4);
            if (move.i0 != -1)
            {
                (double, double) from_pos = (Math.Truncate((double)move.i0 / 3), move.i0 % 3);
                (double, double) to_pos = (Math.Truncate((double)move.i2 / 3), move.i2 % 3);

                //string pos_start= num1[(int)from_pos.Item2].ToString();
                //pos_start += string1[(int)from_pos.Item1];
                int from_y = (int)from_pos.Item1;
                int from_x = (int)from_pos.Item2;


                string pos_start = GameManager.intPosToStringPos[(from_y, from_x)];

                //string pos_dest = num1[(int)to_pos.Item2].ToString();
                //pos_dest += string1[(int)to_pos.Item1];

                int to_y = (int)to_pos.Item1;
                int to_x = (int)to_pos.Item2;

                string pos_dest = GameManager.intPosToStringPos[(to_y, to_x)];

                return (pos_start, pos_dest);
            }
            else
            {
                (double, double) to_pos = (Math.Truncate((double)move.i2 / 3), move.i2 % 3);
                //string[] slist = "E G P".Split(' ');
                int idx = (int)move.i1;
                string ch = PieceStringID_Three[idx];
                //string pos_to = num1[(int)to_pos.Item2].ToString();
                //pos_to += string1[(int)to_pos.Item1];

                string pos_to = GameManager.intPosToStringPos[((int)to_pos.Item1, (int)to_pos.Item2)];

                //string pos_start = ch;
                //string pos_dest = pos_to;

                return (ch, pos_to);


            }
        }


        // get_observation
        public static int[,,] get_observation(int[,] board, int[,] stocks)
        {
            int[,,] array = new int[16, 4, 3];
            for (int k = 0; k < 10; k++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] == k + 1)
                        {
                            array[k, i, j] = 1;
                        }
                        else
                        {
                            array[k, i, j] = 0;
                        }
                    }
                }
            }
            //0 코끼리, 1 기린, 2 병아리
            int idx = 10;
            for (int p = 0; p < 2; p++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            array[idx, i, j] = stocks[p, k];
                        }
                    }
                    idx += 1;
                }
            }
            //for (int i = 0; i < 4; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        array[16, i, j] = 1 - (2 * to_play);
            //    }
            //}
            return array;
        }
    }
}

