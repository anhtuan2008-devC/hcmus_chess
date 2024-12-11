using UnityEngine;
using TMPro;

public class DemGio : MonoBehaviour
{
    public TextMeshProUGUI timeTextBlack;
    public TextMeshProUGUI timeTextWhite;
    public float countDownTime = 302f; // Thời gian đếm ngược 5 phút.
    public float timePlus = 2f;
    private float currentTimeBlack;
    private float currentTimeWhite;
    private Game game;
    private string previousPlayer = "";

    void Start()
    {
        Invoke("OpStart", 2f);
    }

    void OpStart ()
    {
        game = FindObjectOfType<Game>();
        if (game == null)
        {
            Debug.LogError("Không tìm thấy script Game trong Scene.");
        }
        currentTimeWhite = countDownTime;
        currentTimeBlack = countDownTime;

        // Ghi nhận lượt đầu tiên (nếu cần thiết).
        if (game != null)
        {
            previousPlayer = game.GetCurrentPlayer();
        }
    }

    void Update()
    {
        // Nếu game đã kết thúc, thoát sớm.
        if (game == null || game.IsGameOver())
        {
            return;
        }

        // Cập nhật thời gian dựa trên người chơi hiện tại.
        string currentPlayer = game.GetCurrentPlayer();

        // Kiểm tra nếu lượt chơi thay đổi:
        if (currentPlayer != previousPlayer)
        {
            // Thêm 2 giây cho người chơi vừa kết thúc lượt.
            if (previousPlayer == "white")
            {
                currentTimeWhite += timePlus;
                UpdateTimeTextWhite();
            }
            else if (previousPlayer == "black")
            {
                currentTimeBlack += timePlus;
                UpdateTimeTextBlack();
            }

            // Cập nhật người chơi trước đó.
            previousPlayer = currentPlayer;
        }

        // Trừ thời gian của người chơi hiện tại.
        if (currentPlayer == "white" && currentTimeWhite > 0)
        {
            currentTimeWhite -= Time.deltaTime;
            UpdateTimeTextWhite();
        }
        else if (currentPlayer == "black" && currentTimeBlack > 0)
        {
            currentTimeBlack -= Time.deltaTime;
            UpdateTimeTextBlack();
        }

        // Kiểm tra hết giờ cho từng người chơi.
        if (currentTimeBlack <= 0)
        {
            currentTimeBlack = 0;
            UpdateTimeTextBlack();
            tinhNang("white"); // Trắng thắng khi đen hết giờ.
        }
        else if (currentTimeWhite <= 0)
        {
            currentTimeWhite = 0;
            UpdateTimeTextWhite();
            tinhNang("black"); // Đen thắng khi trắng hết giờ.
        }
    }

    // Xử lý khi hết giờ.
    void tinhNang(string winner)
    {
        Debug.Log($"{winner} thắng do đối thủ hết thời gian!");
        game.Winner(winner); // Gọi hàm Winner từ script Game.
    }

    // Cập nhật UI thời gian cho người chơi đen.
    void UpdateTimeTextBlack()
    {
        int minutes = (int)(currentTimeBlack / 60);
        int seconds = (int)(currentTimeBlack % 60);
        timeTextBlack.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Cập nhật UI thời gian cho người chơi trắng.
    void UpdateTimeTextWhite()
    {
        int minutes = (int)(currentTimeWhite / 60);
        int seconds = (int)(currentTimeWhite % 60);
        timeTextWhite.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
