using System;
using UnityEngine;

public static class FormatHelper
{
    /// <summary>
    /// Formats amounts according to game specifications:
    /// - Normal display up to 9999.99
    /// - 10,000+ displayed as 10k, 10.1k, etc.
    /// - Decimals only shown if amount has decimal values
    /// </summary>
    public static string FormatAmount(double amount)
    {
        // Handle negative amounts
        bool isNegative = amount < 0;
        amount = Math.Abs(amount);

        string result;

        if (amount < 10000)
        {
            // For amounts under 10k, show as normal
            // Check if it has decimal part
            if (amount % 1 == 0)
            {
                // No decimal part
                result = ((long)amount).ToString();
            }
            else
            {
                // Has decimal part - show 2 decimal places
                result = amount.ToString("F2");
            }
        }
        else
        {
            // For amounts 10k+, use k notation
            double kAmount = amount / 1000;
            
            if (kAmount % 1 == 0)
            {
                // No decimal part
                result = ((long)kAmount) + "k";
            }
            else
            {
                // Has decimal part - show 2 decimal places then trim trailing zeros
                string formatted = kAmount.ToString("F2");
                // Remove trailing zeros after decimal point
                if (formatted.Contains("."))
                {
                    formatted = formatted.TrimEnd('0').TrimEnd('.');
                }
                result = formatted + "k";
            }
        }

        // Add negative sign if original was negative
        if (isNegative)
        {
            result = "-" + result;
        }

        return result;
    }

    /// <summary>
    /// Formats player names as: First letter + *** + Last 2-3 letters
    /// Example: "Ghanshyam" -> "G***yam"
    /// </summary>
    public static string FormatPlayerName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            return "";

        fullName = fullName.Trim();

        if (fullName.Length <= 1)
            return fullName;

        // First letter + 3 asterisks + Last 2-3 letters
        string firstLetter = fullName.Substring(0, 1).ToUpper();
        
        // Get last 2-3 characters (prefer 3 if available, else use what's left)
        int lastLength = Math.Min(3, fullName.Length - 1);
        string lastLetters = fullName.Substring(fullName.Length - lastLength, lastLength).ToLower();

        return firstLetter + "***" + lastLetters;
    }

    /// <summary>
    /// Formats amount for chip display (integer version)
    /// </summary>
    public static string FormatChipAmount(int amount)
    {
        return FormatAmount(amount);
    }

    /// <summary>
    /// Formats amount for chip display (double version)
    /// </summary>
    public static string FormatChipAmount(double amount)
    {
        return FormatAmount(amount);
    }
}
