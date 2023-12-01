using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Media;
using GFXEngine;
using System.Security.Cryptography;

namespace FirstPlayable_LWM
{
    internal class Program
    {
        //Miscellaneous variables
        static bool LossConditon;
        static int MapAcross;
        static int MapDown;
        static bool AttackCollision;
        static bool WinCondition;
        static bool GameIsRunning;
        static bool FirstTurnAvailable;
        //Player variables
        static int PmHp;
        static int PHp;
        static int PlayerDmg;
        static int PlayerPosX;
        static int PlayerPosY;
        //Enemy variables
        static int EHp;
        static int EmHp;
        static int EnemyDmg;
        static int EnemyPosX;
        static int EnemyPosY;
        static bool EHasAttacked;
        static void Main(string[] args) // <- Entry point for the program
        {
            Console.ForegroundColor = ConsoleColor.Black;
            SetUp();
            while (GameIsRunning)
            {
                Console.ForegroundColor = ConsoleColor.White;
                if (FirstTurnAvailable)
                {
                    Console.Write("<Press Any Key To Start>");
                    Console.ReadKey(true);
                    Console.Clear();
                    PrintMap();
                }
                else
                {
                    PlayerTurn();
                    EnemyTurn();
                    PrintMap();
                    PrintEnemy();
                    PrintPlayer();
                    
                }
                if (PHp <= 0)
                {
                    PHp = 0;
                    LossConditon = true;
                    GameIsRunning = false;
                }
                else if (EHp <= 0)
                {
                    EHp = 0;
                    WinCondition = true;
                    GameIsRunning = false;
                }
                FirstTurnAvailable = false;
            }
            Console.Clear();
            EndScreen();
        }
        static void EndScreen() // <- Ends the game when you have lost or won
        {
            Console.SetCursorPosition(0, 5);
            if (WinCondition)
            {
                Console.Write("     !!!CONGRATULATIONS!!!");
            }
            else if (LossConditon)
            {
                Console.Write("Unfortunately You Ceased Living...");
            }
            Console.Write("\n    <Press Any Key To Exit>");
            Console.ReadKey(true);
        }
        static void SetUp() // <- starts the game 
        {
            PlayerPosX = 1;
            PlayerPosY = 1;
            PHp = 10;
            PmHp = 10;
            PlayerDmg = 1;
            EHp = 10;
            EmHp = 10;
            EnemyPosX = 9;
            EnemyPosY = 9;
            EnemyDmg = 1;
            WinCondition = false;
            AttackCollision = false;
            EHasAttacked = false;
            Console.CursorVisible = false;
            LossConditon = false;
            GameIsRunning = true;
            FirstTurnAvailable = true;
            Console.Clear();
        }
        static int PlayerInput() // <- processes player input
        {
            ConsoleKeyInfo Input = Console.ReadKey(true);
            if (Input.KeyChar == 'w')
            {
                return 1;
            }
            if (Input.KeyChar == 'a')
            {
                return 2;
            }
            if (Input.KeyChar == 's')
            {
                return 3;
            }
            if (Input.KeyChar == 'd')
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
        static void PlayerMovement() // <- processes player movement
        {
            switch (PlayerInput())
            {
                case 0:
                    ErrorMessage();
                    break;
                case 1:
                    PlayerPosition(0, -1);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(0, 1);
                    }
                    break;
                case 2:
                    PlayerPosition(-1, 0);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(1, 0);
                    }
                    break;
                case 3:
                    PlayerPosition(0, 1);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(0, -1);
                    }
                    break;
                case 4:
                    PlayerPosition(1, 0);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(-1, 0);
                    }
                    break;
            }
        }
        static void PlayerTurn() // <- Processes Player turn
        {
            PlayerMovement();
            PrintHUD();
        }
        static void ErrorMessage() // <- Displays Error message
        {
            Console.SetCursorPosition(5, 17);
            Console.Write("\nError :: Invalid Input \nError :: Press Any Key To Continue...");
            Console.SetCursorPosition(0, 0);
            Console.ReadKey(true);
            Console.Clear();
        }
        static void PlayerPosition(int Xmod, int Ymod) // <- determines player position
        {
            PlayerPosY += Ymod;
            PlayerPosX += Xmod;
            switch (MapTileCheck(PlayerPosX, PlayerPosY))
            {
                case '0':
                    break;
                case '1': 
                    PlayerPosY -= Ymod;
                    PlayerPosX -= Xmod;
                    break;
            }
            if (PlayerPosX <= 0)
            {
                PlayerPosX = 1;
            }
            else if (PlayerPosX >= MapAcross-2)
            {
                PlayerPosX = MapAcross-2;
            }
            if (PlayerPosY <= 0)
            {
                PlayerPosY = 1;
            }
            else if (PlayerPosY >= MapDown-2)
            {
                PlayerPosY = MapDown-2;
            }
            Console.SetCursorPosition(PlayerPosX, PlayerPosY);
            PrintPlayer();
        }
        static void PrintPlayer() // <- prints player character
        {
            //Console.SetCursorPosition(PlayerPosX, PlayerPosY);
            //Console.Write("T");
            GFX.GridProcGFX('2', PlayerPosX, PlayerPosY);
        }
        static void PrintEnemy() // <- Prints enemy character
        {
            //Console.SetCursorPosition(EnemyPosX, EnemyPosY);
            //Console.Write("E");
            GFX.GridProcGFX('5', EnemyPosX, EnemyPosY);
        }
        static void PrintMap() // <- prints the map to the screen
        {
            for (int i = 0; i < MapDown; i++)
            {
                for (int j = 0; j < MapAcross; j++)
                {
                    //Console.SetCursorPosition(j, i);
                    //Console.Write(MapLegend()[i][j]);
                    GFX.GridProcGFX(MapLegend()[i][j], j, i);
                }
            }
        }
        static char[][] MapLegend() // <- Processes map references
        {
            string[] mapData = LoadMap();
            int MapDimensionAcross = mapData[0].Length;
            int MapDimensionDown = mapData.Length;
            MapAcross = MapDimensionAcross;
            MapDown = MapDimensionDown;
            char[]MapDataOD = new char[MapDimensionAcross];
            char[][] MapLegendArray = new char[MapDimensionDown][];
            for (int i = 0; i < MapDimensionDown; i++)
            {
                MapLegendArray[i] = mapData[i].ToCharArray();
            }
            return MapLegendArray;
        }
        static int MapTileCheck(int PosX, int PosY) // <- Checks the map for tile value at specified location, then returns said value 
        {
            char[][] TempArray = MapLegend();
            return TempArray[PosY][PosX]; 
        }
        static string[] LoadMap() // <- loads and returns map data
        {
            string path = @"Map_Data.txt";
            string[] mapData = File.ReadAllLines(path);
            return mapData;
        }
        static void EnemyTurn() // <- processes Enemy's turn
        {
            if (EHasAttacked)
            {
                EHasAttacked = false;
            }
            else
            {
                EnemyMovement();
                PrintHUD();
            }
            
        }
        static void EnemyMovement() // <- processes enemy movement
        {
            switch (EnemyAI())
            {
                case 0: break;
                case 1: 
                    if (MapTileCheck(EnemyPosX, (EnemyPosY-1)) == '0') 
                    {
                        EnemyPosY -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp);
                        if (AttackCollision)
                        {
                            EHasAttacked = true;
                            EnemyPosY += 1;
                            AttackCollision = false;
                        }
                    }  
                    break;
                case 2: 
                    if (MapTileCheck((EnemyPosX-1), EnemyPosY) == '0') 
                    { 
                        EnemyPosX -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp);
                        if (AttackCollision)
                        {
                            EHasAttacked = true;
                            EnemyPosX += 1;
                            AttackCollision = false;
                        }
                    }  
                    break;
                case 3: 
                    if (MapTileCheck(EnemyPosX, (EnemyPosY+1)) == '0') 
                    { 
                        EnemyPosY += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp);
                        if (AttackCollision)
                        {
                            EHasAttacked = true;
                            EnemyPosY -= 1;
                            AttackCollision = false;
                        }
                    }  
                    break;
                case 4: 
                    if (MapTileCheck((EnemyPosX+1), EnemyPosY) == '0') 
                    { 
                        EnemyPosX += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp);
                        if (AttackCollision)
                        {
                            EHasAttacked = true;
                            EnemyPosX -= 1;
                            AttackCollision = false;
                        }
                    }  
                    break;
            }
        }
        static int EnemyAI() // <- Determines what the enemy will do
        {
            if (PlayerPosY < EnemyPosY)
            {
                return 1;
            }
            else if (PlayerPosX < EnemyPosX)
            {
                return 2;
            }
            else if (PlayerPosY > EnemyPosY)
            {
                return 3;
            }
            else if (PlayerPosX > EnemyPosX)
            {
                return 4;
            }
            else
            {
                return 0;
            }
            
        }
        static int CombatFunction(int atkrX, int atkrY, int dfndX, int dfndY, int atkrDmg, int dfndHp) // <- processes combat between entities
        {
            if (atkrX == dfndX && atkrY == dfndY)
            {
                AttackCollision = true;
                dfndHp -= atkrDmg;
                if (dfndHp <= 0)
                {
                    dfndHp = 0;
                }
                return dfndHp;
            }
            else
            {
                 return dfndHp;
            }
        }
        static void PrintHUD() // <- Displays HUD
        {
            Console.SetCursorPosition(5, 15);
            Console.Write("                                                    \n                                                    ");
            Console.SetCursorPosition(5, 15);
            Console.Write("Player Hp: " + PHp + "/" + PmHp + "\n" + "     " + "Enemy Hp: " + EHp + "/" + EmHp);
        }
    }
}
