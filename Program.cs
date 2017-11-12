// Author: Sewer Sz. (Sewer56lol)
// Date: 2017-10-13
// Purpose: Provides screen backlight control for Intel GPU powered laptop displays through simple terminal commands. (Intended for scripting)

// Imports
using System;
using System.IO;

namespace NETBrightnessControl
{
    class Program
    {
        // Constants
        const string BRIGHTNESS_FILE = "/sys/class/backlight/intel_backlight/brightness";
        const string MAX_BRIGHTNESS_FILE = "/sys/class/backlight/intel_backlight/max_brightness";

        // Action Lists
        public enum Action
        {
            Show_Help = 0x00,
            Increment = 0x01,
            Decrement = 0x02,
            Return_Value = 0x03,
        }

        // Current Action to Perform
        static int action = 0; // (Action)Show_help by default.
        static int brightnessAmount; // Amount as a percentage to change screen brightness by.

        /// Main Method  
        static void Main(string[] args)
        {
            // Iterate over all arguments supplied.
            for (int x = 0; x < args.Length; x++)
            {
                if (args[x] == ("--inc")) { action = (int)Action.Increment; brightnessAmount = Convert.ToInt32(args[x + 1]); } 
                else if (args[x] == ("--dec")) { action = (int)Action.Decrement; brightnessAmount = Convert.ToInt32(args[x + 1]); }
                else if (args[x] == ("--get")) { action = (int)Action.Return_Value; }
            }

            // Show help, else change brightness.
            switch(action)
            {
                case (int)Action.Show_Help:
                    display_Help();
                    break;
                case (int)Action.Decrement: case (int)Action.Increment:
                    alter_Brightness();
                    break;
                case (int)Action.Return_Value:
                    get_Value();
                    break;
            }
        }

        /// Displays the help screen.
        static void display_Help()
        {
            Console.WriteLine(
                "Intended for easy inclusion in scripts, increments or decrements the brightness by a set percentage value.\n"
              + "Syntax: <--inc/--dec> <percentage>\n"
              + "Examples: NETBrightnessControl --inc 10,  NETBrightnessControl --dec 10\n"
              + "To get current percentage value use NETBrightnessControl --get");
            Console.ReadLine();
        }

        // Changes the brightness colour
        static void alter_Brightness()
        {
            // Obtain Initial Brightness
            int initialBrightness = Convert.ToInt32(File.ReadAllText(BRIGHTNESS_FILE));
        
            // Obtain Max Brightness
            int maxBrightness = Convert.ToInt32(File.ReadAllText(MAX_BRIGHTNESS_FILE));

            // Get requested percentage of brightness.
            float brightnessDelta = ( (float) maxBrightness / 100.0F) * (float)brightnessAmount;

            // Increment or decrement brightness value
            if (action == (int)Action.Decrement)  { initialBrightness -= (int)(brightnessDelta); }
            else { initialBrightness += (int)(brightnessDelta); }

            // Ensure no underflow
            if (initialBrightness > maxBrightness) { initialBrightness = maxBrightness; }
            else if (initialBrightness < 0) { initialBrightness = 0; }

            // Write back to file.
            File.WriteAllText(BRIGHTNESS_FILE, Convert.ToString(initialBrightness));
        }

        // Prints out the value of brightness in percentage.
        static void get_Value()
        {
            // Obtain Initial Brightness
            int initialBrightness = Convert.ToInt32(File.ReadAllText(BRIGHTNESS_FILE));
        
            // Obtain Max Brightness
            int maxBrightness = Convert.ToInt32(File.ReadAllText(MAX_BRIGHTNESS_FILE));

            // Current Brightness Percentage
            float currentBrightnessPercentage = ((float)initialBrightness/(float)maxBrightness * 100);

            // Write out current brightness percentage
            Console.WriteLine(currentBrightnessPercentage.ToString("00") + "%");
        }
    }
}
