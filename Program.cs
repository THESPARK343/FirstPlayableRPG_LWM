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
using System.Runtime.Remoting.Messaging;

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
        static string LogMSG;
        //Player variables
        static int PmHp;
        static int PHp;
        static int PlayerDmg;
        static int PlayerPosX;
        static int PlayerPosY;
        static int PGold;
        //Enemy one variables
        static int EHp;
        static int EmHp;
        static int EnemyDmg;
        static int EnemyPosX;
        static int EnemyPosY;
        static bool EHasAttacked;
        //EnemyTwo variables
        static int E2Hp;
        static int E2mHp;
        static int Enemy2Dmg;
        static int Enemy2PosX;
        static int Enemy2PosY;
        static bool E2HasAttacked;
        //Pickup variables
        static bool Pickup01;//1
        static int Pickup01X;//1 <- Pickup 01 variables
        static int Pickup01Y;//1
        static bool Pickup02;//2
        static int Pickup02X;//2 <- Pickup 02 variables
        static int Pickup02Y;//2
        static bool Pickup03;//3
        static int Pickup03X;//3 <- Pickup 03 variables
        static int Pickup03Y;//3
        static bool Pickup04;//4
        static int Pickup04X;//4 <- Pickup 04 variables
        static int Pickup04Y;//4
        static bool Pickup05;//5
        static int Pickup05X;//5 <- Pickup 05 variables
        static int Pickup05Y;//5
        static void Main(string[] args) // <- Entry point for the program 
        {
            //Console.ForegroundColor = ConsoleColor.Black;
            SetUp();
            while (GameIsRunning)
            {
                //Console.ForegroundColor = ConsoleColor.White;
                if (FirstTurnAvailable)
                {
                    Console.Write("<Press Any Key To Start>");
                    Console.ReadKey(true);
                    Console.Clear();
                    PrintMap();
                    PrintPlayer();
                    PrintEnemy();
                    PrintHUD();
                    MusicPlayer("boodeep01.wav");
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
                else if (PGold >= 5)
                {
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
            //player
            PlayerPosX = 1;
            PlayerPosY = 1;
            PHp = 10;
            PmHp = 10;
            PlayerDmg = 1;
            //enemy one
            EHp = 5;
            EmHp = 5;
            EnemyPosX = 9;
            EnemyPosY = 9;
            EnemyDmg = 1;
            //enemy two
            E2Hp = 10;
            E2mHp = 10;
            Enemy2PosX = 5;
            Enemy2PosY = 5;
            Enemy2Dmg = 1;
            //gold and pickups
            PGold = 0;
            Pickup01 = true;
            Pickup02 = true;
            Pickup03 = true;
            Pickup04 = true;
            Pickup05 = true;
            //conditions
            WinCondition = false;
            AttackCollision = false;
            EHasAttacked = false;
            E2HasAttacked = false;
            Console.CursorVisible = false;
            LossConditon = false;
            GameIsRunning = true;
            FirstTurnAvailable = true;
            //initialized
            LogMSG = "Adventure Started...";
            MapLegend();
            Console.Clear();
            
            
        }
        static void MusicPlayer(string M) // <- plays the background music
        {
            SoundPlayer BGM = new SoundPlayer();
            BGM.SoundLocation = M;
            BGM.PlayLooping();
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
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp, 0);
                    E2Hp = CombatFunction(PlayerPosX, PlayerPosY, Enemy2PosX, Enemy2PosY, PlayerDmg, E2Hp, 0);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(0, 1);
                        //LogMSG = "You Are Engaged In Combat!";
                    }
                    break;
                case 2:
                    PlayerPosition(-1, 0);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp, 0);
                    E2Hp = CombatFunction(PlayerPosX, PlayerPosY, Enemy2PosX, Enemy2PosY, PlayerDmg, E2Hp, 0);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(1, 0);
                        //LogMSG = "You Are Engaged In Combat!";
                    }
                    break;
                case 3:
                    PlayerPosition(0, 1);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp, 0);
                    E2Hp = CombatFunction(PlayerPosX, PlayerPosY, Enemy2PosX, Enemy2PosY, PlayerDmg, E2Hp, 0);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(0, -1);
                        //LogMSG = "You Are Engaged In Combat!";
                    }
                    break;
                case 4:
                    PlayerPosition(1, 0);
                    EHp = CombatFunction(PlayerPosX, PlayerPosY, EnemyPosX, EnemyPosY, PlayerDmg, EHp, 0);
                    E2Hp = CombatFunction(PlayerPosX, PlayerPosY, Enemy2PosX, Enemy2PosY, PlayerDmg, E2Hp, 0);
                    if (AttackCollision)
                    {
                        AttackCollision = false;
                        PlayerPosition(-1, 0);
                        //LogMSG = "You Are Engaged In Combat!";
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
                case '0': //<-- floor detection
                    LogMSG = "No Report.";
                    break;
                case '7': //<-- Wall detection
                    PlayerPosY -= Ymod;
                    PlayerPosX -= Xmod;
                    LogMSG = "It's A Wall...";
                    break;
                case '3': //<-- Health pool detection
                    if (PHp < PmHp)
                    {
                        PHp = PHp + 1;
                        LogMSG = "The Health Pool Rejuvenates You...";
                    }
                    break;
                case '1': //<-- Safe Area detection
                    LogMSG = "In Safe Area.";
                    break;
                case '8': //<-- Hazard Area Detection
                    PHp = PHp - 1;
                    LogMSG = "The Poison Saps Your Strength...";
                    break;
                case '4':
                    //PGold = PGold + 1;
                    //Pickup01 = false;
                    GoldCheck(PlayerPosX, PlayerPosY);
                    LogMSG = "You Got Gold!";
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
        static void GoldCheck(int x, int y) // <- Checks whether the gold pickups exist
        {
            if (PGold == 0)
            {
                Pickup01X = x;
                Pickup01Y = y;
                Pickup01 = false;
                PGold = PGold + 1;
            }
            else if (PGold == 1)
            {
                Pickup02X = x;
                Pickup02Y = y;
                Pickup02 = false;
                PGold = PGold + 1;
            }
            else if (PGold == 2)
            {
                Pickup03X = x;
                Pickup03Y = y;
                Pickup03 = false;
                PGold = PGold + 1;
            }
            else if (PGold == 3)
            {
                Pickup04X = x;
                Pickup04Y = y;
                Pickup04 = false;
                PGold = PGold + 1;
            }
            else if (PGold == 4)
            {
                Pickup05X = x;
                Pickup05Y = y;
                Pickup05 = false;
                PGold = PGold + 1;
            }
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
            if (EHp > 0)
            {
                GFX.GridProcGFX('5', EnemyPosX, EnemyPosY);
            }
            if (E2Hp > 0)
            {
                GFX.GridProcGFX('5', Enemy2PosX, Enemy2PosY);
            }
        }
        static void PrintMap() // <- prints the map to the screen 
        {
            for (int i = 0; i < MapDown; i++)
            {
                for (int j = 0; j < MapAcross; j++)
                {
                    //Console.SetCursorPosition(j, i);
                    //Console.Write(MapLegend()[i][j]);
                    if (i == j)
                    {

                    }
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
            if (Pickup01 == false)
            {
                MapLegendArray[Pickup01Y][Pickup01X] = '0';
            }
            if (Pickup02 == false)
            {
                MapLegendArray[Pickup02Y][Pickup02X] = '0';
            }
            if (Pickup03 == false)
            {
                MapLegendArray[Pickup03Y][Pickup03X] = '0';
            }
            if (Pickup04 == false)
            {
                MapLegendArray[Pickup04Y][Pickup04X] = '0';
            }
            if (Pickup05 == false)
            {
                MapLegendArray[Pickup05Y][Pickup05X] = '0';
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
            //if (EHasAttacked)
            //{
            //    EHasAttacked = false;
            //}
            //else
            //{
            //}
            if (EHp > 0)
            {
                EnemyMovement();
            }
            else
            {
                EnemyPosX = 0;
                EnemyPosY = 0;
            }
            if (E2Hp > 0)
            {
                Enemy2Movement();
            }
            else
            {
                Enemy2PosX = 0;
                Enemy2PosY = 0;
            }
            PrintHUD();

        }
        static void EnemyMovement() // <- processes enemy one movement 
        {
            switch (EnemyAI())
            {
                case 0: break;
                case 1: 
                    if (MapTileCheck(EnemyPosX, (EnemyPosY-1)) == '0') 
                    {
                        EnemyPosY -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosY += 1;
                            EHasAttacked = false;
                        }
                    }
                    if (MapTileCheck(EnemyPosX, (EnemyPosY - 1)) == '8')
                    {
                        EnemyPosY -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosY += 1;
                            EHasAttacked = false;
                        }
                    }
                    break;
                case 2: 
                    if (MapTileCheck((EnemyPosX-1), EnemyPosY) == '0') 
                    { 
                        EnemyPosX -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosX += 1;
                            EHasAttacked = false;
                        }
                    }
                    if (MapTileCheck((EnemyPosX - 1), EnemyPosY) == '8')
                    {
                        EnemyPosX -= 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosX += 1;
                            EHasAttacked = false;
                        }
                    }
                    break;
                case 3: 
                    if (MapTileCheck(EnemyPosX, (EnemyPosY+1)) == '0') 
                    { 
                        EnemyPosY += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosY -= 1;
                            EHasAttacked = false;
                        }
                    }
                    if (MapTileCheck(EnemyPosX, (EnemyPosY + 1)) == '8')
                    {
                        EnemyPosY += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosY -= 1;
                            EHasAttacked = false;
                        }
                    }
                    break;
                case 4: 
                    if (MapTileCheck((EnemyPosX+1), EnemyPosY) == '0') 
                    { 
                        EnemyPosX += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosX -= 1;
                            EHasAttacked = false;
                        }
                    }
                    if (MapTileCheck((EnemyPosX + 1), EnemyPosY) == '8')
                    {
                        EnemyPosX += 1;
                        PHp = CombatFunction(EnemyPosX, EnemyPosY, PlayerPosX, PlayerPosY, EnemyDmg, PHp, 1);
                        if (EHasAttacked)
                        {
                            EnemyPosX -= 1;
                            EHasAttacked = false;
                        }
                    }
                    break;
            }
        }
        static void Enemy2Movement() // <- processes enemy two movement 
        {
            switch (Enemy2AI())
            {
                case 0: break;
                case 1:
                    if (MapTileCheck(Enemy2PosX, (Enemy2PosY - 1)) == '0')
                    {
                        Enemy2PosY -= 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosY += 1;
                            E2HasAttacked = false;
                        }
                    }
                    if (MapTileCheck(Enemy2PosX, (Enemy2PosY - 1)) == '8')
                    {
                        Enemy2PosY -= 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (AttackCollision)
                        {
                            Enemy2PosY += 1;
                            E2HasAttacked = false;
                        }
                    }
                    break;
                case 2:
                    if (MapTileCheck((Enemy2PosX - 1), Enemy2PosY) == '0')
                    {
                        Enemy2PosX -= 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosX += 1;
                            E2HasAttacked = false;
                        }
                    }
                    if (MapTileCheck((Enemy2PosX - 1), Enemy2PosY) == '8')
                    {
                        Enemy2PosX -= 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosX += 1;
                            E2HasAttacked = false;
                        }
                    }
                    break;
                case 3:
                    if (MapTileCheck(Enemy2PosX, (Enemy2PosY + 1)) == '0')
                    {
                        Enemy2PosY += 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosY -= 1;
                            E2HasAttacked = false;
                        }
                    }
                    if (MapTileCheck(Enemy2PosX, (Enemy2PosY + 1)) == '8')
                    {
                        Enemy2PosY += 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosY -= 1;
                            E2HasAttacked = false;
                        }
                    }
                    break;
                case 4:
                    if (MapTileCheck((Enemy2PosX + 1), Enemy2PosY) == '0')
                    {
                        Enemy2PosX += 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosX -= 1;
                            E2HasAttacked = false;
                        }
                    }
                    if (MapTileCheck((Enemy2PosX + 1), Enemy2PosY) == '8')
                    {
                        EnemyPosX += 1;
                        PHp = CombatFunction(Enemy2PosX, Enemy2PosY, PlayerPosX, PlayerPosY, Enemy2Dmg, PHp, 2);
                        if (E2HasAttacked)
                        {
                            Enemy2PosX -= 1;
                            E2HasAttacked = false;
                        }
                    }
                    break;
            }
        }
        static int EnemyAI() // <- Determines what enemy one will do 
        {
            if (PlayerPosY < EnemyPosY)
            {
                return 1; // <- up
            }
            else if (PlayerPosX < EnemyPosX)
            {
                return 2; // <- left
            }
            else if (PlayerPosY > EnemyPosY)
            {
                return 3; // <- down
            }
            else if (PlayerPosX > EnemyPosX)
            {
                return 4; // <- right
            }
            else
            {
                return 0; // <- N/A
            }
        }
        static int Enemy2AI() // <- Determines what enemy two will do 
        {
            //if (PlayerPosY < Enemy2PosY)
            //{
            //    return 1;
            //}
            //else if (PlayerPosX < Enemy2PosX)
            //{
            //    return 2;
            //}
            //else if (PlayerPosY > Enemy2PosY)
            //{
            //    return 3;
            //}
            //else if (PlayerPosX > Enemy2PosX)
            //{
            //    return 4;
            //}
            //else
            //{
            //    return 0;
            //}
            Random RNG = new Random();
            switch (RNG.Next(1,4))
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
            }
            return 0;
        }
        static int CombatFunction(int atkrX, int atkrY, int dfndX, int dfndY, int atkrDmg, int dfndHp, int Enemy) // <- processes combat between entities 
        {
            if (atkrX == dfndX && atkrY == dfndY)
            {
                switch (Enemy)
                {
                    case 0:
                        AttackCollision = true;
                        break;
                    case 1:
                        EHasAttacked = true;
                        LogMSG = "You Are Engaged In Combat!";
                        break;
                    case 2:
                        E2HasAttacked = true;
                        LogMSG = "You Are Engaged In Combat!";
                        break;
                }
                dfndHp -= atkrDmg;
                if (dfndHp <= 0)
                {
                    //LogMSG = "You Are Victorious!";
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
            Console.Write("                            \n                           \n                          \n                                                                         "); 
            Console.SetCursorPosition(5, 15);
            Console.Write("Player Hp: " + PHp + "/" + PmHp + "  |  " + "Gold: " + PGold);
            Console.Write("\n" + "Enemy: " + EHp + "/" + EmHp);
            Console.Write("\n" + "Enemy: " + E2Hp + "/" + E2mHp);
            Console.Write("\n" + "Intel Report: " + LogMSG);
            
        }
    }
}
