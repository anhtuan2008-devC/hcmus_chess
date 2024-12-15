using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public string type;  // "pawn", "knight", "queen", "king", "rook", "bishop"
    // References to objects in the Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    // Position for this Chesspiece on the Board
    private int xBoard = -1;
    private int yBoard = -1;

    // Player identifier ("black" or "white")
    public string player;

    // References to all possible Sprites for this Chesspiece
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    // Getter for player
    public string GetPlayer()
    {
        return player;
    }

    // Activate this chess piece and set its sprite and player
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "black_queen":
                AssignSprite(black_queen, "black");
                type = "queen";  // Gán loại quân cờ
                break;
            case "black_knight":
                AssignSprite(black_knight, "black");
                type = "knight";  // Gán loại quân cờ
                break;
            case "black_bishop":
                AssignSprite(black_bishop, "black");
                type = "bishop";  // Gán loại quân cờ
                break;
            case "black_king":
                AssignSprite(black_king, "black");
                type = "king";  // Gán loại quân cờ
                break;
            case "black_rook":
                AssignSprite(black_rook, "black");
                type = "rook";  // Gán loại quân cờ
                break;
            case "black_pawn":
                AssignSprite(black_pawn, "black");
                type = "pawn";  // Gán loại quân cờ
                break;
            case "white_queen":
                AssignSprite(white_queen, "white");
                type = "queen";  // Gán loại quân cờ
                break;
            case "white_knight":
                AssignSprite(white_knight, "white");
                type = "knight";  // Gán loại quân cờ
                break;
            case "white_bishop":
                AssignSprite(white_bishop, "white");
                type = "bishop";  // Gán loại quân cờ
                break;
            case "white_king":
                AssignSprite(white_king, "white");
                type = "king";  // Gán loại quân cờ
                break;
            case "white_rook":
                AssignSprite(white_rook, "white");
                type = "rook";  // Gán loại quân cờ
                break;
            case "white_pawn":
                AssignSprite(white_pawn, "white");
                type = "pawn";  // Gán loại quân cờ
                break;
        }
        //        // Set the correct sprite and player
        //        switch (this.name)
        //{
        //    case "black_queen": AssignSprite(black_queen, "black"); break;
        //    case "black_knight": AssignSprite(black_knight, "black"); break;
        //    case "black_bishop": AssignSprite(black_bishop, "black"); break;
        //    case "black_king": AssignSprite(black_king, "black"); break;
        //    case "black_rook": AssignSprite(black_rook, "black"); break;
        //    case "black_pawn": AssignSprite(black_pawn, "black"); break;
        //    case "white_queen": AssignSprite(white_queen, "white"); break;
        //    case "white_knight": AssignSprite(white_knight, "white"); break;
        //    case "white_bishop": AssignSprite(white_bishop, "white"); break;
        //    case "white_king": AssignSprite(white_king, "white"); break;
        //    case "white_rook": AssignSprite(white_rook, "white"); break;
        //    case "white_pawn": AssignSprite(white_pawn, "white"); break;
        //}
    }

    // Helper method for assigning sprites and player
    private void AssignSprite(Sprite sprite, string playerColor)
    {
        this.GetComponent<SpriteRenderer>().sprite = sprite;
        player = playerColor;
    }

    // Adjust chess piece position in Unity world
    public void SetCoords()
    {
        float x = xBoard * 1.1f - 3.88f;
        float y = yBoard * 1.1f - 3.88f;
        transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard() => xBoard;
    public int GetYBoard() => yBoard;

    public void SetXBoard(int x) => xBoard = x;
    public void SetYBoard(int y) => yBoard = y;

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() &&
            controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            DestroyMovePlates();
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject obj in movePlates)
        {
            Destroy(obj);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                PromotePawn();
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                PromotePawn();
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);
            if (cp == null) MovePlateSpawn(x, y);
            else if (cp.GetComponent<Chessman>().player != player) MovePlateAttackSpawn(x, y);
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        // Regular move (1 square forward)
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
        }

        // Diagonal attacks
        PawnDiagonalAttack(xBoard + 1, y);
        PawnDiagonalAttack(xBoard - 1, y);

        // Check for the possibility of moving 2 squares forward (first move only)
        if ((player == "white" && yBoard == 1) || (player == "black" && yBoard == 6))
        {
            int twoSquareMoveY = (player == "white") ? yBoard + 2 : yBoard - 2;
            // Check if there's a piece blocking the 2-square move
            if (sc.PositionOnBoard(x, twoSquareMoveY) && sc.GetPosition(x, twoSquareMoveY) == null &&
                sc.GetPosition(x, y) == null)  // Ensure no piece in the square 1 step ahead
            {
                MovePlateSpawn(x, twoSquareMoveY);  // Spawn move plate for 2-square move
            }
        }
    }



    private void PawnDiagonalAttack(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) != null &&
            sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public MovePlate MovePlateSpawn(int x, int y)
    {
        Vector3 position = new Vector3(x * 1.1f - 3.88f, y * 1.1f - 3.88f, -3.0f);
        GameObject mp = Instantiate(movePlate, position, Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(x, y);
        return mpScript;
    }

    public void MovePlateAttackSpawn(int x, int y)
    {
        Vector3 position = new Vector3(x * 1.1f - 3.88f, y * 1.1f - 3.88f, -3.0f);
        GameObject mp = Instantiate(movePlate, position, Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(x, y);
    }

    public void PromotePawn()
    {
        if ((player == "white" && yBoard == 7) || (player == "black" && yBoard == 0))
        {
            AssignSprite(player == "white" ? white_queen : black_queen, player);
            this.name = player == "white" ? "white_queen" : "black_queen";
        }
    }
}
