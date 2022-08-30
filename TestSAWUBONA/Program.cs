using System;
using System.Text.RegularExpressions;
using TestSAWUBONA;

namespace SawubonaTest // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private static bool IsAllCorrect = false;

        private static string mapCoordinates;
        private static string[] route;
        private static string startPosition;

        private static int CurrentXPosition;
        private static int CurrentYPosition;

        private static Robot robot = new Robot();

        static void Main(string[] args)
        {

            //********* Inputs for test **********
            //string input = "M:-10,10,-10,10;S:-5,5;[W5,E5,N4,E3,S2,W1]";
            //string input = "[W1,N1,E1,E1,S1,S1,W1,W1,N1,E1];S:0,0;M:-1,1,-1,1";
            //string input = "S:0,0;[N1,N1,N1,N1,N1,S1,S1,S1,S1,S1];M:0,0,0,5";
            // *********************************************

            Console.WriteLine("***********\\\\\\ SAWUBONA Test ////***********\nA prototype of a robot scheduling software.\n********************************************\nPlease insert a coordinate :");

            string input = Console.ReadLine();

            checkInputParts(input);
            if (IsAllCorrect)
            {
                separateInput(input);
                getCurrentPositions();
                moveTheRobot(route, mapCoordinates);
                //getMapCoordinates(mapCoordinates);
            }
        }

        private static void getCurrentPositions()
        {
            CurrentXPosition = int.Parse(startPosition.Split(',')[0]);
            CurrentYPosition = int.Parse(startPosition.Split(',')[1]);
            robot.CurrentPonit = $"{CurrentXPosition},{CurrentYPosition}";
        }

        private static void printReports()
        {
            printAllPositionsCleaned();
            printUniquePositionsCleaned();
        }

        private static void checkInputParts(string input)
        {
            Regex reg = new Regex(@"(^[M][:](-?\d{1,2}[,]){3}-?\d+$)|(^[S][:]-?\d+[,]-?\d+$)|(^[\[]([A-Z]-?\d+[,])+[A-Z]-?\d+[\]]$)");
            if (checkInputParts(input, ';', 2))
            {
                string[] newInput = input.Split(';');
                for (int i = 0; i < newInput.Length; i++)
                {
                    if (reg.IsMatch(newInput[i]))
                    {
                        IsAllCorrect = true;
                    }
                    else
                    {
                        IsAllCorrect = false;
                        Console.WriteLine($"Input is not correct in Part {newInput[i]}");
                        break;
                    }
                }
            }
            else
            {
                IsAllCorrect = false;
                Console.WriteLine("The input should be only 3 parts");
            }
        }
        private static void printAllPositionsCleaned()
        {
            string result = robot.RaportAllPosition.Aggregate((x, y) => x + ';' + y);
            Console.WriteLine($"\nAll Positions Cleaned: {robot.RaportAllPosition.Count} cells.\n{result}");
        }
        private static void printUniquePositionsCleaned()
        {
            robot.RaportUniquePosition = robot.RaportAllPosition.Distinct().ToList();
            string result = robot.RaportUniquePosition.Aggregate((x, y) => x + ';' + y);
            Console.WriteLine($"\nUnique Positions Cleaned: {robot.RaportUniquePosition.Count} cells.\n{result}\n");
        }

        private static void moveTheRobot(string[] route, string mapCoordinates)
        {
            bool isOutOfRang = false;
            int[] mapToInts = mapCoordinates.Split(',').Select(int.Parse).ToArray();
            checkStartPosition();

            robot.StartPonit = startPosition;

            robot.RaportAllPosition.Add(startPosition);

            foreach (var item in route)
            {

                int stepNum = int.Parse(item.Substring(1));
                switch (item.Substring(0, 1))
                {
                    case "W":

                        for (int i = 1; i < stepNum + 1; i++)
                        {
                            CurrentXPosition--;
                            if (CurrentXPosition >= mapToInts[0])
                            {
                                robot.CurrentPonit = String.Format($"{CurrentXPosition},{CurrentYPosition}");
                                robot.RaportAllPosition.Add(robot.CurrentPonit);
                            }
                            else
                            {
                                Console.Error.WriteLine($"The robot stopped... it was going out of the area from the WEST, Robot current position is ({robot.CurrentPonit}).\nPlease check the input :)");
                                printReports();
                                isOutOfRang = true;
                                break;
                            }
                        }

                        break;

                    case "E":

                        for (int i = 1; i < stepNum + 1; i++)
                        {
                            CurrentXPosition++;
                            if (CurrentXPosition <= mapToInts[1])
                            {
                                robot.CurrentPonit = String.Format($"{CurrentXPosition},{CurrentYPosition}");
                                robot.RaportAllPosition.Add(robot.CurrentPonit);
                            }
                            else
                            {
                                Console.Error.WriteLine($"The robot is outside of Map from the EAST, Robot current position is ({robot.CurrentPonit})");
                                printReports();
                                isOutOfRang = true;
                                break;
                            }
                        }
                        break;

                    case "N":

                        for (int i = 1; i < stepNum + 1; i++)
                        {
                            CurrentYPosition++;
                            if (CurrentYPosition >= mapToInts[2] && CurrentYPosition <= mapToInts[3])
                            {
                                robot.CurrentPonit = String.Format($"{CurrentXPosition},{CurrentYPosition}");
                                robot.RaportAllPosition.Add(robot.CurrentPonit);

                            }
                            else
                            {
                                Console.Error.WriteLine($"The robot is outside of Map from the NORTH, Robot current position is ({robot.CurrentPonit})");
                                printReports();
                                isOutOfRang = true;
                                break;
                            }
                        }

                        break;
                    case "S":

                        for (int i = 1; i < stepNum + 1; i++)
                        {
                            CurrentYPosition--;
                            if (CurrentYPosition >= (mapToInts[3] * -1))
                            {
                                robot.CurrentPonit = String.Format($"{CurrentXPosition},{CurrentYPosition}");
                                robot.RaportAllPosition.Add(robot.CurrentPonit);
                            }
                            else
                            {
                                Console.Error.WriteLine($"The robot is outside of Map from the SOUTH, Robot current position is ({robot.CurrentPonit})");
                                printReports();
                                isOutOfRang = true;
                                break;
                            }
                        }

                        break;
                    default:
                        Console.Error.WriteLine($"This is error input: {item}");
                        IsAllCorrect = false;
                        break;
                }
                if (isOutOfRang)
                    break;
            }
            if (!isOutOfRang)
                printReports();
        }

        private static bool checkStartPosition()
        {
            bool isInOfRang = true;
            int[] mapToInts = mapCoordinates.Split(',').Select(int.Parse).ToArray();

            string[] startInputs = startPosition.Split(',');

            int startX = int.Parse(startInputs[0]);
            int startY = int.Parse(startInputs[1]);

            if (startX < 0)
            {
                if (Math.Abs(startX) > Math.Abs(mapToInts[0]))
                {
                    Console.WriteLine($"Check your input, the start position {startX} is out of rang .");
                    isInOfRang = false;
                }
            }
            else
            {
                if (startX > mapToInts[1])
                {
                    Console.WriteLine($"Check your input, the start position {startX} is out of rang .");
                    isInOfRang = false;
                }
            }
            if (startY < 0)
            {
                if (Math.Abs(startY) > Math.Abs(mapToInts[2]))
                {
                    Console.WriteLine($"Check your input, the start position {startY} is out of rang .");
                    isInOfRang = false;
                }
            }
            else
            {
                if (startY > mapToInts[3])
                {
                    Console.WriteLine($"Check your input, the start position {startY} is out of rang .");
                    isInOfRang = false;
                }
            }

            return isInOfRang;
        }

        private static bool checkInputParts(string input, char separate, int count)
        {
            bool isTowPart = false;
            int commaNumber = 0;
            foreach (var ch in input)
            {
                if (ch == separate)
                {
                    commaNumber++;
                }
                if (commaNumber == count)
                {
                    isTowPart = true;
                }
                else
                {
                    isTowPart = false;
                }
            }
            return isTowPart;
        }

        private static void separateInput(string input)
        {
            string[] splitedInput = input.Split(';');

            foreach (var part in splitedInput)
            {
                if (part[0] == 'M' && part[1] == ':')
                {
                    mapCoordinates = part.Substring(2, part.Length - 2);
                }
                else if (part[0] == 'S' && part[1] == ':')
                {
                    startPosition = part.Substring(2, part.Length - 2);
                }
                else if (part[0] == '[' && part[part.Length - 1] == ']')
                {
                    route = part.Substring(1, part.Length - 2).Split(',');
                }
            }
            if (checkStartPosition())
            {
                IsAllCorrect = true;
            }
            else
            {
                IsAllCorrect = false;
            }
        }

        private static void getMapCoordinates(string mapCoordinates)
        {
            string[] mapCoor = mapCoordinates.Split(',');

            int zeroPointX = 0;
            int zeroPointY = 0;

            int minX = int.Parse(mapCoor[0]);
            int maxX = int.Parse(mapCoor[1]);
            int minY = int.Parse(mapCoor[2]);
            int maxY = int.Parse(mapCoor[3]);

            int x = (Math.Abs(minX) + maxX) + 1;
            int y = (Math.Abs(minY) + maxY) + 1;

            if (x == y)
            {
                zeroPointX = (x - 1) / 2;
                zeroPointY = (y - 1) / 2;
                Console.WriteLine($"Zero X Point {zeroPointX} and Zero point Y In the Array{zeroPointY}");
            }
            creatMap(x, y, zeroPointX, zeroPointY);
            Console.WriteLine($"Number of cells {x * y}");

        }

        private static void creatMap(int x, int y, int zeroPointX, int zeroPointY)
        {
            string[,] map = new string[x, y];
            //Set 0,0 in the meddle of map
            map[zeroPointX, zeroPointY] = "0,0";

            int counter = 10;
            for (int h = 0; h < (x - 1); h++)
            {
                for (int v = 0; v < (y - 1); v++)
                {
                    if (h < 10)
                    {
                        if (v < 10)
                        {
                            map[h, v] = string.Format($"-{counter - h},{counter - v}");
                        }
                        else
                        {
                            map[h, v] = string.Format($"-{counter - h},{v - 10}");
                        }
                    }
                    else
                    {
                        if (v < 10)
                        {
                            map[h, v] = string.Format($"{h - 10},{-v}");

                        }
                        map[h, v] = string.Format($"{h - 10},{v - 10}");

                    }
                }
            }


        }
    }
}