using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project7_130716
{
    class ConsolePrototype
        {
            public Boolean Enabled;
            
            private string consoleString;
            private string consolePrevString;
            private string consoleLog;
            private Rectangle CONSOLE_REGION;

            public string getString() { return consoleString; }
            public string getPrevString() { return consolePrevString; }
            public string getLog() { return consoleLog; }
            public int getLength() { return consoleString.Length; }
            public void setString(string String) { consoleString = String; }
            public void setPrevString(string String) { consolePrevString = String; }
            public void setLog(string String) { consoleLog = String; }
            public void Close()
            {
                Enabled = false;
                consoleString = "";
            }
            public Rectangle getRegion()
            {
                CONSOLE_REGION = new Rectangle(Form1.Resolution.Width - 520, 0, 520, 50);
                return CONSOLE_REGION;
            }
            public Rectangle getRegionForPrevString()
            {
                return new Rectangle(CONSOLE_REGION.X + 100, CONSOLE_REGION.Y, CONSOLE_REGION.Width - 97, 20);
            }
            public Rectangle getRegionForString()
            {
                return new Rectangle(CONSOLE_REGION.X + 81, CONSOLE_REGION.Y + 25, CONSOLE_REGION.Width - 60, 20);
            }
            public void applyCommand()
            {
                consoleString = consoleString.Trim();
                if (consoleLog == "Unknown command." || consoleLog == "Still unknown command.")
                    consoleLog = "Still unknown command.";
                else
                    consoleLog = "Unknown command.";
                switch (consoleString)
                {
                    case "OUTPUT":
                    case "DEBUG":
                    case "INFORMATION":
                    case "INFO":
                    case "DEBUGINFO":
                    case "DEBUGINFORMATION":
                        if (Form1.ShowDI)
                            Form1.ShowDI = false;
                        else
                            Form1.ShowDI = true;
                        consoleLog = "Debug output is " + (Form1.ShowDI ? "enabled." : "disabled");
                        break;
                    case "REFRESH":
                        Form1.RefreshInventory();
                        consoleLog = "Cooldown was reset.";
                        break;
                    case "QUIT":
                    case "EXIT":
                        Application.Exit();
                        break;
                }
                consolePrevString = consoleString;
                if (consoleString.Length > 0)
                    consoleString = consoleString.Remove(0);
            }
        }
}
