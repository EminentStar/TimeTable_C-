using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace TimeTable.Useful_Functions
{
    public class JKAppExceptions
    {
        private static JKAppExceptions jkAppExceptions = new JKAppExceptions();

        private JKAppExceptions()
        {

        }
        public static JKAppExceptions GetInstance()
        {
            return jkAppExceptions;
        }

        public string KoreanPadLeft(string paramStr, int totalLength, char padChar)
        {
            string stringValue = null;
            int byteCount = GetByteCountOfString(paramStr);
            int changedLength = totalLength - byteCount / 2;

            stringValue = paramStr.PadLeft(changedLength, padChar);
            return stringValue;
        }
        public string BytesPadLeft(string paramStr, int totalLength, char padChar)
        {
            string returnStr = null;
            int bytesCount = System.Text.Encoding.Default.GetByteCount(paramStr);
            int spaceCount = totalLength - bytesCount;

            for (int i = 0; i < spaceCount; i++)
            {
                returnStr += padChar;
            }
            returnStr += paramStr;

            return returnStr;
        }
        public int GetByteCountOfString(string paramStr)
        {
            return System.Text.Encoding.Default.GetByteCount(paramStr);
        }

        public bool AskReinputOrGoBack()
        {
            /*******************USAGE***********************
               if (jkAppExceptions.AskReinputOrGoBack())
                        continue;
                    else
                        break;
             ***********************************************/
            Console.Write("재입력은 1, 뒤로가시려면 아무키나 입력 : ");
            return Console.ReadLine().Equals("1");
        }

        public int GetDigit()
        {
            int input_integer = 0;
            string input_string;
            byte[] asciibyte = null;

            while (true)
            {
                input_string = Console.ReadLine();
                asciibyte = Encoding.ASCII.GetBytes(input_string);

                if ((asciibyte.Length == 1) && (asciibyte[0] >= 48 && asciibyte[0] <= 57))
                {
                    input_integer = Convert.ToInt32(input_string);
                    break;
                }
                else
                {
                    Console.Write("잘못된 입력입니다. 다시 입력해주세요. : ");
                    continue;
                }
            }
            return input_integer;
        }

        public int GetNumber()
        {
            byte[] asciibyte = null;
            int input_integer = 0, count = 0;
            string input_string;
            bool isDigit = false;

            while (true)
            {

                input_string = Console.ReadLine();
                isDigit = false;

                asciibyte = Encoding.ASCII.GetBytes(input_string);

                if (asciibyte.Length == 0)
                {
                    Console.Write("잘못된 입력입니다. 다시 입력해주세요. : ");
                    continue;
                }
                else
                {
                    for (count = 0; count < asciibyte.Length; count++)
                    {
                        if (asciibyte[count] < 48 || asciibyte[count] > 57)
                        {
                            isDigit = true;
                        }
                    }
                }

                if (isDigit)
                {
                    Console.Write("(숫자만 입력가능) : ");
                    continue;
                }
                else
                {
                    input_integer = Convert.ToInt32(input_string);
                    break;
                }
            }
            return input_integer;
        }

    }
}
