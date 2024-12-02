using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MastermindGame
{
    public partial class MainWindow : Window
    {
        private readonly string[] _colors = { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private string[] _generatedCode;
        private int _attempts;
        private int _maxAttempts;
        private int _score;
        private List<string> _history;
        private DispatcherTimer _timer;
        private int _timeLeft;
        private string _playerName;
        private List<string> _highScores;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            _generatedCode = GenerateRandomCode();
            _attempts = 0;
            _score = 0;
            _history = new List<string>();
            _highScores = _highScores ?? new List<string>();
            _maxAttempts = 10; // Default value, can be updated by user

            ScoreLabel.Content = "Score: 0";
            HistoryListBox.ItemsSource = null;

            ComboBox1.ItemsSource = _colors;
            ComboBox2.ItemsSource = _colors;
            ComboBox3.ItemsSource = _colors;
            ComboBox4.ItemsSource = _colors;

            Title = "Mastermind Game - Start";

            AskPlayerName();
        }

        private void AskPlayerName()
        {
            do
            {
                var dialog = new InputDialog("Enter your name:", "Player Name");
                if (dialog.ShowDialog() == true)
                {
                    _playerName = dialog.ResponseText;
                }
            } while (string.IsNullOrWhiteSpace(_playerName));
        }

        private string[] GenerateRandomCode()
        {
            var random = new Random();
            return Enumerable.Range(0, 4).Select(_ => _colors[random.Next(_colors.Length)]).ToArray();
        }

        private void NextAttempt()
        {
            _attempts++;
            Title = $"Mastermind - Attempt {_attempts}";
            if (_attempts >= _maxAttempts)
            {
                MessageBox.Show("Game Over! You've reached the maximum attempts.");
                AddHighScore();
                EndGame();
            }
        }

        private void AddHighScore()
        {
            string highScore = $"{_playerName} - {_attempts} attempts - {_score}/100";
            if (_highScores.Count < 15)
            {
                _highScores.Add(highScore);
            }
            else
            {
                _highScores.RemoveAt(0);
                _highScores.Add(highScore);
            }
        }

        private void UpdateHistory(string[] guesses, string feedback)
        {
            _history.Add($"Attempt {_attempts}: {string.Join(", ", guesses)} - {feedback}");
            HistoryListBox.ItemsSource = null;
            HistoryListBox.ItemsSource = _history;
        }

        private void UpdateScore(int redCount, int whiteCount)
        {
            _score -= (4 - redCount) + whiteCount;
            ScoreLabel.Content = $"Score: {_score}";
        }

        private void EndGame()
        {
            AddHighScore();
            InitializeGame();
        }
