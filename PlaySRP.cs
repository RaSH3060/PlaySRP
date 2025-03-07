using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PlaySRPLauncher
{
    public class GradientPanel : Panel
    {
        [Browsable(true)]
        public Color GradientStart { get; set; } = Color.FromArgb(28, 28, 30);
        [Browsable(true)]
        public Color GradientEnd { get; set; } = Color.FromArgb(40, 40, 45);

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(ClientRectangle, GradientStart, GradientEnd, 45F))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
            base.OnPaint(e);
        }
    }

    public class PlaySRPButton : Button
    {
        private bool _hovered;

        public PlaySRPButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Size = new Size(120, 40);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (var path = new GraphicsPath())
            {
                path.AddArc(new Rectangle(0, 0, 10, 10), 180, 90);
                path.AddArc(new Rectangle(Width - 11, 0, 10, 10), -90, 90);
                path.AddArc(new Rectangle(Width - 11, Height - 11, 10, 10), 0, 90);
                path.AddArc(new Rectangle(0, Height - 11, 10, 10), 90, 90);
                path.CloseAllFigures();

                using (var brush = new SolidBrush(_hovered ? 
                    Color.FromArgb(80, 80, 90) : Color.FromArgb(60, 60, 70)))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(100, 100, 110), 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, 
                _hovered ? Color.White : Color.Silver, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            Invalidate();
        }
    }

    public class PlaySRPListBox : ListBox
    {
        private const int WM_NCPAINT = 0x85;

        public PlaySRPListBox()
        {
            BorderStyle = BorderStyle.None;
            DrawMode = DrawMode.OwnerDrawVariable;
            BackColor = Color.FromArgb(40, 40, 45);
            ForeColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 10F);
            ItemHeight = 40;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCPAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                using (var pen = new Pen(Color.FromArgb(70, 70, 75), 2))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backgroundColor = isSelected ? Color.FromArgb(60, 60, 70) : BackColor;

            using (var brush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            var gameEntry = Items[e.Index] as GameEntry;
            if (gameEntry == null)
            {
                TextRenderer.DrawText(e.Graphics, GetItemText(Items[e.Index]), Font, e.Bounds, ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                return;
            }

            int iconSize = 24;
            int iconMargin = 5;
            Rectangle iconRect = new Rectangle(e.Bounds.X + iconMargin, e.Bounds.Y + (e.Bounds.Height - iconSize) / 2, iconSize, iconSize);
            Image icon = gameEntry.GetIconImage();
            if (icon != null)
            {
                e.Graphics.DrawImage(icon, iconRect);
            }

            Rectangle textRect = new Rectangle(iconRect.Right + iconMargin, e.Bounds.Y, e.Bounds.Width - iconSize - 3 * iconMargin, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, gameEntry.DisplayName, Font, textRect, 
                isSelected ? Color.White : ForeColor, 
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            using (var pen = new Pen(Color.FromArgb(70, 70, 75)))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = ItemHeight;
        }
    }

    public class PlaySRPLauncher : Form
    {
        private const string ConfigFile = "play.ini";
        private PlaySRPListBox listBox;

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [STAThread]
        static void Main()
        {
            
            FreeConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PlaySRPLauncher());
        }

        public PlaySRPLauncher()
        {
            InitializeComponents();
            LoadConfiguration();
        }

        private void InitializeComponents()
        {
            Size = new Size(800, 600);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PlaySRP";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            BackColor = Color.FromArgb(28, 28, 30);

            var mainPanel = new GradientPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var headerLabel = new Label
            {
                Text = "My Game Library",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                TextAlign = ContentAlignment.MiddleLeft
            };

            listBox = new PlaySRPListBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 10)
            };

            listBox.AllowDrop = true;
            listBox.DragEnter += (s, e) => e.Effect = DragDropEffects.Copy;
            listBox.DragDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    foreach (var file in ((string[])e.Data.GetData(DataFormats.FileDrop))
                        .Where(f => f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)))
                    {
                        listBox.Items.Add(new GameEntry(file, "", ""));
                    }
                    SaveConfiguration();
                }
            };

            var buttonPanel = new GradientPanel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                GradientStart = Color.FromArgb(35, 35, 40),
                GradientEnd = Color.FromArgb(28, 28, 30),
                Padding = new Padding(20, 10, 20, 10)
            };

            var playButton = new PlaySRPButton
            {
                Text = "▶ PLAY",
                ForeColor = Color.FromArgb(100, 220, 150),
                Dock = DockStyle.Left
            };

            var addButton = new PlaySRPButton
            {
                Text = "+ ADD",
                ForeColor = Color.FromArgb(100, 180, 220),
                Dock = DockStyle.Left,
                Margin = new Padding(10, 0, 0, 0)
            };

            var deleteButton = new PlaySRPButton
            {
                Text = "✖ DELETE",
                ForeColor = Color.FromArgb(220, 100, 100),
                Dock = DockStyle.Right
            };

            playButton.Click += (s, e) => LaunchGame();
            addButton.Click += AddGame;
            deleteButton.Click += DeleteGame;

            buttonPanel.Controls.AddRange(new Control[] { playButton, addButton, deleteButton });

            var contextMenu = new ContextMenuStrip();
            contextMenu.Renderer = new PlaySRPMenuRenderer();

            var parametersItem = new ToolStripMenuItem("Launch Parameters");
            parametersItem.ForeColor = Color.FromArgb(204, 204, 204);
            parametersItem.Click += EditParameters;
            contextMenu.Items.Add(parametersItem);

            var locationItem = new ToolStripMenuItem("File Location");
            locationItem.ForeColor = Color.FromArgb(204, 204, 224);
            locationItem.Click += FileLocation;
            contextMenu.Items.Add(locationItem);

            var renameItem = new ToolStripMenuItem("Rename");
            renameItem.ForeColor = Color.FromArgb(224, 204, 204);
            renameItem.Click += RenameGame;
            contextMenu.Items.Add(renameItem);

            listBox.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    int index = listBox.IndexFromPoint(e.Location);
                    if (index != ListBox.NoMatches)
                    {
                        listBox.SelectedIndex = index;
                        contextMenu.Show(listBox, e.Location);
                    }
                }
            };

            mainPanel.Controls.Add(listBox);
            mainPanel.Controls.Add(headerLabel);
            mainPanel.Controls.Add(buttonPanel);

            Controls.Add(mainPanel);
        }

        private void LaunchGame()
        {
            var gameEntry = listBox.SelectedItem as GameEntry;
            if (gameEntry == null) return;

            if (!File.Exists(gameEntry.ExePath))
            {
                MessageBox.Show("Executable not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = gameEntry.ExePath,
                    Arguments = gameEntry.Arguments,
                    WorkingDirectory = Path.GetDirectoryName(gameEntry.ExePath),
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Launch failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddGame(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Executable Files|*.exe" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    listBox.Items.Add(new GameEntry(ofd.FileName, "", ""));
                    SaveConfiguration();
                }
            }
        }

        private void DeleteGame(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                listBox.Items.RemoveAt(listBox.SelectedIndex);
                SaveConfiguration();
            }
        }

        private void EditParameters(object sender, EventArgs e)
        {
            var gameEntry = listBox.SelectedItem as GameEntry;
            if (gameEntry == null) return;

            using (var dialog = new ParameterDialog(gameEntry.ExePath, gameEntry.Arguments))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    gameEntry.Arguments = dialog.Arguments;
                    listBox.Items[listBox.SelectedIndex] = gameEntry;
                    SaveConfiguration();
                }
            }
        }

        private void FileLocation(object sender, EventArgs e)
        {
            var gameEntry = listBox.SelectedItem as GameEntry;
            if (gameEntry == null) return;

            if (File.Exists(gameEntry.ExePath))
            {
                try
                {
                    Process.Start("explorer.exe", $"/select,\"{gameEntry.ExePath}\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot open file location: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Executable not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenameGame(object sender, EventArgs e)
        {
            var gameEntry = listBox.SelectedItem as GameEntry;
            if (gameEntry == null) return;

            using (var dialog = new RenameDialog(gameEntry.CustomName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    gameEntry.CustomName = dialog.NewName;
                    listBox.Items[listBox.SelectedIndex] = gameEntry;
                    SaveConfiguration();
                }
            }
        }

        private class ParameterDialog : Form
        {
            public string Arguments { get; private set; }

            public ParameterDialog(string exePath, string arguments)
            {
                Size = new Size(500, 200);
                StartPosition = FormStartPosition.CenterParent;
                Text = "Edit Launch Parameters";
                Font = new Font("Segoe UI", 10F);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                BackColor = Color.FromArgb(40, 40, 45);
                ForeColor = Color.WhiteSmoke;

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 3,
                    ColumnCount = 1,
                    Padding = new Padding(20)
                };

                var pathLabel = new Label
                {
                    Text = $"Executable: {Path.GetFileName(exePath)}",
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 0, 0, 10)
                };

                var argsBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = arguments,
                    BackColor = Color.FromArgb(60, 60, 70),
                    ForeColor = Color.WhiteSmoke
                };

                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };

                var okButton = new PlaySRPButton
                {
                    Text = "SAVE",
                    ForeColor = Color.FromArgb(100, 220, 150),
                    Dock = DockStyle.Right
                };

                var cancelButton = new PlaySRPButton
                {
                    Text = "CANCEL",
                    ForeColor = Color.FromArgb(220, 100, 100),
                    Dock = DockStyle.Right,
                    Margin = new Padding(10, 0, 0, 0)
                };

                okButton.Click += (s, ev) => { Arguments = argsBox.Text; DialogResult = DialogResult.OK; };
                cancelButton.Click += (s, ev) => DialogResult = DialogResult.Cancel;

                buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton });

                layout.Controls.Add(pathLabel, 0, 0);
                layout.Controls.Add(argsBox, 0, 1);
                layout.Controls.Add(buttonPanel, 0, 2);

                Controls.Add(layout);
            }
        }

        private class RenameDialog : Form
        {
            public string NewName { get; private set; }

            public RenameDialog(string currentName)
            {
                Size = new Size(400, 150);
                StartPosition = FormStartPosition.CenterParent;
                Text = "Rename Game";
                Font = new Font("Segoe UI", 10F);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                BackColor = Color.FromArgb(40, 40, 45);
                ForeColor = Color.WhiteSmoke;

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 2,
                    ColumnCount = 1,
                    Padding = new Padding(20)
                };

                var nameBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = currentName,
                    BackColor = Color.FromArgb(60, 60, 70),
                    ForeColor = Color.WhiteSmoke
                };

                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                var okButton = new PlaySRPButton
                {
                    Text = "SAVE",
                    ForeColor = Color.FromArgb(100, 220, 150),
                    Dock = DockStyle.Right
                };

                var cancelButton = new PlaySRPButton
                {
                    Text = "CANCEL",
                    ForeColor = Color.FromArgb(220, 100, 100),
                    Dock = DockStyle.Right,
                    Margin = new Padding(10, 0, 0, 0)
                };

                okButton.Click += (s, e) => { NewName = nameBox.Text; DialogResult = DialogResult.OK; };
                cancelButton.Click += (s, e) => DialogResult = DialogResult.Cancel;

                buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton });

                layout.Controls.Add(nameBox, 0, 0);
                layout.Controls.Add(buttonPanel, 0, 1);

                Controls.Add(layout);
            }
        }

        private class PlaySRPMenuRenderer : ToolStripProfessionalRenderer
        {
            public PlaySRPMenuRenderer() : base(new PlaySRPColors()) { }
        }

        private class PlaySRPColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(60, 60, 70);
            public override Color MenuItemBorder => Color.FromArgb(80, 80, 90);
            public override Color MenuBorder => Color.FromArgb(70, 70, 75);
            public override Color ImageMarginGradientBegin => Color.FromArgb(40, 40, 45);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(40, 40, 45);
            public override Color ImageMarginGradientEnd => Color.FromArgb(40, 40, 45);
            public override Color ToolStripDropDownBackground => Color.FromArgb(40, 40, 45);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(60, 60, 70);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(60, 60, 70);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(50, 50, 55);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(50, 50, 55);
        }

        public static bool ParseCommand(string command, out string exePath, out string arguments)
        {
            exePath = "";
            arguments = "";
            command = command.Trim();
            if (command.StartsWith("\""))
            {
                int endQuote = command.IndexOf('\"', 1);
                if (endQuote == -1) return false;
                exePath = command.Substring(1, endQuote - 1); 
                arguments = command.Substring(endQuote + 1).Trim(); 
            }
            else
            {
                int spaceIndex = command.IndexOf(' ');
                exePath = spaceIndex == -1 ? command : command.Substring(0, spaceIndex);
                arguments = spaceIndex == -1 ? "" : command.Substring(spaceIndex + 1).Trim();
            }
            return true;
        }

        void LoadConfiguration()
        {
            if (!File.Exists(ConfigFile)) return;
            try
            {
                foreach (var line in File.ReadAllLines(ConfigFile, Encoding.UTF8))
                {
                    if (GameEntry.TryParse(line, out GameEntry gameEntry))
                    {
                        listBox.Items.Add(gameEntry);
                    }
                }
            }
            catch { }
        }

        void SaveConfiguration()
        {
            try
            {
                var lines = listBox.Items.Cast<GameEntry>().Select(g => g.ToString());
                File.WriteAllLines(ConfigFile, lines, Encoding.UTF8);
            }
            catch { }
        }
    }

    public class GameEntry
    {
        public string ExePath { get; set; }
        public string Arguments { get; set; }
        public string CustomName { get; set; }

        public GameEntry(string exePath, string arguments, string customName = "")
        {
            ExePath = exePath;
            Arguments = arguments;
            CustomName = customName;
        }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(CustomName) ? Path.GetFileNameWithoutExtension(ExePath) : CustomName; }
        }

        public override string ToString()
        {
            return $"play|{ExePath}|{Arguments}|{CustomName}";
        }

        public static bool TryParse(string entry, out GameEntry gameEntry)
        {
            gameEntry = null;
            if (!entry.StartsWith("play|")) return false;
            string[] parts = entry.Split(new char[] { '|' }, 4);
            if (parts.Length < 3)
                return false;
            string exePath = parts[1];
            string arguments = parts[2];
            string customName = parts.Length == 4 ? parts[3] : "";
            gameEntry = new GameEntry(exePath, arguments, customName);
            return true;
        }

        private static Dictionary<string, Image> _iconCache = new Dictionary<string, Image>();

        public Image GetIconImage()
        {
            if (_iconCache.TryGetValue(ExePath, out var image))
                return image;
            try
            {
                Icon icon = Icon.ExtractAssociatedIcon(ExePath);
                image = icon?.ToBitmap();
                _iconCache[ExePath] = image;
            }
            catch { }
            return image;
        }
    }
}
