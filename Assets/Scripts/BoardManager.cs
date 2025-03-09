using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Text stateText;
    Button[,] buttons = new Button[19, 19];
    [SerializeField] private Sprite xSprite, oSprite;
    private char[,] board = new char[19, 19];
    private bool isPlayerTurn = true;
    private int moveCount = 0;

    // Tối ưu hóa tham số AI
    [SerializeField] private int aiDifficulty = 3; // Giảm độ sâu mặc định để AI phản hồi nhanh hơn
    [SerializeField] private float aiThinkingTime = 0.1f; // Giảm thời gian "suy nghĩ" của AI

    // Cải tiến bộ nhớ đệm
    private Dictionary<string, AIEvalResult> transpositionTable = new Dictionary<string, AIEvalResult>();
    private int transpositionHits = 0;

    // Theo dõi vị trí cuối cùng được đánh
    private Vector2Int lastMove = new Vector2Int(-1, -1);

    // Mảng hướng để tối ưu hóa tìm kiếm
    private readonly int[] dx = { 1, 0, 1, 1 };
    private readonly int[] dy = { 0, 1, 1, -1 };

    // Tối ưu bằng cách lưu trữ kết quả hàm đánh giá
    private struct AIEvalResult
    {
        public int Score;
        public int Depth;
        public bool IsExact;

        public AIEvalResult(int score, int depth, bool isExact)
        {
            Score = score;
            Depth = depth;
            IsExact = isExact;
        }
    }

    void Start()
    {
        InitializeBoard();
        stateText.text = "Player Turn";
    }

    void InitializeBoard()
    {
        var cells = GetComponentsInChildren<Button>();
        int n = 0;
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                buttons[i, j] = cells[n];
                board[i, j] = '.';
                n++;
                int r = i, c = j;
                buttons[i, j].onClick.AddListener(() => OnClickCell(r, c));
            }
        }
    }

    private void OnClickCell(int r, int c)
    {
        if (!isPlayerTurn || board[r, c] != '.') return;

        MakeMove(r, c, 'X', xSprite);

        if (CheckGameEnd(r, c, "Player Wins!")) return;

        isPlayerTurn = false;
        stateText.text = "Bot Turn";
        Invoke(nameof(AIMove), aiThinkingTime);
    }

    void MakeMove(int r, int c, char playerSymbol, Sprite playerSprite)
    {
        board[r, c] = playerSymbol;
        buttons[r, c].GetComponent<Image>().sprite = playerSprite;
        buttons[r, c].interactable = false;
        moveCount++;
        lastMove = new Vector2Int(r, c);
    }

    bool CheckGameEnd(int r, int c, string winMessage)
    {
        if (IsWon(r, c))
        {
            stateText.text = winMessage;
            DisableAllButtons();
            return true;
        }
        if (IsGameDraw())
        {
            stateText.text = "Draw!";
            return true;
        }
        return false;
    }

    void AIMove()
    {
        // Xóa bảng chuyển đổi khi đến lượt AI để tránh sự cố bộ nhớ
        if (transpositionTable.Count > 100000)
        {
            transpositionTable.Clear();
            transpositionHits = 0;
        }

        // Tìm nước đi tốt nhất
        int[] bestMove = FindSmartMove();
        int x = bestMove[0], y = bestMove[1];

        MakeMove(x, y, 'O', oSprite);

        if (CheckGameEnd(x, y, "AI Wins!")) return;

        isPlayerTurn = true;
        stateText.text = "Player Turn";
    }

    void DisableAllButtons()
    {
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                buttons[i, j].interactable = false;
            }
        }
    }

    // Phương pháp tìm kiếm thông minh và hiệu quả hơn
    int[] FindSmartMove()
    {
        // Sử dụng chiến lược mở đầu cho các nước đi đầu tiên
        if (moveCount < 3)
        {
            return GetOpeningMove();
        }

        // Tìm các ô hợp lệ xung quanh các nước đi hiện tại
        List<Vector2Int> validMoves = GetValidMovesNearExistingOnes();

        // Sắp xếp các nước đi theo thứ tự ưu tiên
        List<ScoredMove> scoredMoves = PrioritizeMoves(validMoves);

        // Kiểm tra nước thắng ngay lập tức hoặc nước phòng thủ
        int[] immediateMove = CheckImmediateMoves(scoredMoves);
        if (immediateMove[0] != -1)
        {
            return immediateMove;
        }

        // Sử dụng minimax với cắt tỉa alpha-beta
        return RunMinimaxSearch(scoredMoves);
    }

    private struct ScoredMove
    {
        public Vector2Int Position;
        public int Score;

        public ScoredMove(Vector2Int pos, int score)
        {
            Position = pos;
            Score = score;
        }
    }

    List<ScoredMove> PrioritizeMoves(List<Vector2Int> validMoves)
    {
        List<ScoredMove> scoredMoves = new List<ScoredMove>();

        foreach (var move in validMoves)
        {
            // Đánh giá sơ bộ giá trị của mỗi nước đi
            int score = QuickEvaluate(move.x, move.y);
            scoredMoves.Add(new ScoredMove(move, score));
        }

        // Sắp xếp các nước đi theo thứ tự giảm dần của điểm số
        scoredMoves.Sort((a, b) => b.Score.CompareTo(a.Score));

        return scoredMoves;
    }

    int QuickEvaluate(int x, int y)
    {
        int scoreForO = 0;
        int scoreForX = 0;

        // Kiểm tra nhanh giá trị của ô (x,y) nếu đặt O
        board[x, y] = 'O';
        scoreForO = EvaluatePosition(x, y, 'O');

        // Kiểm tra nhanh giá trị của ô (x,y) nếu đặt X
        board[x, y] = 'X';
        scoreForX = EvaluatePosition(x, y, 'X');

        // Đặt lại ô trống
        board[x, y] = '.';

        // Trả về giá trị cao nhất, ưu tiên giá trị tấn công
        return Mathf.Max(scoreForO * 11 / 10, scoreForX);
    }

    int[] CheckImmediateMoves(List<ScoredMove> scoredMoves)
    {
        // Kiểm tra nước thắng ngay lập tức cho AI
        foreach (var move in scoredMoves)
        {
            board[move.Position.x, move.Position.y] = 'O';
            bool isWinning = IsWon(move.Position.x, move.Position.y);
            board[move.Position.x, move.Position.y] = '.';

            if (isWinning)
            {
                return new int[] { move.Position.x, move.Position.y };
            }
        }

        // Kiểm tra và chặn nước thắng của người chơi
        foreach (var move in scoredMoves)
        {
            board[move.Position.x, move.Position.y] = 'X';
            bool isBlockingWin = IsWon(move.Position.x, move.Position.y);
            board[move.Position.x, move.Position.y] = '.';

            if (isBlockingWin)
            {
                return new int[] { move.Position.x, move.Position.y };
            }
        }

        return new int[] { -1, -1 };
    }

    int[] RunMinimaxSearch(List<ScoredMove> scoredMoves)
    {
        int bestScore = int.MinValue;
        int bestX = -1, bestY = -1;

        // Giới hạn số lượng nước đi để xem xét để tăng tốc độ
        int movesToConsider = Mathf.Min(scoredMoves.Count, 15);

        // Chỉ xem xét các nước đi tốt nhất
        for (int i = 0; i < movesToConsider; i++)
        {
            var move = scoredMoves[i];
            board[move.Position.x, move.Position.y] = 'O';

            // Sử dụng iterative deepening để có thể dừng tìm kiếm sớm
            int score = 0;
            for (int depth = 1; depth <= aiDifficulty; depth++)
            {
                score = Minimax(depth, false, int.MinValue, int.MaxValue);

                // Nếu tìm thấy nước thắng, không cần tìm kiếm sâu hơn
                if (score >= 900000)
                    break;
            }

            board[move.Position.x, move.Position.y] = '.';

            if (score > bestScore)
            {
                bestScore = score;
                bestX = move.Position.x;
                bestY = move.Position.y;
            }
        }

        // Nếu không tìm thấy nước đi tốt, chọn nước đầu tiên
        if (bestX == -1 || bestY == -1)
        {
            bestX = scoredMoves[0].Position.x;
            bestY = scoredMoves[0].Position.y;
        }

        return new int[] { bestX, bestY };
    }

    int[] GetOpeningMove()
    {
        // Nếu là nước đi đầu tiên, chọn vị trí trung tâm
        if (moveCount == 0)
        {
            return new int[] { 9, 9 };
        }

        // Nếu là nước đi thứ hai và người chơi không chọn trung tâm, chọn trung tâm
        if (board[9, 9] == '.')
        {
            return new int[] { 9, 9 };
        }

        // Nếu là nước đi thứ hai và người chơi đã chọn trung tâm
        // Chọn một vị trí gần trung tâm
        int[] offsets = { -1, 0, 1 };
        foreach (int dx in offsets)
        {
            foreach (int dy in offsets)
            {
                int nx = 9 + dx;
                int ny = 9 + dy;
                if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && board[nx, ny] == '.')
                {
                    return new int[] { nx, ny };
                }
            }
        }

        // Trường hợp hiếm gặp, tìm vị trí trống đầu tiên
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                if (board[i, j] == '.')
                {
                    return new int[] { i, j };
                }
            }
        }

        return new int[] { 0, 0 };
    }

    List<Vector2Int> GetValidMovesNearExistingOnes()
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();
        bool[,] considered = new bool[19, 19];

        // Tối ưu: Đặt khoảng cách tìm kiếm dựa trên số lượng nước đi
        int searchDistance = moveCount < 10 ? 2 : 1;

        for (int x = 0; x < 19; x++)
        {
            for (int y = 0; y < 19; y++)
            {
                if (board[x, y] != '.')
                {
                    // Tìm kiếm xung quanh các ô đã được đánh
                    for (int dx = -searchDistance; dx <= searchDistance; dx++)
                    {
                        for (int dy = -searchDistance; dy <= searchDistance; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && board[nx, ny] == '.' && !considered[nx, ny])
                            {
                                validMoves.Add(new Vector2Int(nx, ny));
                                considered[nx, ny] = true;
                            }
                        }
                    }
                }
            }
        }

        // Nếu không tìm thấy nước đi nào (hiếm), trả về một số ô trống
        if (validMoves.Count == 0)
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    if (board[x, y] == '.' && validMoves.Count < 10)
                    {
                        validMoves.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return validMoves;
    }

    int Minimax(int depth, bool isMax, int alpha, int beta)
    {
        // Tạo khóa cho bảng chuyển đổi
        string boardKey = GetBoardHash() + (isMax ? "1" : "0") + depth;

        // Kiểm tra xem đã tính toán vị trí này chưa
        if (transpositionTable.ContainsKey(boardKey))
        {
            var cachedResult = transpositionTable[boardKey];
            if (cachedResult.Depth >= depth && cachedResult.IsExact)
            {
                transpositionHits++;
                return cachedResult.Score;
            }
        }

        // Kiểm tra trạng thái kết thúc
        int terminalScore;
        if (CheckForTerminalState(out terminalScore))
        {
            return terminalScore;
        }

        // Nếu đã đạt đến độ sâu tối đa, đánh giá bảng
        if (depth == 0)
        {
            int evaluation = EvaluateBoard();
            transpositionTable[boardKey] = new AIEvalResult(evaluation, depth, true);
            return evaluation;
        }

        // Chỉ xem xét các nước đi có khả năng cao 
        List<Vector2Int> validMoves = GetValidMovesNearExistingOnes();

        // Tối ưu: Sắp xếp các nước đi để cắt tỉa alpha-beta hiệu quả hơn
        List<ScoredMove> scoredMoves = PrioritizeMoves(validMoves);

        if (isMax)
        {
            int bestScore = int.MinValue;

            foreach (var scoredMove in scoredMoves)
            {
                var move = scoredMove.Position;
                board[move.x, move.y] = 'O';
                int score = Minimax(depth - 1, false, alpha, beta);
                board[move.x, move.y] = '.';

                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);

                if (beta <= alpha)
                    break;
            }

            transpositionTable[boardKey] = new AIEvalResult(bestScore, depth, true);
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            foreach (var scoredMove in scoredMoves)
            {
                var move = scoredMove.Position;
                board[move.x, move.y] = 'X';
                int score = Minimax(depth - 1, true, alpha, beta);
                board[move.x, move.y] = '.';

                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);

                if (beta <= alpha)
                    break;
            }

            transpositionTable[boardKey] = new AIEvalResult(bestScore, depth, true);
            return bestScore;
        }
    }

    // Tối ưu: Sử dụng hàm băm nhanh hơn cho bảng
    string GetBoardHash()
    {
        // Chỉ cần băm khu vực xung quanh nước đi gần đây
        int size = 5; // Kích thước khu vực xung quanh
        int startX = Mathf.Max(0, lastMove.x - size);
        int startY = Mathf.Max(0, lastMove.y - size);
        int endX = Mathf.Min(18, lastMove.x + size);
        int endY = Mathf.Min(18, lastMove.y + size);

        string result = "";
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                result += board[i, j];
            }
        }

        return result;
    }

    bool CheckForTerminalState(out int terminalScore)
    {
        // Tối ưu: Chỉ kiểm tra xung quanh nước đi gần nhất
        if (lastMove.x != -1 && lastMove.y != -1)
        {
            char player = board[lastMove.x, lastMove.y];
            if (IsWon(lastMove.x, lastMove.y))
            {
                terminalScore = player == 'O' ? 1000000 : -1000000;
                return true;
            }
        }

        // Kiểm tra hòa
        if (IsGameDraw())
        {
            terminalScore = 0;
            return true;
        }

        terminalScore = 0;
        return false;
    }

    int EvaluateBoard()
    {
        int score = 0;

        // Tối ưu: chỉ đánh giá các vị trí có quân cờ
        for (int x = 0; x < 19; x++)
        {
            for (int y = 0; y < 19; y++)
            {
                if (board[x, y] != '.')
                {
                    // Đánh giá vị trí
                    int centerDistance = Mathf.Abs(x - 9) + Mathf.Abs(y - 9);
                    int positionalValue = (18 - centerDistance) / 2;

                    if (board[x, y] == 'O')
                    {
                        score += EvaluatePosition(x, y, 'O');
                        score += positionalValue;
                    }
                    else
                    {
                        score -= EvaluatePosition(x, y, 'X');
                        score -= positionalValue;
                    }
                }
            }
        }

        return score;
    }

    int EvaluatePosition(int x, int y, char player)
    {
        int totalScore = 0;

        for (int d = 0; d < 4; d++)
        {
            int count = CountInDirection(x, y, dx[d], dy[d], player);
            int countReverse = CountInDirection(x, y, -dx[d], -dy[d], player);
            int totalCount = count + countReverse + 1;

            bool blocked1 = IsBlocked(x + (count + 1) * dx[d], y + (count + 1) * dy[d]);
            bool blocked2 = IsBlocked(x - (countReverse + 1) * dx[d], y - (countReverse + 1) * dy[d]);

            // Thắng ngay lập tức
            if (totalCount >= 5)
                return 1000000;

            // Đánh giá các chuỗi quân
            if (totalCount == 4)
            {
                if (!blocked1 && !blocked2) totalScore += 500000; // Tứ hai đầu
                else if (!blocked1 || !blocked2) totalScore += 50000; // Tứ một đầu
                else totalScore += 10; // Tứ bị chặn
            }
            else if (totalCount == 3)
            {
                if (!blocked1 && !blocked2) totalScore += 10000; // Tam hai đầu
                else if (!blocked1 || !blocked2) totalScore += 1000; // Tam một đầu
                else totalScore += 5; // Tam bị chặn
            }
            else if (totalCount == 2)
            {
                if (!blocked1 && !blocked2) totalScore += 100; // Nhị hai đầu
                else if (!blocked1 || !blocked2) totalScore += 10; // Nhị một đầu
            }

            // Mẫu đặc biệt
            if (HasSpecialPattern(x, y, dx[d], dy[d], player))
            {
                totalScore += 5000;
            }
        }

        return totalScore;
    }

    bool HasSpecialPattern(int x, int y, int dx, int dy, char player)
    {
        // Kiểm tra các mẫu đặc biệt - tối ưu các trường hợp phổ biến

        // Kiểm tra mẫu O.OO hoặc OO.O
        if (MatchesPattern(x, y, dx, dy, player, '.', player, player) ||
            MatchesPattern(x, y, dx, dy, player, player, '.', player))
            return true;

        return false;
    }

    bool MatchesPattern(int x, int y, int dx, int dy, params char[] pattern)
    {
        // Tìm vị trí bắt đầu
        int startX = x - (pattern.Length / 2) * dx;
        int startY = y - (pattern.Length / 2) * dy;

        // Kiểm tra mẫu
        for (int i = 0; i < pattern.Length; i++)
        {
            int checkX = startX + i * dx;
            int checkY = startY + i * dy;

            if (checkX < 0 || checkX >= 19 || checkY < 0 || checkY >= 19 ||
                (pattern[i] != '.' && board[checkX, checkY] != pattern[i]))
            {
                return false;
            }
        }

        return true;
    }

    bool IsBlocked(int x, int y)
    {
        return x < 0 || x >= 19 || y < 0 || y >= 19 || board[x, y] != '.';
    }

    bool IsWon(int x, int y)
    {
        char player = board[x, y];

        // Tối ưu: Kiểm tra ngay từ vị trí hiện tại
        for (int d = 0; d < 4; d++)
        {
            int count = 1; // Bắt đầu với quân hiện tại

            // Đếm theo một hướng
            for (int i = 1; i < 5; i++)
            {
                int nx = x + i * dx[d];
                int ny = y + i * dy[d];
                if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && board[nx, ny] == player)
                    count++;
                else
                    break;
            }

            // Đếm theo hướng ngược lại
            for (int i = 1; i < 5; i++)
            {
                int nx = x - i * dx[d];
                int ny = y - i * dy[d];
                if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && board[nx, ny] == player)
                    count++;
                else
                    break;
            }

            if (count >= 5)
                return true;
        }

        return false;
    }

    int CountInDirection(int x, int y, int dx, int dy, char player)
    {
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + i * dx, ny = y + i * dy;
            if (nx >= 0 && nx < 19 && ny >= 0 && ny < 19 && board[nx, ny] == player)
                count++;
            else
                break;
        }
        return count;
    }

    private bool IsGameDraw()
    {
        return moveCount >= 19 * 19;
    }
}