using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    private AudioManager audioManager; // Được dùng để quản lý âm thanh (các hiệu ứng khi di chuyển hoặc chiến thắng)
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Tham chiếu đến đối tượng quản lý trò chơi (GameController). Từ đó, các thông tin về trạng thái bàn cờ và logic trò chơi được truy cập.
    public GameObject controller;

    // Tham chiếu đến quân cờ gốc (Chessman) đã tạo ra MovePlate này
    GameObject reference = null;

    // Tọa độ của MovePlate trên bàn cờ(trong hệ tọa độ ma trận 8x8).
    int matrixX;
    int matrixY;

    // false: Vị trí có thể di chuyển đến, true: Vị trí có thể tấn công(có quân đối phương).
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            // Nếu attack là true, MovePlate được làm nổi bật bằng cách đổi màu thành đỏ, giúp người chơi dễ nhận biết vị trí tấn công hợp lệ
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    // Đây là phương thức chính được kích hoạt khi người chơi nhấn chuột lên MovePlate.
    public void OnMouseUp()
    {
        // 1. Tìm GameController
        controller = GameObject.FindGameObjectWithTag("GameController");

        // 2. Nếu là vị trí tấn công (attack == true):
        if (attack)
        {
            // Lấy quân cờ tại vị trí tấn công
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            // Nếu quân bị tấn công là vua, kết thúc trò chơi:
            if (cp.name == "white_king") { controller.GetComponent<Game>().Winner("black"); audioManager.PlaySFX(audioManager.winClip); }
            if (cp.name == "black_king") { controller.GetComponent<Game>().Winner("white"); audioManager.PlaySFX(audioManager.winClip); }

            // Xóa quân bị tấn công:
            Destroy(cp);
        }

        // Cập nhật vị trí của quân cờ tham chiếu (reference):
        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),reference.GetComponent<Chessman>().GetYBoard());

        // Cập nhật tọa độ mới cho quân cờ
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        // Cập nhật ma trận bàn cờ
        controller.GetComponent<Game>().SetPosition(reference);

        // Phát âm thanh di chuyển
        audioManager.PlaySFX(audioManager.moveClip);

        // Chuyển lượt chơi
        controller.GetComponent<Game>().NextTurn();

        // Xóa tất cả MovePlates:
        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    // Gán tọa độ (x, y) cho MovePlate:
    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }
    // Gán quân cờ tạo ra MovePlate này.
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    // Trả về quân cờ tham chiếu.
    public GameObject GetReference()
    {
        return reference;
    }
}