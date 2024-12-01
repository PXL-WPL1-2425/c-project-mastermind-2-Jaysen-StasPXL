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

       