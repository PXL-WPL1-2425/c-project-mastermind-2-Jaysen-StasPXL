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
        private int _score;
        private List<string> _history;
        private DispatcherTimer _timer;
        private int _timeLeft;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        // Helper: Game Initialisatie
        private void InitializeGame()
        {
            _generatedCode = GenerateRandomCode();
            _attempts = 0;
            _score = 0;
            _history = new List<string>();
            ScoreLabel.Content = "Score: 0";
            HistoryListBox.ItemsSource = null;

            ComboBox1.ItemsSource = _colors;
            ComboBox2.ItemsSource = _colors;
            ComboBox3.ItemsSource = _colors;
            ComboBox4.ItemsSource = _colors;

            Title = "Mastermind Game - Start";
        }

        // Helper: Random Code Generator
        private string[] GenerateRandomCode()
        {
            var random = new Random();
            return Enumerable.Range(0, 4).Select(_ => _colors[random.Next(_colors.Length)]).ToArray();
        }

        // Mastermind-06: Pogingen
        private void NextAttempt()
        {
            _attempts++;
            Title = $"Mastermind - Attempt {_attempts}";
            if (_attempts >= 10)
            {
                MessageBox.Show("Game Over! You've reached the maximum attempts.");
                EndGame();
            }
        }

        // Mastermind-07: Historiek
        private void UpdateHistory(string[] guesses, string feedback)
        {
            _history.Add($"Attempt {_attempts}: {string.Join(", ", guesses)} - {feedback}");
            HistoryListBox.ItemsSource = null;
            HistoryListBox.ItemsSource = _history;
        }

        // Mastermind-08: Score
        private void UpdateScore(int redCount, int whiteCount)
        {
            _score -= (4 - redCount) + whiteCount;
            ScoreLabel.Content = $"Score: {_score}";
        }

        // Mastermind-09: Speleinde
        private void EndGame()
        {
            if (MessageBox.Show("Do you want to play again?", "Play Again", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                InitializeGame();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        // Mastermind-10: Afsluiten
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_attempts < 10)
            {
                var result = MessageBox.Show("Are you sure you want to exit without finishing the game?", "Confirm Exit", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        // Helper: Gok Controleren
        private void CheckCode(object sender, RoutedEventArgs e)
        {
            var guesses = new[] {
                ComboBox1.SelectedItem as string,
                ComboBox2.SelectedItem as string,
                ComboBox3.SelectedItem as string,
                ComboBox4.SelectedItem as string
            };

            // Check if all ComboBoxes have a selected color
            if (guesses.Any(g => g == null))
            {
                MessageBox.Show("Please select a color for each position.");
                return;
            }

            int redCount = 0;
            int whiteCount = 0;

            // Feedback loop (red = correct color and position, white = correct color but wrong position)
            var feedback = new List<string>();
            var remainingCode = _generatedCode.ToList();
            var remainingGuess = new List<string>();

            // First pass: Check for correct colors in the correct position (red)
            for (int i = 0; i < 4; i++)
            {
                if (guesses[i] == _generatedCode[i])
                {
                    redCount++;
                    feedback.Add("Red");
                    remainingCode[i] = null; // Mark this code color as used
                }
                else
                {
                    feedback.Add(null);
                    remainingGuess.Add(guesses[i]);
                }
            }

            // Second pass: Check for correct colors in the wrong position (white)
            foreach (var guess in remainingGuess)
            {
                if (remainingCode.Contains(guess))
                {
                    whiteCount++;
                    remainingCode[remainingCode.IndexOf(guess)] = null; // Mark this code color as used
                }
            }

            // Update score
            UpdateScore(redCount, whiteCount);

            // Add to history
            var feedbackMessage = string.Join(", ", feedback.Where(f => f != null));
            UpdateHistory(guesses, feedbackMessage);

            // Check for win condition
            if (redCount == 4)
            {
                MessageBox.Show("Congratulations! You've cracked the code.");
                EndGame();
            }
            else
            {
                NextAttempt();
            }
        }

    }
}
}  
