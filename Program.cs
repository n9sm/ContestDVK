using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace ContestDVK
{
    internal sealed class AudioTrack : IDisposable
    {
        private readonly string filePath;
        private SoundPlayer player;
        private Timer timer;
        private bool looping;
        private int delaySeconds = 2;
        private int volume = 50; // SoundPlayer does not expose volume control.

        public AudioTrack(string path)
        {
            filePath = path;
            player = new SoundPlayer(path);
            player.Load();
            timer = new Timer();
            timer.Interval = delaySeconds * 1000;
            timer.Tick += (s, e) => Play();
        }

        public void Play()
        {
            try
            {
                player.Stop();
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossibile riprodurre il file:\n" + filePath + "\n\n" + ex.Message,
                    "Contest DVK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StartLoop()
        {
            looping = true;
            Play();
            timer.Interval = Math.Max(1, delaySeconds) * 1000;
            timer.Start();
        }

        public void StopLoop()
        {
            looping = false;
            timer.Stop();
            player.Stop();
        }

        public void SetDelay(int value)
        {
            delaySeconds = value;
            if (looping)
                timer.Interval = delaySeconds * 1000;
        }

        public void SetVolume(int value)
        {
            // SoundPlayer has no volume API. Keep the control visible but explain limitation.
            volume = value;
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
            player.Stop();
            player.Dispose();
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly AudioTrack[] tracks;

        public MainForm()
        {
            Text = "IZ3NVR/N9SM Contest DVK";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(760, 230);
            ClientSize = new Size(900, 230);
            Font = new Font("Segoe UI", 9F);

            string audio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio");
            string[] names = { "CQ_NVR.wav", "IZ3NVR.wav", "59_15.wav" };
            tracks = new AudioTrack[names.Length];
            
            var main = new TableLayoutPanel();
            main.Dock = DockStyle.Fill;
            main.ColumnCount = 1;
            main.RowCount = names.Length + 1;
            main.Padding = new Padding(10);
            main.AutoSize = false;
            for (int i = 0; i < names.Length; i++)
                main.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / (names.Length + 1)));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            Controls.Add(main);

            for (int i = 0; i < names.Length; i++)
            {
                string path = Path.Combine(audio, names[i]);
                var row = new FlowLayoutPanel();
                row.Dock = DockStyle.Fill;
                row.WrapContents = false;
                row.AutoScroll = true;
                row.Padding = new Padding(0, 5, 0, 2);

                var label = new Label();
                label.Text = "Track " + (i + 1) + "  (" + names[i] + ")";
                label.Width = 205;
                label.TextAlign = ContentAlignment.MiddleLeft;
                row.Controls.Add(label);

                if (!File.Exists(path))
                {
                    var missing = new Label();
                    missing.Text = "File audio mancante";
                    missing.Width = 170;
                    missing.ForeColor = Color.DarkRed;
                    missing.TextAlign = ContentAlignment.MiddleLeft;
                    row.Controls.Add(missing);
                    AddRowButtons(row, null, i);
                }
                else
                {
                    try
                    {
                        tracks[i] = new AudioTrack(path);
                        AddRowButtons(row, tracks[i], i);
                    }
                    catch (Exception ex)
                    {
                        var err = new Label();
                        err.Text = "Errore caricamento WAV: " + ex.Message;
                        err.Width = 280;
                        row.Controls.Add(err);
                        AddRowButtons(row, null, i);
                    }
                }

                main.Controls.Add(row, 0, i);
            }

            var note = new Label();
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            note.ForeColor = Color.DimGray;
            note.Text = "Nota: il controllo volume è visualizzato, ma SoundPlayer non supporta la regolazione del volume.";
            main.Controls.Add(note, 0, names.Length);

            FormClosing += (s, e) =>
            {
                foreach (var track in tracks)
                    if (track != null) track.Dispose();
            };
        }

        private void AddRowButtons(FlowLayoutPanel row, AudioTrack track, int index)
        {
            var play = new Button { Text = "Play", Width = 65, Height = 28, Enabled = track != null };
            play.Click += (s, e) => track.Play();
            row.Controls.Add(play);

            var loop = new Button { Text = "Loop", Width = 65, Height = 28, Enabled = track != null };
            loop.Click += (s, e) => track.StartLoop();
            row.Controls.Add(loop);

            var stop = new Button { Text = "Stop", Width = 65, Height = 28, Enabled = track != null };
            stop.Click += (s, e) => track.StopLoop();
            row.Controls.Add(stop);

            var volume = new TrackBar
            {
                Minimum = 0, Maximum = 100, Value = 50, TickFrequency = 25,
                Width = 125, Height = 35, Enabled = track != null
            };
            volume.Scroll += (s, e) => { if (track != null) track.SetVolume(volume.Value); };
            volume.ValueChanged += (s, e) =>
            {
                volume.AccessibleDescription = "Volume " + volume.Value +
                    " (non supportato dal motore audio SoundPlayer)";
            };
            row.Controls.Add(volume);

            var delayLabel = new Label { Text = "Delay (s)", Width = 60, Height = 28, TextAlign = ContentAlignment.MiddleCenter };
            row.Controls.Add(delayLabel);
            var delay = new NumericUpDown { Minimum = 1, Maximum = 60, Value = 2, Width = 55, Enabled = track != null };
            delay.ValueChanged += (s, e) => { if (track != null) track.SetDelay((int)delay.Value); };
            row.Controls.Add(delay);
        }
    }

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
