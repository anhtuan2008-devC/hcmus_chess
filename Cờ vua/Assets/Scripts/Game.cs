using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;
    public GameObject gameOverObject;

    public GameObject[,] positions = new GameObject[8, 8]; // Đây là mảng 2 chiều 8x8, đại diện cho bàn cờ. Mỗi ô chứa thông tin về quân cờ đang ở vị trí đó (nếu có).

    // Hai mảng lưu danh sách các quân cờ thuộc về người chơi "black" (đen) và "white" (trắng). Mỗi người chơi có tối đa 16 quân cờ.
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    // Biến theo dõi lượt chơi hiện tại, giá trị là "white" hoặc "black".
    private string currentPlayer = "white";

    // Đánh dấu trạng thái của trò chơi (kết thúc hoặc chưa kết thúc).
    private bool gameOver = false;
    private AudioManager audioManager;

    //Unity calls this right when the game starts
    public void Start()
    {
        // Tìm đối tượng AudioManager trong Scene
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager không được tìm thấy trong Scene.");
        }
        // Tạo các quân cờ trắng và đen bằng cách gọi hàm Create
        playerWhite = new GameObject[] { Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1) };
        playerBlack = new GameObject[] { Create("black_rook", 0, 7), Create("black_knight",1,7),
            Create("black_bishop",2,7), Create("black_queen",3,7), Create("black_king",4,7),
            Create("black_bishop",5,7), Create("black_knight",6,7), Create("black_rook",7,7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6) };

        // Đặt tất cả quân cờ vào mảng positions thông qua hàm SetPosition
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
        audioManager.PlaySFX(audioManager.whiteTurnClip);
    }

    // Tạo và khởi tạo một quân cờ.
    public GameObject Create(string name, int x, int y)
    {
        //1) Tạo một đối tượng quân cờ bằng Instantiate.
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);

        //2) Gán tên (ví dụ: "white_rook") và tọa độ(x, y).
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);

        //3) Kích hoạt đối tượng thông qua hàm Activate trong script Chessman.
        cm.Activate();
        return obj;
    }

    // Đặt một quân cờ vào vị trí xác định trên mảng positions.
    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    // Làm trống vị trí (x, y) trên mảng positions.
    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    // Hàm trả về quân cờ tại vị trí (x, y) hoặc null nếu không có quân cờ ở đó:
    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    // Hàm kiểm tra xem tọa độ (x, y) có nằm trong phạm vi bàn cờ (0-7) hay không
    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    // Trả về người chơi hiện tại (đen hoặc trắng).
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver() // Kiểm tra xem game có kết thúc chưa
    {
        return gameOver;
    }

    // Chuyển lượt cho người chơi còn lại:
    public void NextTurn()
    {
        if (IsGameOver()) { return; }
        else if (currentPlayer == "white")
        {
            currentPlayer = "black";
            if (!IsGameOver()) { audioManager.PlaySFX(audioManager.blackTurnClip); }
        }
        else
        {
            currentPlayer = "white";
            if (!IsGameOver()) { audioManager.PlaySFX(audioManager.whiteTurnClip); }
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;
        gameOverObject.SetActive(true);

        // Tìm GameObject với tag "WinnerText"
        GameObject winnerTextObj = GameObject.FindGameObjectWithTag("WinnerText");
        if (playerWinner == "white")
        {
            audioManager.PlaySFX(audioManager.whiteWinClip);
        }
        if (playerWinner == "black")
        {
            audioManager.PlaySFX(audioManager.blackWinClip);
        }
        if (winnerTextObj != null)
        {
            // Lấy thành phần TMP_Text
            TMP_Text winnerText = winnerTextObj.GetComponent<TMP_Text>();
                winnerText.text = playerWinner + " is the winner";
        }
    }
}
