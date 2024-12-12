using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    private AudioManager audioManager;
    public GameObject controller;
    private GameObject reference = null;

    public int matrixX;
    public int matrixY;

    public bool attack = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);  // Red for attack
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp != null)
            {
                if (cp.name == "white_king")
                {
                    controller.GetComponent<Game>().Winner("black");
                    audioManager.PlaySFX(audioManager.winClip);
                }
                if (cp.name == "black_king")
                {
                    controller.GetComponent<Game>().Winner("white");
                    audioManager.PlaySFX(audioManager.winClip);
                }

                Destroy(cp); // Xóa quân bị ăn
                controller.GetComponent<Game>().RemovePieceFromPlayerList(cp); // Loại bỏ quân khỏi danh sách

            }
        }

        // Cập nhật vị trí quân cờ sau khi di chuyển
        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(), reference.GetComponent<Chessman>().GetYBoard());
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();
        controller.GetComponent<Game>().SetPosition(reference);

        audioManager.PlaySFX(audioManager.moveClip);

        // Chuyển lượt chơi sang người chơi tiếp theo (hoặc bot)
        controller.GetComponent<Game>().NextTurn();

        // Xóa các ô di chuyển của quân cờ
        reference.GetComponent<Chessman>().DestroyMovePlates();
    }



    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
