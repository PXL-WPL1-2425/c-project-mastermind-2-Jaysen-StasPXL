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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit the game?", "Confirm Exit", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void CheckCode(object sender, RoutedEventArgs e)
        {
            var guesses = new[] {
                ComboBox1.SelectedItem as string,
                ComboBox2.SelectedItem as string,
                ComboBox3.SelectedItem as string,
                ComboBox4.SelectedItem as string
            };

            if (guesses.Any(g => g == null))
            {
                MessageBox.Show("Please select a color for each position.");
                return;
            }

            int redCount = 0;
            int whiteCount = 0;

            var feedback = new List<string>();
            var remainingCode = _generatedCode.ToList();
            var remainingGuess = new List<string>();

            for (int i = 0; i < 4; i++)
            {
                if (guesses[i] == _generatedCode[i])
                {
                    redCount++;
                    feedback.Add("Red");
                    remainingCode[i] = null;
                }
                else
                {
                    feedback.Add(null);
                    remainingGuess.Add(guesses[i]);
                }
            }

            foreach (var guess in remainingGuess)
            {
                if (remainingCode.Contains(guess))
                {
                    whiteCount++;
                    remainingCode[remainingCode.IndexOf(guess)] = null;
                }
            }

            UpdateScore(redCount, whiteCount);
            UpdateHistory(guesses, string.Join(", ", feedback.Where(f => f != null)));

            if (redCount == 4)
            {
                MessageBox.Show("Congratulations! You've cracked the code.");
                AddHighScore();
                InitializeGame();
            }
            else
            {
                NextAttempt();
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("Enter maximum attempts (3-20):", "Settings");
            if (dialog.ShowDialog() == true && int.TryParse(dialog.ResponseText, out int attempts))
            {
                if (attempts >= 3 && attempts <= 20)
                {
                    _maxAttempts = attempts;
                }
                else
                {
                    MessageBox.Show("Please enter a number between 3 and 20.");
                }
            }
        }
        private void ViewHighScores(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Join("\n", _highScores), "High Scores");
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private class InputDialog
        {
            private string v1;
            private string v2;

            public InputDialog(string v1, string v2)
            {
                this.v1 = v1;
                this.v2 = v2;
            }

            public string ResponseText { get; internal set; }

            internal bool ShowDialog()
            {
                throw new NotImplementedException();
            }
        }
    }
}