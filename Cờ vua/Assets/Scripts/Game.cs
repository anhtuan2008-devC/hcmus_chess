using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    public bool isPvE = true;  // true: PvE mode, false: PvP mode
    private bool isAITurn = false;
    public GameObject chesspiece;
    public GameObject gameOverObject;
    private GameObject[,] positions = new GameObject[8, 8];  // Board grid 8x8
    private List<GameObject> playerBlack = new List<GameObject>();
    private List<GameObject> playerWhite = new List<GameObject>();

    private string currentPlayer = "white";
    private bool gameOver = false;
    private AudioManager audioManager;

    public enum DifficultyLevel { Easy, Medium, Hard }
    public DifficultyLevel difficulty = DifficultyLevel.Easy;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found.");
        }

        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // Initialize white pieces
        playerWhite.AddRange(InitializePieces("white", 0, 1));

        // Initialize black pieces
        playerBlack.AddRange(InitializePieces("black", 7, 6));

        // Set all pieces on the board
        foreach (var piece in playerWhite) SetPosition(piece);
        foreach (var piece in playerBlack) SetPosition(piece);

        audioManager?.PlaySFX(audioManager.whiteTurnClip);
    }

    private List<GameObject> InitializePieces(string color, int backRow, int frontRow)
    {
        List<GameObject> pieces = new List<GameObject>();

        pieces.AddRange(new GameObject[] {
            Create(color + "_rook", 0, backRow), Create(color + "_knight", 1, backRow),
            Create(color + "_bishop", 2, backRow), Create(color + "_queen", 3, backRow),
            Create(color + "_king", 4, backRow), Create(color + "_bishop", 5, backRow),
            Create(color + "_knight", 6, backRow), Create(color + "_rook", 7, backRow),
            Create(color + "_pawn", 0, frontRow), Create(color + "_pawn", 1, frontRow),
            Create(color + "_pawn", 2, frontRow), Create(color + "_pawn", 3, frontRow),
            Create(color + "_pawn", 4, frontRow), Create(color + "_pawn", 5, frontRow),
            Create(color + "_pawn", 6, frontRow), Create(color + "_pawn", 7, frontRow)
        });

        return pieces;
    }

    private GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);

        // Kiểm tra và gắn Chessman vào GameObject
        Chessman cm = obj.GetComponent<Chessman>();
        if (cm == null)
        {
            cm = obj.AddComponent<Chessman>(); // Gắn Chessman nếu chưa có
        }

        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();

        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public bool PositionOnBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public string GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => gameOver;

    public void NextTurn()
    {
        if (gameOver) return;

        currentPlayer = currentPlayer == "white" ? "black" : "white"; // Đổi lượt chơi
        audioManager?.PlaySFX(currentPlayer == "white" ? audioManager.whiteTurnClip : audioManager.blackTurnClip);

        // Kiểm tra xem có phải là lượt của bot không
        isAITurn = !isAITurn;

        if (isPvE && isAITurn) // Nếu đang chơi PvE và đến lượt bot
        {
            StartCoroutine(PerformAIMove()); // Bot thực hiện nước đi
        }
    }


    private IEnumerator PerformAIMove()
    {
        yield return new WaitForSeconds(1f);

        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                PerformEasyMove();
                break;
            case DifficultyLevel.Medium:
                PerformSmartMove();
                break;
            case DifficultyLevel.Hard:
                PerformHardMove();
                break;
        }
    }

    private void PerformEasyMove()
    {
        List<GameObject> aiPieces = currentPlayer == "black" ? playerBlack : playerWhite;
        foreach (var piece in aiPieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            cm.InitiateMovePlates();
            List<GameObject> movePlates = GetActiveMovePlates();

            if (movePlates.Count > 0)
            {
                // Chọn một nước đi ngẫu nhiên
                GameObject randomMove = movePlates[UnityEngine.Random.Range(0, movePlates.Count)];

                // Lấy vị trí mới mà bot sẽ di chuyển đến
                int targetX = randomMove.GetComponent<MovePlate>().matrixX;
                int targetY = randomMove.GetComponent<MovePlate>().matrixY;

                // Kiểm tra nếu vị trí có quân của đối phương
                GameObject targetPiece = positions[targetX, targetY];
                if (targetPiece != null && targetPiece.GetComponent<Chessman>().player != currentPlayer)
                {
                    // Nếu có quân đối phương, bot sẽ ăn quân đó
                    Destroy(targetPiece);
                }

                // Tiến hành di chuyển quân
                randomMove.GetComponent<MovePlate>().OnMouseUp();
                return;
            }
        }
    }

    private void PerformSmartMove()
    {
        List<GameObject> aiPieces = currentPlayer == "black" ? playerBlack : playerWhite;
        GameObject bestPiece = null;
        GameObject bestMovePlate = null;
        int bestScore = int.MinValue;

        foreach (var piece in aiPieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            cm.InitiateMovePlates();
            List<GameObject> movePlates = GetActiveMovePlates();

            foreach (var movePlate in movePlates)
            {
                int targetX = movePlate.GetComponent<MovePlate>().matrixX;
                int targetY = movePlate.GetComponent<MovePlate>().matrixY;

                GameObject targetPiece = positions[targetX, targetY];
                int moveScore = 0;

                // Tính điểm cho nước đi
                if (targetPiece != null)
                {
                    Chessman targetCm = targetPiece.GetComponent<Chessman>();
                    if (targetCm.player != currentPlayer)
                    {
                        moveScore += GetPieceValue(targetCm.type); // Giá trị quân cờ bị ăn
                    }
                }

                // Thêm điểm cho vị trí chiến lược (nếu cần)
                moveScore += EvaluatePosition(targetX, targetY);

                // So sánh và chọn nước đi tốt nhất
                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestPiece = piece;
                    bestMovePlate = movePlate;
                }
            }
        }

        // Thực hiện nước đi tốt nhất
        if (bestPiece != null && bestMovePlate != null)
        {
            int targetX = bestMovePlate.GetComponent<MovePlate>().matrixX;
            int targetY = bestMovePlate.GetComponent<MovePlate>().matrixY;

            GameObject targetPiece = positions[targetX, targetY];
            if (targetPiece != null && targetPiece.GetComponent<Chessman>().player != currentPlayer)
            {
                Destroy(targetPiece);
            }

            bestMovePlate.GetComponent<MovePlate>().OnMouseUp();
        }
    }

    // Hàm tính giá trị quân cờ
    private int GetPieceValue(string type)
    {
        switch (type)
        {
            case "pawn": return 1;
            case "knight": return 3;
            case "bishop": return 3;
            case "rook": return 5;
            case "queen": return 9;
            case "king": return 1000; // Giá trị rất cao để bảo vệ vua
            default: return 0;
        }
    }

    // Hàm đánh giá vị trí
    private int EvaluatePosition(int x, int y)
    {
        // Ví dụ: Ưu tiên vị trí ở giữa bàn cờ
        return (4 - Math.Abs(4 - x)) + (4 - Math.Abs(4 - y));
    }

    private void PerformMediumMove()
    {
        List<Move> possibleMoves = GetAllPossibleMoves(currentPlayer);
        Move bestMove = null;
        int maxGain = int.MinValue; // Khởi tạo giá trị tối thiểu

        foreach (Move move in possibleMoves)
        {
            int gain = GetPieceValue(move.targetPiece); // Tính giá trị của quân cờ đối phương

            // Nếu quân cờ có giá trị lớn hơn, chọn làm nước đi tốt nhất
            if (gain > maxGain)
            {
                maxGain = gain;
                bestMove = move;
            }
        }

        if (bestMove != null)
        {
            ExecuteMove(bestMove); // Thực hiện nước đi tốt nhất
        }
    }


    private void PerformHardMove()
    {
        Move bestMove = MiniMaxAlgorithm(currentPlayer, 3, int.MinValue, int.MaxValue);
        // Depth = 3
        ExecuteMove(bestMove);
    }

    private List<Move> GetAllPossibleMoves(string player)
    {
        List<Move> moves = new List<Move>();
        List<GameObject> playerPieces = player == "black" ? playerBlack : playerWhite;

        foreach (GameObject piece in playerPieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            cm.InitiateMovePlates(); // Khởi tạo các ô di chuyển của quân cờ
            foreach (GameObject movePlate in GetActiveMovePlates()) // Lấy danh sách các ô di chuyển
            {
                MovePlate mp = movePlate.GetComponent<MovePlate>();
                GameObject targetPiece = positions[mp.matrixX, mp.matrixY]; // Lấy quân cờ tại vị trí đích

                // Kiểm tra nếu ô đích có quân đối phương
                if (targetPiece != null && targetPiece.GetComponent<Chessman>().player != player)
                {
                    // Bot sẽ ăn quân đối phương
                    moves.Add(new Move(piece, new Vector2Int(cm.GetXBoard(), cm.GetYBoard()), new Vector2Int(mp.matrixX, mp.matrixY), targetPiece));
                }
                else if (targetPiece == null)
                {
                    // Nếu ô trống, bot có thể di chuyển đến đó
                    moves.Add(new Move(piece, new Vector2Int(cm.GetXBoard(), cm.GetYBoard()), new Vector2Int(mp.matrixX, mp.matrixY)));
                }
            }
        }

        return moves;
    }

    private Move MiniMax(string player, int depth, int alpha, int beta)
    {
        if (depth == 0)
        {
            return EvaluatePosition(player); // Đánh giá và trả về một Move
        }

        List<Move> possibleMoves = GetAllPossibleMoves(player);
        Move bestMove = null;

        foreach (Move move in possibleMoves)
        {
            ExecuteMove(move);
            Move evalMove = MiniMax(player == "white" ? "black" : "white", depth - 1, alpha, beta);
            UndoMove(move);

            // Chọn move tốt nhất
            if (evalMove != null)
            {
                bestMove = evalMove;
            }

            // Cắt tỉa Alpha-Beta
            if (player == "white")
            {
                alpha = Mathf.Max(alpha, bestMove != null ? bestMove.value : int.MinValue);
            }
            else
            {
                beta = Mathf.Min(beta, bestMove != null ? bestMove.value : int.MaxValue);
            }

            if (beta <= alpha)
            {
                break; // Cắt tỉa
            }
        }

        return bestMove;
    }


    private Move EvaluatePosition(string player)
    {
        List<Move> possibleMoves = GetAllPossibleMoves(player);

        if (possibleMoves.Count > 0)
        {
            // Đánh giá move đầu tiên (Ví dụ: trả về value ngẫu nhiên hoặc dựa trên logic của bạn)
            int value = 0; // Cần tính toán giá trị đánh giá dựa trên bàn cờ
            return new Move(possibleMoves[0].x, possibleMoves[0].y, value);
        }

        return null;
    }



    private Move MiniMaxAlgorithm(string player, int depth, int alpha, int beta)
    {
        if (depth == 0)
        {
            return EvaluatePosition(player); // Trả về Move tốt nhất
        }

        List<Move> possibleMoves = GetAllPossibleMoves(player);
        Move bestMove = null;

        if (player == "white") // Bot tối đa hóa điểm
        {
            int maxEval = int.MinValue;

            foreach (Move move in possibleMoves)
            {
                ExecuteMove(move);
                Move evalMove = MiniMax(player == "white" ? "black" : "white", depth - 1, alpha, beta);
                if (evalMove != null && evalMove.value > maxEval)
                {
                    maxEval = evalMove.value;
                    bestMove = evalMove;
                }

                UndoMove(move);

                if (evalMove.value > maxEval)
                {
                    maxEval = evalMove.value;
                    bestMove = evalMove;
                }

                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha)
                {
                    break;  // Cắt tỉa Alpha-Beta
                }
            }
        }
        else // Người chơi tối thiểu hóa điểm
        {
            int minEval = int.MaxValue;

            foreach (Move move in possibleMoves)
            {
                ExecuteMove(move);
                Move evalMove = MiniMax(player == "white" ? "black" : "white", depth - 1, alpha, beta);
                UndoMove(move);

                if (evalMove.value < minEval)
                {
                    minEval = evalMove.value;
                    bestMove = evalMove;
                }

                beta = Mathf.Min(beta, minEval);
                if (beta <= alpha)
                {
                    break;  // Cắt tỉa
                }
            }
        }

        return bestMove;
    }



    private int MinimaxEvaluate(string player, int depth, int alpha, int beta)
    {
        if (depth == 0) // Đánh giá tại các nút lá (hoặc độ sâu tối đa)
        {
            return EvaluateBoard(player);
        }

        List<Move> possibleMoves = GetAllPossibleMoves(player);
        int eval = (player == "white") ? int.MinValue : int.MaxValue;

        foreach (var move in possibleMoves)
        {
            ExecuteMove(move);
            int childEval = MinimaxEvaluate(player == "white" ? "black" : "white", depth - 1, alpha, beta);
            UndoMove(move);

            if (player == "white")
            {
                eval = Mathf.Max(eval, childEval);
                alpha = Mathf.Max(alpha, eval);
            }
            else
            {
                eval = Mathf.Min(eval, childEval);
                beta = Mathf.Min(beta, eval);
            }

            if (beta <= alpha)
                break;
        }

        return eval;
    }

    private void UndoMove(Move move)
    {
        // Hoàn tác các bước đi bằng cách đưa quân cờ quay lại vị trí cũ và khôi phục quân bị ăn
        Chessman cm = move.chessPiece.GetComponent<Chessman>();
        cm.SetXBoard(move.start.x);
        cm.SetYBoard(move.start.y);
        cm.SetCoords();
        SetPosition(move.chessPiece);

        if (move.targetPiece != null)
        {
            Destroy(move.targetPiece);  // Khôi phục quân cờ bị ăn
        }
    }

    private void ExecuteMove(Move move)
    {
        if (move == null) return;

        Chessman cm = move.chessPiece.GetComponent<Chessman>();

        // Lấy vị trí quân cờ đích
        MovePlate mp = cm.MovePlateSpawn(move.end.x, move.end.y);

        // Nếu có quân của đối phương tại vị trí đó, thực hiện ăn quân
        if (move.targetPiece != null)
        {
            Destroy(move.targetPiece); // Ăn quân của đối phương
        }

        mp.OnMouseUp(); // Di chuyển quân tới vị trí đích
    }


    private int GetPieceValue(GameObject piece)
    {
        if (piece == null) return 0;
        string pieceName = piece.name.ToLower();
        if (pieceName.Contains("pawn")) return 1;
        if (pieceName.Contains("knight") || pieceName.Contains("bishop")) return 3;
        if (pieceName.Contains("rook")) return 5;
        if (pieceName.Contains("queen")) return 9;
        return 0;
    }

    private List<GameObject> GetActiveMovePlates()
    {
        return new List<GameObject>(GameObject.FindGameObjectsWithTag("MovePlate"));
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;
        gameOverObject.SetActive(true);

        var winnerTextObj = GameObject.FindGameObjectWithTag("WinnerText");
        if (winnerTextObj != null)
        {
            var winnerText = winnerTextObj.GetComponent<TMP_Text>();
            winnerText.text = $"{playerWinner} is the winner";
        }

        audioManager?.PlaySFX(playerWinner == "white" ? audioManager.whiteWinClip : audioManager.blackWinClip);
        // Sau khi xác nhận người thắng, cần cập nhật trạng thái game.
        if (playerWinner == "white")
        {
            // Xử lý khi người chơi thắng
            // Cập nhật lượt tiếp theo cho người chơi nếu có
            NextTurn();
        }
        else
        {
            // Xử lý khi bot thắng
            // Cập nhật lượt tiếp theo cho bot
            NextTurn();
        }
    }
    public void RemovePieceFromPlayerList(GameObject piece)
    {
        if (playerBlack.Contains(piece))
        {
            playerBlack.Remove(piece);
        }
        else if (playerWhite.Contains(piece))
        {
            playerWhite.Remove(piece);
        }
    }
    private int EvaluateBoard(string player)
    {
        int evaluation = 0;

        // Đánh giá quân cờ (ví dụ, quân cờ cao giá trị sẽ được cộng điểm)
        List<GameObject> playerPieces = player == "black" ? playerBlack : playerWhite;
        List<GameObject> opponentPieces = player == "black" ? playerWhite : playerBlack;

        foreach (var piece in playerPieces)
        {
            evaluation += GetPieceValue(piece); // Thêm giá trị quân của người chơi
        }

        foreach (var piece in opponentPieces)
        {
            evaluation -= GetPieceValue(piece); // Trừ giá trị quân của đối thủ
        }

        return evaluation;
    }
}

public class Move
{
    public GameObject chessPiece;
    public Vector2Int start;
    public Vector2Int end;
    public GameObject targetPiece;
    public int value;

    // Thêm các thuộc tính x và y nếu cần
    public int x;
    public int y;

    // Constructor cho Move
    public Move(GameObject chessPiece, Vector2Int start, Vector2Int end, GameObject targetPiece = null, int value = 0)
    {
        this.chessPiece = chessPiece;
        this.start = start;
        this.end = end;
        this.targetPiece = targetPiece;
        this.value = value;
    }

    // Constructor nếu bạn cần x, y như là một kiểu tọa độ
    public Move(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }
}
