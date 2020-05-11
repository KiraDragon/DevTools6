using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO; 
using System.Media; 

namespace Snake
{
  //Creates a data structure position that is made out of a row and a column. 
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    //Introduces enums that control gamestate
    public enum GameState
    {
		InMainMenu,
		Start,
		Help, 
		ScorePage
    }

      public enum Level
    {
		One,
        Two,
        Three,
        End
    }

    class Program
    {
///////////////////////////////////////////////METHODS/////////////////////////////////////////////////////
        static public void SaveScore(string username, int score)
        {
            StreamWriter sw = new StreamWriter("../../scoreboard.txt", true); 
            string entry; 
            entry = "{0}: {1} points"; 
            entry = string.Format(entry, username, score.ToString()); 
            sw.WriteLine(entry);
            sw.Close(); 
        }

        static public void PlayMusic()
        {
            System.Media.SoundPlayer bgm = new System.Media.SoundPlayer(); 
            bgm.SoundLocation = "../../royaltyfreebgm.wav";
            bgm.PlayLooping(); 
        }
        
        static public List<Position> MakeObstacles(Random rng)
        {
            //intialise the position of first 5 obstacle
            List<Position> obstacles = new List<Position>();
            for(int i = 0; i < 5; i++)
            {
                obstacles.Add(new Position(rng.Next(0, Console.WindowHeight),
                            rng.Next(0, Console.WindowWidth))); 
            };

			//produce obstacles item on certain position with Cyan coloured "="
            foreach (Position obstacle in obstacles)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                Console.Write("=");
            }

            return obstacles; 
        }

        static public void MakeNewObstacles(Random rng, List<Position> obstacles, Position food, Queue<Position> snakeElements)
        {
            //Creates a new Position named obstacle
                    Position obstacle = new Position();
                    do
                    {
                        //When the snake collides with the food, it sets obstacle to a new random position that does not contain the snake, the obstacle or food
                        obstacle = new Position(rng.Next(0, Console.WindowHeight),
                            rng.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row));

                    //Adds the obstacle into an array of obstacles
                    obstacles.Add(obstacle);
                    //Sets the cursor position the the obstacles column and row
                    Console.SetCursorPosition(obstacle.col, obstacle.row);
                    //Sets the color of the obstacle
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    //Writes = in the console as visualisation of the obstacle
                    Console.Write("=");
        }

        static public Position MakeFood(Random rng, List<Position> obstacles, Queue<Position> snakeElements, int determiner)
        {
            Position food;
            do
            {
                food = new Position(rng.Next(0, Console.WindowHeight),
                    rng.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));
            switch(determiner)
            {
                case 0:     
                     Console.SetCursorPosition(food.col, food.row);
                     Console.ForegroundColor = ConsoleColor.Yellow;
                     Console.Write("@");
                     break; 
                case 1:
                    Console.SetCursorPosition(food.col, food.row);
                     Console.ForegroundColor = ConsoleColor.DarkRed;
                     Console.Write("@");
                     break;
                case 2:
                    Console.SetCursorPosition(food.col, food.row);
                     Console.ForegroundColor = ConsoleColor.Green;
                     Console.Write("@");
                     break;
                default:
                    Console.SetCursorPosition(food.col, food.row);
                     Console.ForegroundColor = ConsoleColor.DarkBlue;
                     Console.Write("@");
                    break; 
            }

            return food; 
        }

