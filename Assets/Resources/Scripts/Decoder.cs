using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts
{
    public class Decoder
    {

        // 문자를 넣으면 숫자ㄹ 바꾸는 함수 (보조 함수)
        public static double convert_position_string_to_pos_index(string pos_str)
        {
            string str = "abcd";
            pos_str = pos_str.ToLower();
            double col = Double.Parse(pos_str[0] + "") - 1;
            double row = str.IndexOf(pos_str[1]);
            return row * 3 + col;
        }
        // 문자를 넣으면 숫자ㄹ 바꾸는 함수
        public static double encode_to_action_index(string input1, string input2, double to_play)
        {
            Dictionary<string, int> stock_kinds = new Dictionary<string, int>();
            stock_kinds.Add("E", 0);
            stock_kinds.Add("G", 1);
            stock_kinds.Add("P", 2);
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
                from_board = convert_position_string_to_pos_index(input1);
            }
            to_board = convert_position_string_to_pos_index(input2);
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
            int board_size = 12;
            double promote;
            double to_board;
            //Debug.Assert(0 <= action && action < (board_size + 3) * board_size * 2);
            promote = action % 2;
            double new_action = Math.Truncate((double)action / 2);
            to_board = new_action % board_size;
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
        // 숫자와, 보드판 상태를 넣으면 문자로 바꾸는 함수
        public static string action_to_string(double action_num, double[,] board)
        {


            List<double> move = new List<double>();
            (double, double, double, double) tu1 = decode_from_action_index(action_num);
            move.Add(tu1.Item1);
            move.Add(tu1.Item2);
            move.Add(tu1.Item3);
            move.Add(tu1.Item4);
            if (move[0] != -1)
            {
                (double, double) from_pos = (Math.Truncate((double)move[0] / 3), move[0] % 3);
                (double, double) to_pos = (Math.Truncate((double)move[2] / 3), move[2] % 3);
                double kind = board[(int)to_pos.Item1, (int)to_pos.Item2];
                string[] slist = "L E G P C".Split(' ');
                string ch = "";
                if (kind == 0)
                {
                    ch = " ";
                }
                else
                {
                    int idx = (int)(kind - 1) % 5;
                    ch = slist[idx];
                }
                string num1 = "123";
                string string1 = "abcd";
                string pos_from = num1[(int)from_pos.Item2] + "";
                pos_from += string1[(int)from_pos.Item1];
                pos_from += num1[(int)to_pos.Item2];
                pos_from += string1[(int)to_pos.Item1];
                pos_from += ch;
                return pos_from;
            }
            else
            {
                (double, double) to_pos = (Math.Truncate((double)move[2] / 3), move[2] % 3);
                string[] slist = "E G P".Split(' ');
                int idx = (int)move[1];
                string ch = slist[idx];
                string num1 = "123";
                string string1 = "abcd";
                string pos_to = num1[(int)to_pos.Item2] + "";
                pos_to += string1[(int)to_pos.Item1];
                pos_to += ch;
                return "->" + pos_to;
            }
        }

        public static (string, string) action_to_stringTuple(double action_num, int[,] board)
        {


            List<double> move = new List<double>();
            (double, double, double, double) tu1 = decode_from_action_index(action_num);
            move.Add(tu1.Item1);
            move.Add(tu1.Item2);
            move.Add(tu1.Item3);
            move.Add(tu1.Item4);
            if (move[0] != -1)
            {
                (double, double) from_pos = (Math.Truncate((double)move[0] / 3), move[0] % 3);
                (double, double) to_pos = (Math.Truncate((double)move[2] / 3), move[2] % 3);
                int kind = board[(int)to_pos.Item1, (int)to_pos.Item2];
                string[] slist = "L E G P C".Split(' ');
                string ch = "";
                if (kind == 0)
                {
                    ch = " ";
                }
                else
                {
                    int idx = (int)(kind - 1) % 5;
                    ch = slist[idx];
                }
                string num1 = "123";
                string string1 = "abcd";
                string pos_start= num1[(int)from_pos.Item2].ToString();
                pos_start += string1[(int)from_pos.Item1];

                string pos_dest = num1[(int)to_pos.Item2].ToString();
                pos_dest += string1[(int)to_pos.Item1];
                //pos_from += ch;

                return (pos_start, pos_dest);

                //return pos_from;
            }
            else
            {
                (double, double) to_pos = (Math.Truncate((double)move[2] / 3), move[2] % 3);
                string[] slist = "E G P".Split(' ');
                int idx = (int)move[1];
                string ch = slist[idx];
                string num1 = "123";
                string string1 = "abcd";
                string pos_to = num1[(int)to_pos.Item2].ToString();
                pos_to += string1[(int)to_pos.Item1];
                //pos_to += ch;
                //return "->" + pos_to;

                string pos_start = ch;
                string pos_dest = pos_to;

                return (pos_start, pos_dest);


            }
        }


        // get_observation
        public static double[,,] get_observation(double[,] board, double[,] stocks, double to_play)
        {
            double[,,] array = new double[17, 4, 3];
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
            int idx = 10;
            for (int p = 0; p < 2; p++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            array[idx, i, j] = Math.Truncate((double)stocks[p, k] / 2);
                        }
                    }
                    idx += 1;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    array[16, i, j] = 1 - (2 * to_play);
                }
            }
            return array;
        }
    }
}

