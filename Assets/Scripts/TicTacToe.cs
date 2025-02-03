using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TicTacToe : MonoBehaviour
{
    [SerializeField] private Sprite _crossSprite;
    [SerializeField] private Sprite _circleSprite;
    [SerializeField] private Image[] _cells;
    private char[] _board = new char[9]; // 'X', 'O', or ' '
    private bool _playerTurn = true;

    private void Start()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            int index = i; 
            _cells[i].gameObject.GetComponent<Button>().onClick.AddListener(() => OnCellClick(index));
        }
        
        ResetBoard();
    }

    private void OnCellClick(int index)
    {
        if (!_playerTurn || _board[index] != ' ') return;
        
        _board[index] = 'X';
        _cells[index].sprite = _crossSprite;
        _playerTurn = false;

        if (CheckWin('X'))
        {
            EndGame("Player win!");
            return;
        }
        
        Invoke("ComputerMove", 0.5f);
    }

    private void ComputerMove()
    {
        int index = GetBestMove();
        if (index == -1)
        {
            EndGame("draw");
            return;
        }

        _board[index] = 'O';
        _cells[index].sprite = _circleSprite;
        
        if (CheckWin('O'))
        {
            EndGame("Computer win!");
            return;
        }
        
        _playerTurn = true;
    }

    private int GetBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < _board.Length; i++)
        {
            if (_board[i] == ' ')
            {
                _board[i] = 'O';
                int score = Minimax(_board, 0, int.MinValue, int.MaxValue, false);
                _board[i] = ' ';
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        
        return bestMove;
    }

    private int Minimax(char[] boardState, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (CheckWin('O')) return 10 - depth;
        if (CheckWin('X')) return depth - 10;
        if (!boardState.Contains(' ')) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            
            for (int i = 0; i < boardState.Length; i++)
            {
                if (boardState[i] == ' ')
                {
                    boardState[i] = 'O';
                    int score = Minimax(boardState, depth + 1, alpha, beta, false);
                    boardState[i] = ' ';
                    bestScore = Mathf.Max(score, bestScore);
                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha) break;
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            
            for (int i = 0; i < boardState.Length; i++)
            {
                if (boardState[i] == ' ')
                {
                    boardState[i] = 'X';
                    int score = Minimax(boardState, depth + 1, alpha, beta, true);
                    boardState[i] = ' ';
                    bestScore = Mathf.Min(score, bestScore);
                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha) break;
                }
            }
            
            return bestScore;
        }
    }

    private bool CheckWin(char player)
    {
        int[,] winPatterns = new int[,] {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // horizontal
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // vertical
            {0, 4, 8}, {2, 4, 6}             // diagonal
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (_board[winPatterns[i, 0]] == player &&
                _board[winPatterns[i, 1]] == player &&
                _board[winPatterns[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    private void EndGame(string message)
    {
        Debug.Log(message);
        Invoke("ResetBoard", 2.0f);
    }

    private void ResetBoard()
    {
        for (int i = 0; i < _board.Length; i++)
        {
            _board[i] = ' ';
            _cells[i].sprite = null;
        }
        
        _playerTurn = true;
    }
}