        static public Level WinCondition(string username, int userpoints, Level level)
        {
                if(level == Level.One && userpoints >= 100)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    string youwin = "PASS LEVEL 1!";
                    string _continue = "Press Enter to Continue";
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n");
                    Console.Write(new string(' ', (Console.WindowWidth - youwin.Length) / 2));
                    Console.WriteLine(youwin);
                    Console.Write(new string(' ', (Console.WindowWidth - _continue.Length) / 2));
                    Console.WriteLine(_continue); 
                    Console.ReadLine();
                    return Level.Two; 
                }
                else if(level == Level.Two && userpoints >= 200)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    string youwin = "PASS LEVEL 2!";
                    string _continue = "Press Enter to Continue";
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n");
                    Console.Write(new string(' ', (Console.WindowWidth - youwin.Length) / 2));
                    Console.WriteLine(youwin);
                    Console.Write(new string(' ', (Console.WindowWidth - _continue.Length) / 2));
                    Console.WriteLine(_continue); 
                    Console.ReadLine();
                    return Level.Three; 
                }
                // When the user gets 1000 points, the user would win
                else if (level == Level.Three && userpoints >= 300)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    string youwin = "PASS LEVEL 3! YOU WIN!";
                    string _continue = "Press Enter to Continue";
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n");
                    Console.Write(new string(' ', (Console.WindowWidth - youwin.Length) / 2));
                    Console.WriteLine(youwin);
                    Console.Write(new string(' ', (Console.WindowWidth - _continue.Length) / 2));
                    Console.WriteLine(_continue); 
                    SaveScore(username, userpoints); 
                    Console.ReadLine();
                    return Level.End; 
                }
                else
                {
                    return level; 
                }
        }

        // When snake use up 3 lives, console will show "Game over!" and the total amount of points gathered
        static public bool RealGameOver(List<Position> obstacles, Queue<Position> snakeElements, Position snakeNewHead, int userpoints, string username)
        {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                string gameover = "Game over!";
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n");
                Console.Write(new string(' ', (Console.WindowWidth - gameover.Length) / 2));
                Console.WriteLine(gameover);
                if (userpoints < 0) userpoints = 0;
                userpoints = Math.Max(userpoints, 0);
                SaveScore(username, userpoints); 
                string finalPoints = "Your points are: {0}";
                Console.Write(new string(' ', (Console.WindowWidth - finalPoints.Length) / 2));
                Console.WriteLine(finalPoints, userpoints);
                Console.ReadLine(); 
                return false;
        }

        //Show the player how many lives that snake reamin
        static public void Lives(int lives)
        {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                string live = "Remain lives: " + lives;
                Console.WriteLine("\n\n");
                //Console.Write(new string(' ', (Console.WindowWidth - live.Length) / 2));
                Console.WriteLine(live);
                //return true;
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////











        static void Main(string[] args)
        {

            PlayMusic(); 
  			//intialise different variable
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int foodDissapearTime = 16000;
            int negativePoints = 0;
            string userName;
            Console.WriteLine("Enter your name: ");
            userName = Console.ReadLine();
            Console.Clear(); 
            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };
            GameState state = GameState.InMainMenu; 
            Level level = Level.One; 
            double sleepTime = 100;
            int direction = right;
            Random randomNumbersGenerator = new Random();
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;
            int lives = 3;
            while(true)
            {
                if(state == GameState.Start)
                {
                    Console.Clear(); 
                    List<Position> obstacles = MakeObstacles(randomNumbersGenerator); 

			        //Declare a new variable snakeElements
                    Queue<Position> snakeElements = new Queue<Position>();
			        //Initialise the length of the snake
                    for (int i = 0; i <= 3; i++)
                    {
                        snakeElements.Enqueue(new Position(0, i));
                    }
            
                    // Creates new food at a random position (as long as the position has not been taken by an obstacles or the snake)
            
                    Position food = MakeFood(randomNumbersGenerator, obstacles, snakeElements, 0); 

                    // Redraws the snake
                    foreach (Position position in snakeElements)
                    {
                        Console.SetCursorPosition(position.col, position.row);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("*");
                    }

                    int userPoints = 0;
                    int time = 0;
                    int seconds = 15;
                    bool mainloop = true; 
                    // Loops the game till it ends
                    while (mainloop)
                    {
                
                        int determiner = randomNumbersGenerator.Next(3);
                        userPoints = (snakeElements.Count - 4) * 100 - negativePoints;
                        Console.SetCursorPosition(0, 0);
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        Console.WriteLine("Score: " + userPoints + " ");
                        Lives(lives);

                        time++;
                        if (time % 10 == 0)
                        {
                            seconds--;
                            if (seconds == -1)
                            {
                                seconds = 15;
                            }
                        }

                        Console.SetCursorPosition(0, 0);
                        Console.ForegroundColor = ConsoleColor.Green;
                        string thetimeTime = "Food time: ";
                        Console.WriteLine("\n");
                        Console.Write(new string(' ', (Console.WindowWidth - thetimeTime.Length) / 2));
                        Console.WriteLine(thetimeTime + seconds + " ");

                

                        level = WinCondition(userName, userPoints, level); 

                        // Increment 1
                        negativePoints++;

                        // Detects key inputs to change direction 
                        if (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo userInput = Console.ReadKey();
                            if (userInput.Key == ConsoleKey.LeftArrow)
                            {
                                if (direction != right) direction = left;
                            }
                            if (userInput.Key == ConsoleKey.RightArrow)
                            {
                                if (direction != left) direction = right;
                            }
                            if (userInput.Key == ConsoleKey.UpArrow)
                            {
                                if (direction != down) direction = up;
                            }
                            if (userInput.Key == ConsoleKey.DownArrow)
                            {
                                if (direction != up) direction = down;
                            }
                        }

                        Position snakeHead = snakeElements.Last();  // Sets the last element of the snake as the snake head 
                        Position nextDirection = directions[direction]; // Sets the direction the snake is moving

                        Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                            snakeHead.col + nextDirection.col); // Sets the new position of snake head based on the snake's direction 
                
                        // Makes the snake come out from the other side of the window when it passes through the edge of the window 
                        if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                        if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                        if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                        if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                        //Replace mainloop = GameOver(obstacles, snakeElements, snakeNewHead, userPoints, userName) with below code; 
                        if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                        {
                            //int lives = 3;
                            lives = lives - 1;
                            if (lives <= 0)
                            {
                                mainloop =  RealGameOver(obstacles, snakeElements, snakeNewHead, userPoints, userName);
                                state = GameState.InMainMenu; 
                            }
                            else
                            {
                                Lives(lives);
                                mainloop = true;
                            }
                        }
                        else
                        {
                            mainloop = true; 
                        }

                        // Draws the snake's first body after the snake head in every frame
                        Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("*");

                        // Adds the snake head into the snake element
                        snakeElements.Enqueue(snakeNewHead);

                        // Draws the snake head depending on which way the snake is facing
                        Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        if (direction == right) Console.Write(">");
                        if (direction == left) Console.Write("<");
                        if (direction == up) Console.Write("^");
                        if (direction == down) Console.Write("v");
                
                        if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                        {
                            Console.Beep();  
                            //If the snake's head intercepts the location of the food
                            // feeding the snake
                            food = MakeFood(randomNumbersGenerator, obstacles, snakeElements, determiner);

                            if(determiner == 0)
                            {
                                userPoints += 50;
                                seconds = 15;
                            }

                             if(determiner == 1)
                            {
                                userPoints += 100;
                                seconds = 15;
                            }

                             if(determiner == 2)
                            {
                                userPoints += 150;
                                seconds = 15;
                            }
                            lastFoodTime = Environment.TickCount;
                            sleepTime--;

                            MakeNewObstacles(randomNumbersGenerator, obstacles, food, snakeElements); 
                        }
                        else
                        {
                            // moving...
                            //Removes the last position of the snake by writing a space, pushing the snake forwards
                            Position last = snakeElements.Dequeue();
                            Console.SetCursorPosition(last.col, last.row);
                            Console.Write(" ");
                        }

                        if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                        {
                            //If the time limit has expired, Negative points are increased by 50 
                            negativePoints = negativePoints + 50;
                            //Sets the cursor position to where the food was and deletes it by writing a space over it
                            Console.SetCursorPosition(food.col, food.row);
                            Console.Write(" ");
                            food = MakeFood(randomNumbersGenerator, obstacles, snakeElements, determiner);
                            //Resets the timer
                            lastFoodTime = Environment.TickCount;
                        }

                        //Sets the cursor position to the new column and row with the food
                        Console.SetCursorPosition(food.col - 1, food.row);

                        sleepTime -= 0.01;

                        Thread.Sleep((int)sleepTime);

                        if(level == Level.End)
                        {
                            mainloop = false; 
                            state = GameState.InMainMenu; 
                        }
                    }
                }
                else if(state == GameState.InMainMenu)
                {
                   Console.Clear(); 
                   Console.ForegroundColor = ConsoleColor.Red;
                   Console.WriteLine("\n <*** S N A K E ");
                   Console.ForegroundColor = ConsoleColor.Green;
                   Console.WriteLine("\n - Start");
                   Console.WriteLine("\n - Scoreboard");
                   Console.WriteLine("\n - Help");
                   Console.WriteLine("\n - Quit");
                   Console.WriteLine(" ");
                   Console.WriteLine("Your selection: ");
                   string selection = Console.ReadLine().ToLower(); 
                   if(selection == "start")
                   {
                       state = GameState.Start; 
                       Console.Clear(); 
                   }
                   else if(selection == "help")
                   {
                       Console.Clear(); 
                       state = GameState.Help; 
                   }
                   else if(selection == "scoreboard")
                   {
                       Console.Clear(); 
                       state = GameState.ScorePage; 
                   }
                   else if(selection == "quit")
                   {
                        break; 
                   }
                   else
                   {
                       Console.WriteLine("Invalid Response");
                   }
                }
                else if(state == GameState.Help)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("In this game, you play as a snake ( ***> )."); 
                    Console.WriteLine(" "); 
                    Console.WriteLine("Your goal is to eat the food ( @ ) before they disappear to grow in length");
                    Console.WriteLine(" ");
                    Console.WriteLine("Dodge obstacles ( = ) and do not hit yourself or you'll lose a life, once your lives runs out, you lose.");
                    Console.WriteLine(" ");
                    Console.WriteLine("The score is added each time a new food is eaten, and the final score is shown at game over");
                    Console.WriteLine(" ");
                    Console.WriteLine("Good luck!");
                    Console.WriteLine(" ");
                    Console.WriteLine("Type back to go back to main menu...");
                    string selection = Console.ReadLine().ToLower(); 
                    if(selection == "back")
                    {
                       Console.Clear(); 
                       state = GameState.InMainMenu; 
                    }
                    else
                    {
                       Console.WriteLine("Invalid Response");
                    }
                }
                else if(state == GameState.ScorePage)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Scoreboard"); 
                    Console.WriteLine(); 
                    string[] lines = File.ReadAllLines("../../scoreboard.txt", Encoding.UTF8);
                    foreach(string line in lines)
                    {
                        Console.WriteLine(line); 
                    }
                    Console.WriteLine("Type back to go back to main menu...");
                    string selection = Console.ReadLine().ToLower(); 
                    if(selection == "back")
                    {
                       Console.Clear(); 
                       state = GameState.InMainMenu; 
                    }
                    else
                    {
                       Console.WriteLine("Invalid Response");
                    }
                }
            }
        }
    }
}
