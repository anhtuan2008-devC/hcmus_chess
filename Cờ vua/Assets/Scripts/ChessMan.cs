using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //References to objects in our Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    //Position for this Chesspiece on the Board
    //The correct position will be set later
    private int xBoard = -1;
    private int yBoard = -1;

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public string GetPlayer()
    {
        return player;
    }

    public void Activate()
    {
        //Get the game controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Take the instantiated location and adjust transform
        SetCoords();

        //Choose correct sprite based on piece's name
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    // Điều chỉnh vị trí của quân cờ trên bàn cờ Unity dựa vào tọa độ xBoard và yBoard
    public void SetCoords()
    {
        //Get the board value in order to convert to xy coords
        float x = xBoard;
        float y = yBoard;

        //Adjust by variable offset
        x *= 1.1f;
        y *= 1.1f;

        //Add constants (pos 0,0)
        x += -3.88f;
        y += -3.88f;

        //Set actual unity values
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    private void OnMouseUp() //Kích hoạt khi người chơi nhấn vào quân cờ
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            // Xóa các gợi ý nước đi cũ bằng hàm
            DestroyMovePlates();

            // Tạo các gợi ý nước đi mới bằng
            InitiateMovePlates();
        }
    }

    // Xóa các gợi ý nước đi cũ 
    public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); //Be careful with this function "Destroy" it is asynchronous
        }
    }

    // Xác định các nước đi hợp lệ dựa vào loại quân cờ (hậu, xe, mã, tượng, vua, tốt).
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
                phongTot();
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                phongTot();
                break;
        }
    }

    // Quân di chuyển theo đường thẳng hoặc đường chéo (hậu, xe, tượng).
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

    // Quân mã di chuyển theo hình chữ "L".
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
        //Game sc = controller.GetComponent<Game>();
        //if (kingMoved == false && sc.GetPosition(xBoard -1, yBoard) == null && sc.GetPosition(xBoard - 2, yBoard) == null && sc.GetPosition(xBoard - 3, yBoard) == null)
        //{
        //    switch (player)
        //    {
        //        case "white":
        //            this.GetComponent<SpriteRenderer>().sprite = white_rook;
        //            this.name = "white_rook";
        //            break;
        //        case "black":
        //            this.GetComponent<SpriteRenderer>().sprite = black_rook;
        //            this.name = "black_rook";
        //            break;
        //    }
        //    if (sc.PositionOnBoard(xBoard - 4, yBoard))
        //    {
        //        GameObject cp = sc.GetPosition(xBoard - 4, yBoard);

        //        if (cp != null)
        //        {
        //            MovePlateSpawn(xBoard - 4, yBoard);
        //        }
        //    }
        //}
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    // Xử lý di chuyển của quân tốt, bao gồm di chuyển một ô, hai ô (nước đi đầu tiên) hoặc ăn chéo.
    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            // Di chuyen 1 o
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }
            // Di chuyen 2 o luc bat dau
            if (sc.PositionOnBoard(x, y + 1) && sc.GetPosition(x, y) == null && sc.GetPosition(x, y + 1) == null && yBoard == 1 && player == "white")
            {
                MovePlateSpawn(x, y + 1);
            }
            if (sc.PositionOnBoard(x, y - 1) && sc.GetPosition(x, y) == null && sc.GetPosition(x, y - 1) == null && yBoard == 6 && player == "black")
            {
                MovePlateSpawn(x, y - 1);
            }

            // Di chuyen cheo an quan
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }
            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    // Tạo các ô gợi ý nước đi hợp lệ.
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= 1.1f;
        y *= 1.1f;

        //Add constants (pos 0,0)
        x += -3.88f;
        y += -3.88f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    // Tạo các ô gợi ý nước đi để ăn quân đối phương.
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= 1.1f;
        y *= 1.1f;

        //Add constants (pos 0,0)
        x += -3.88f;
        y += -3.88f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
    public void phongTot()
    {
        Game sc = controller.GetComponent<Game>();

        // Kiểm tra nếu quân tốt đã đến hàng cuối
        if ((player == "white" && yBoard == 7) || (player == "black" && yBoard == 0))
        {
            // Phong cấp thành quân hậu
            switch (player)
            {
                case "white":
                    this.GetComponent<SpriteRenderer>().sprite = white_queen;
                    this.name = "white_queen";
                    break;
                case "black":
                    this.GetComponent<SpriteRenderer>().sprite = black_queen;
                    this.name = "black_queen";
                    break;
            }
        }
    }
}