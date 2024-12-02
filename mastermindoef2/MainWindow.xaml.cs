// Mastermind-10: Afsluiten
using System.Windows.Controls;
using System.Windows;

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
